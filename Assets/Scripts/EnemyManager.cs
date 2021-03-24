using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Jobs;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;
    public bool recycleZombies = true;
    public int MaxEnemies;
    public float originalSpawnRate;
    public float spawnRate;
    public List<Enemy> activeEnemies;
    public int orginalMaxZombies;

    [SerializeField]
    private Light2D center;
    [SerializeField]
    private int spawnRadius;
    [SerializeField]
    private Enemy ZombiePrefab;
    [SerializeField]
    private int attackRange;
    [SerializeField]
    private float cooldown;
    [SerializeField]
    private float _Visibility = 10f;

    private float _spawnRate;
    private int index = 0;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        activeEnemies = new List<Enemy>();
        activeEnemies.AddRange(GetComponentsInChildren<Enemy>());
        spawnRadius = Mathf.RoundToInt(Mathf.Max(center.pointLightOuterRadius + 1, spawnRadius));
        MaxEnemies = GameManager.instance.SelectedDifficulty.StartingMaxZombies;
        orginalMaxZombies = MaxEnemies;
        originalSpawnRate = spawnRate;
        //DayNightCycle.instance.OnDayPeriodChange.AddListener(ChangeVisibility);
    }

    private void spawnEnemies()
    {
        Vector2 point = LightManager.instance.FindValidSpawnPosition();
        Enemy newZombie = Instantiate(ZombiePrefab, point, Quaternion.identity, transform);
        if (activeEnemies.Count < MaxEnemies)
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
    private void Update()
    {
        _Visibility = GameManager.instance.SelectedDifficulty.ComputeVisibility(DayNightCycle.instance._TimeValue/24f);
        _spawnRate -= Time.deltaTime;
        if (_spawnRate <= 0)
        {
            spawnEnemies();
            _spawnRate = spawnRate;
        }
        QueueEnemyAIJobs();
        AttackTargets();
    }

    private void LateUpdate()
    {
        moveJobHandle.Complete();
        ApplyJobResults();
        DisposeJobData();
    }

    private NativeMultiHashMap<int, Vector2> relativePositions;
    private TransformAccessArray transformAccessArray;
    private NativeArray<Vector2> targets, previousTargetNativeArray, movePositionsNativeArray;
    private NativeArray<float> timeTillNextNativeArray;
    private JobHandle moveJobHandle;

    private void QueueEnemyAIJobs()
    {
        Transform[] transforms = new Transform[activeEnemies.Count];
        Vector2[] previousTargets = new Vector2[activeEnemies.Count];
        float[] timeTillNext = new float[activeEnemies.Count];
        relativePositions = new NativeMultiHashMap<int, Vector2>(activeEnemies.Count, Allocator.TempJob);
        //Iterate through all enemies storing the necessary info.
        for (int i = 0; i < activeEnemies.Count; i++)
        {
            transforms[i] = activeEnemies[i].transform;
            previousTargets[i] = activeEnemies[i].target;
            timeTillNext[i] = activeEnemies[i].timeTillNextRandom;
            for (int j = 0; j < activeEnemies[i].influencingLights.Count; j++)
            {
                relativePositions.Add(i, activeEnemies[i].influencingLights[j].transform.position);
            }
        }
        transformAccessArray = new TransformAccessArray(transforms, 10);
        targets = new NativeArray<Vector2>(activeEnemies.Count, Allocator.TempJob);
        previousTargetNativeArray = new NativeArray<Vector2>(previousTargets, Allocator.TempJob);
        movePositionsNativeArray = new NativeArray<Vector2>(activeEnemies.Count, Allocator.TempJob);
        timeTillNextNativeArray = new NativeArray<float>(timeTillNext, Allocator.TempJob);
        FindTargetsJob findTargets = new FindTargetsJob
        {
            relativeTargets = relativePositions,
            previousTargets = previousTargetNativeArray,
            deltaTime = Time.deltaTime,
            timeTillNextRandomDirection = timeTillNextNativeArray,
            random = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(1, 100000)),
            results = targets,
            playerPosition = Player.instance.transform.position,
            EnemyVisibility = _Visibility
        };
        MoveEnemiesJob moveJob = new MoveEnemiesJob
        {
            deltaTime = Time.fixedDeltaTime,
            targetPositions = targets,
            movePositions = movePositionsNativeArray
        };
        JobHandle findTargetsJobHandle = findTargets.Schedule(transformAccessArray);
        moveJobHandle = moveJob.Schedule(transformAccessArray, findTargetsJobHandle);
    }

    private void ApplyJobResults()
    {
        for (int i = 0; i < activeEnemies.Count; i++)
        {
            Enemy enemy = activeEnemies[i];
            enemy.rigidbody.MovePosition(movePositionsNativeArray[i]);
            enemy.target = targets[i];
            enemy.timeTillNextRandom = timeTillNextNativeArray[i];
        }
    }

    private void DisposeJobData()
    {
        movePositionsNativeArray.Dispose();
        previousTargetNativeArray.Dispose();
        timeTillNextNativeArray.Dispose();
        transformAccessArray.Dispose();
        targets.Dispose();
        relativePositions.Dispose();
    }

    private void AttackTargets()
    {
        foreach(Enemy enemy in activeEnemies)
        {
            RaycastHit2D hit = Physics2D.Raycast(enemy.transform.position, enemy.transform.right, attackRange, LayerMask.GetMask("Building"));
            bool playerInRange = Vector2.Distance(Player.instance.transform.position, enemy.transform.position) < attackRange;
            bool hitBuilding = hit.collider != null;
            if (playerInRange || hitBuilding)
            {
                enemy.attackCooldown -= Time.deltaTime;
                if (enemy.attackCooldown <= 0.01f)
                {
                    if (hitBuilding && hit.collider.TryGetComponent(out Building building))
                    {
                        building.Damage(enemy.AttackDamage);
                    }
                    else if (playerInRange)
                    {
                        Player.instance.Damage(enemy.AttackDamage);
                    }
                    enemy.animator.SetTrigger("Attack");
                    enemy.attackCooldown = cooldown;
                }
            }
        }
    }
    private struct MoveEnemiesJob : IJobParallelForTransform
    {
        [ReadOnly]
        public float deltaTime;
        [ReadOnly]
        public NativeArray<Vector2> targetPositions;
        [WriteOnly]
        public NativeArray<Vector2> movePositions;

        public void Execute(int index, TransformAccess transform)
        {
            Vector2 dir = targetPositions[index] - (Vector2)transform.position;
            float angle = math.degrees(math.atan2(dir.y, dir.x));
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            movePositions[index] = (Vector2)transform.position + (Vector2.Distance(transform.position, targetPositions[index]) > 2f ? (Vector2)(transform.rotation * Vector2.right * (deltaTime * 2)) : Vector2.zero);
        }
    }

    private struct FindTargetsJob : IJobParallelForTransform
    {
        [ReadOnly]
        public NativeMultiHashMap<int, Vector2> relativeTargets;
        [ReadOnly]
        public NativeArray<Vector2> previousTargets;
        [WriteOnly]
        public NativeArray<Vector2> results;
        public NativeArray<float> timeTillNextRandomDirection;
        [ReadOnly]
        public Vector2 playerPosition;

        public float deltaTime;
        public float EnemyVisibility;
        public Unity.Mathematics.Random random;

        public void Execute(int index, TransformAccess transform)
        {
            timeTillNextRandomDirection[index] -= deltaTime;
            //Exit early if player is in range
            if (Vector2.Distance(playerPosition, transform.position) < EnemyVisibility)
            {
                results[index] = playerPosition;
                return;
            }
            Vector2 closest = Vector2.zero;
            float lastClosest = float.MaxValue;
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

    public NativeArray<float2> GetActiveEnemyPositionsAsNativeArray(Allocator allocator)
    {
        NativeArray<float2> result = new NativeArray<float2>(activeEnemies.Count, allocator, NativeArrayOptions.UninitializedMemory);
        for (int i = 0; i < activeEnemies.Count; i++)
        {
            result[i] = (Vector2)activeEnemies[i].transform.position;
        }
        return result;
    }

    private void ChangeVisibility(PartOfDay period)
    {
        switch (period)
        {
            case PartOfDay.Morning:
                _Visibility = 10;
                break;
            case PartOfDay.Noon:
                _Visibility = 30;
                break;
            case PartOfDay.Afternoon:
                _Visibility = 50;
                break;
            case PartOfDay.Evening:
                _Visibility = 15;
                break;
            case PartOfDay.Night:
                _Visibility = 5;
                break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(Vector3.zero, center.pointLightOuterRadius);
        Gizmos.DrawWireSphere(Vector3.zero, spawnRadius);
    }
}
