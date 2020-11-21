using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;

public class EnemyManager : MonoBehaviour
{
    List<Enemy> activeEnemies;
    [SerializeField]
    private Rect spawnBounds;
    [SerializeField]
    private Enemy ZombiePrefab;
    [SerializeField]
    private int maxEnemies;
    void Start()
    {
        activeEnemies = new List<Enemy>();
        activeEnemies.AddRange(GetComponentsInChildren<Enemy>());
        InvokeRepeating("spawnEnemies", 1, 1);
    }
    private void spawnEnemies()
    {
        if (activeEnemies.Count < maxEnemies)
        activeEnemies.Add(
            Instantiate(ZombiePrefab, new Vector2(
                                          UnityEngine.Random.Range(spawnBounds.xMin, spawnBounds.xMax),
                                          UnityEngine.Random.Range(spawnBounds.yMin, spawnBounds.yMax)
                                          ),
                                      Quaternion.identity,
                                      transform));
    }
    // Update is called once per frame
    void Update()
    {
        Transform[] transforms = new Transform[activeEnemies.Count];
        Vector2[] previousTargets = new Vector2[activeEnemies.Count];
        float[] timeTillNext = new float[activeEnemies.Count];
        NativeMultiHashMap<int, Vector2> relativePositions = new NativeMultiHashMap<int, Vector2>(activeEnemies.Count, Allocator.TempJob);
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
        TransformAccessArray transformAccessArray = new TransformAccessArray(transforms, 100);
        NativeArray<Vector2> targets = new NativeArray<Vector2>(activeEnemies.Count, Allocator.TempJob);
        NativeArray<Vector2> previousTargetNativeArray = new NativeArray<Vector2>(previousTargets, Allocator.TempJob);
        NativeArray<float> timeTillNextNativeArray = new NativeArray<float>(timeTillNext, Allocator.TempJob);
        FindTargetsJob findTargets = new FindTargetsJob
        {
            relativeTargets = relativePositions,
            previousTargets = previousTargetNativeArray,
            deltaTime = Time.deltaTime,
            timeTillNextRandomDirection = timeTillNextNativeArray,
            random = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(0, 100000)),
            results = targets
        };
        MoveEnemiesJob moveJob = new MoveEnemiesJob
        {
            deltaTime = Time.deltaTime,
            targetPositions = targets
        };
        JobHandle findTargetsJobHandle = findTargets.Schedule(transformAccessArray);
        JobHandle moveJobHandle = moveJob.Schedule(transformAccessArray, findTargetsJobHandle);
        moveJobHandle.Complete();
        for (int i = 0; i < activeEnemies.Count; i++)
        {
            activeEnemies[i].target = targets[i];
            //activeEnemies[i].transform.LookAt(targets[i]);
            activeEnemies[i].timeTillNextRandom = timeTillNextNativeArray[i];
        }
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
        public void Execute(int index, TransformAccess transform)
        {
            transform.position = Vector2.Lerp(transform.position, targetPositions[index], deltaTime);
            Vector2 dir = targetPositions[index] - (Vector2)transform.position;
            float angle = math.degrees(math.atan2(dir.y, dir.x));
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
    struct FindTargetsJob : IJobParallelForTransform
    {
        [ReadOnly]
        public NativeMultiHashMap<int, Vector2> relativeTargets;
        public NativeArray<Vector2> previousTargets;
        public NativeArray<Vector2> results;
        public NativeArray<float> timeTillNextRandomDirection;
        public float deltaTime;
        public Unity.Mathematics.Random random;
        public void Execute(int index, TransformAccess transform)
        {
            Vector2 closest = Vector2.zero;
            float lastClosest = float.MaxValue;
            timeTillNextRandomDirection[index] -= deltaTime;
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
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube((Vector3)spawnBounds.center, new Vector3(spawnBounds.width, spawnBounds.height));
    }
}
