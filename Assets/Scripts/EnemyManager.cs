using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.Experimental.Rendering.Universal;

public class EnemyManager : MonoBehaviour
{
    public List<Enemy> activeEnemies;
    [SerializeField]
    private Light2D center;
    [SerializeField]
    private int spawnRadius;
    [SerializeField]
    private Enemy ZombiePrefab;
    [SerializeField]
    public int maxEnemies;
    public float originalSpawnRate;
    public float spawnRate;
    private float _spawnRate;
    [SerializeField]
    public int orginalMaxZombies;
    [SerializeField]
    private int attackRange;
    [SerializeField]
    private float cooldown;

    private int index = 0;

    public static EnemyManager instance;
    public bool recycleZombies = true;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        activeEnemies = new List<Enemy>();
        activeEnemies.AddRange(GetComponentsInChildren<Enemy>());
        //InvokeRepeating("spawnEnemies", 1, 1);
        spawnRadius = Mathf.RoundToInt(Mathf.Max(center.pointLightOuterRadius + 1, spawnRadius));
        orginalMaxZombies = maxEnemies;
        originalSpawnRate = spawnRate;
    }
    private void spawnEnemies()
    {
        float innerRadius = center.pointLightOuterRadius;
        float ratio = innerRadius / spawnRadius;
        float radius = Mathf.Sqrt(UnityEngine.Random.Range(ratio * ratio, 1f)) * spawnRadius;
        Vector2 point = UnityEngine.Random.insideUnitCircle.normalized * radius;
        Enemy newZombie = Instantiate(ZombiePrefab, point, Quaternion.identity, transform);
        if (activeEnemies.Count < maxEnemies)
            activeEnemies.Add(newZombie);
        else if (recycleZombies)
        {
            if (index >= activeEnemies.Count)
            {
                index = 0;
            }
            Destroy(activeEnemies[index].gameObject);
            activeEnemies[index] = newZombie;
            index++;
        }
    }
    // Update is called once per frame
    void Update()
    {
        _spawnRate -= Time.deltaTime;
        if (_spawnRate <= 0)
        {
            spawnEnemies();
            _spawnRate = spawnRate;
        }
        QueueEnemyAIJobs();
    }
    private void LateUpdate()
    {
        attackJobHandle.Complete();
        ApplyJobResults();
        DisposeJobData();
    }
    NativeMultiHashMap<int, Vector2> relativePositions;
    TransformAccessArray transformAccessArray;
    NativeArray<Vector2> targets, previousTargetNativeArray, movePositionsNativeArray;
    NativeArray<float> timeTillNextNativeArray, cooldownNativeArray;
    JobHandle attackJobHandle;
    private void QueueEnemyAIJobs()
    {
        Transform[] transforms = new Transform[activeEnemies.Count];
        Vector2[] previousTargets = new Vector2[activeEnemies.Count];
        float[] timeTillNext = new float[activeEnemies.Count];
        float[] attackCooldowns = new float[activeEnemies.Count];
        relativePositions = new NativeMultiHashMap<int, Vector2>(activeEnemies.Count, Allocator.TempJob);
        for (int i = 0; i < activeEnemies.Count; i++)
        {
            transforms[i] = activeEnemies[i].transform;
            previousTargets[i] = activeEnemies[i].target;
            timeTillNext[i] = activeEnemies[i].timeTillNextRandom;
            attackCooldowns[i] = activeEnemies[i].attackCooldown;
            for (int j = 0; j < activeEnemies[i].influencingLights.Count; j++)
            {
                relativePositions.Add(i, activeEnemies[i].influencingLights[j].transform.position);
            }
        }
        transformAccessArray = new TransformAccessArray(transforms, 100);
        targets = new NativeArray<Vector2>(activeEnemies.Count, Allocator.TempJob);
        previousTargetNativeArray = new NativeArray<Vector2>(previousTargets, Allocator.TempJob);
        movePositionsNativeArray = new NativeArray<Vector2>(activeEnemies.Count, Allocator.TempJob);
        timeTillNextNativeArray = new NativeArray<float>(timeTillNext, Allocator.TempJob);
        cooldownNativeArray = new NativeArray<float>(attackCooldowns, Allocator.TempJob);
        FindTargetsJob findTargets = new FindTargetsJob
        {
            relativeTargets = relativePositions,
            previousTargets = previousTargetNativeArray,
            deltaTime = Time.deltaTime,
            timeTillNextRandomDirection = timeTillNextNativeArray,
            random = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(1, 100000)),
            results = targets,
            playerPosition = Player.instance.transform.position
        };
        MoveEnemiesJob moveJob = new MoveEnemiesJob
        {
            deltaTime = Time.fixedDeltaTime,
            targetPositions = targets,
            movePositions = movePositionsNativeArray
        };
        AttackPlayerJob attackPlayerJob = new AttackPlayerJob
        {
            playerPos = Player.instance.transform.position,
            attackRange = attackRange,
            cooldown = cooldown,
            deltaTime = Time.deltaTime,
            AttackCooldowns = cooldownNativeArray
        };
        JobHandle findTargetsJobHandle = findTargets.Schedule(transformAccessArray);
        JobHandle moveJobHandle = moveJob.Schedule(transformAccessArray, findTargetsJobHandle);
        attackJobHandle = attackPlayerJob.Schedule(transformAccessArray, moveJobHandle);
    }

    private void ApplyJobResults()
    {
        for (int i = 0; i < activeEnemies.Count; i++)
        {
            activeEnemies[i].rigidbody.MovePosition(movePositionsNativeArray[i]);
            activeEnemies[i].target = targets[i];
            activeEnemies[i].attackCooldown = cooldownNativeArray[i];
            if (cooldownNativeArray[i] < .01f)
            {
                activeEnemies[i].animator.SetTrigger("Attack");
                activeEnemies[i].attackCooldown = cooldown;
                Player.instance.Damage(10);
            }
            activeEnemies[i].timeTillNextRandom = timeTillNextNativeArray[i];
        }
    }
    private void DisposeJobData()
    {
        movePositionsNativeArray.Dispose();
        cooldownNativeArray.Dispose();
        previousTargetNativeArray.Dispose();
        timeTillNextNativeArray.Dispose();
        transformAccessArray.Dispose();
        targets.Dispose();
        relativePositions.Dispose();
    }

    struct MoveEnemiesJob : IJobParallelForTransform
    {
        public float deltaTime;
        public NativeArray<Vector2> targetPositions;
        public NativeArray<Vector2> movePositions;
        public void Execute(int index, TransformAccess transform)
        {
            //transform.position = Vector2.Lerp(transform.position, targetPositions[index], deltaTime);
            Vector2 dir = targetPositions[index] - (Vector2)transform.position;
            float angle = math.degrees(math.atan2(dir.y, dir.x));
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            movePositions[index] = (Vector2)transform.position + (Vector2.Distance(transform.position, targetPositions[index]) > 2f ? (Vector2)(transform.rotation * Vector2.right * (deltaTime * 2)) : Vector2.zero);
        }
    }
    struct FindTargetsJob : IJobParallelForTransform
    {
        [ReadOnly]
        public NativeMultiHashMap<int, Vector2> relativeTargets;
        public NativeArray<Vector2> previousTargets;
        public NativeArray<Vector2> results;
        public NativeArray<float> timeTillNextRandomDirection;
        public Vector2 playerPosition;
        public float deltaTime;
        public Unity.Mathematics.Random random;
        public void Execute(int index, TransformAccess transform)
        {
            Vector2 closest = Vector2.zero;
            float lastClosest = float.MaxValue;
            timeTillNextRandomDirection[index] -= deltaTime;
            if (Vector2.Distance(playerPosition, transform.position) < 10f)
            {
                results[index] = playerPosition;
                return;
            }
            if (relativeTargets.TryGetFirstValue(index, out Vector2 target, out NativeMultiHashMapIterator<int> it))
            {
                do
                {
                    float distance = Vector2.Distance(transform.position, target);
                    if (distance < lastClosest)
                    {
                        closest = target;
                        lastClosest = distance;
                    }
                } while (relativeTargets.TryGetNextValue(out target, ref it));
                results[index] = closest;
            }
            else
            {
                if (timeTillNextRandomDirection[index] < .01f)
                {
                    timeTillNextRandomDirection[index] = 2;
                    results[index] = (Vector2)transform.position + (Vector2)random.NextFloat2Direction() * 10f;
                }
                else
                {
                    results[index] = previousTargets[index];
                }
            }
        }
    }
    struct AttackPlayerJob : IJobParallelForTransform
    {
        public NativeArray<float> AttackCooldowns;
        public Vector2 playerPos;
        public float deltaTime;
        public float cooldown;
        public int attackRange;
        public void Execute(int index, TransformAccess transform)
        {
            if (Vector2.Distance(transform.position, playerPos) < attackRange)
                AttackCooldowns[index] -= deltaTime;
            else
                AttackCooldowns[index] = cooldown;
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(Vector3.zero, center.pointLightOuterRadius);
        Gizmos.DrawWireSphere(Vector3.zero, spawnRadius);
    }
}
