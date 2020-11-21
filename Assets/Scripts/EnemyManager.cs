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
    void Start()
    {
        activeEnemies = new List<Enemy>();
        activeEnemies.AddRange(GetComponentsInChildren<Enemy>());
    }

    // Update is called once per frame
    void Update()
    {
        TransformAccessArray transformAccessArray = new TransformAccessArray(activeEnemies.Count, 100);
        NativeArray<NativeArray<Vector2>> relativePositions = new NativeArray<NativeArray<Vector2>>(activeEnemies.Count, Allocator.TempJob);
        for (int i = 0; i < activeEnemies.Count; i++)
        {
            transformAccessArray[i] = activeEnemies[i].transform;
            NativeArray<Vector2> influencingLights = new NativeArray<Vector2>(activeEnemies[i].influencingLights.Count, Allocator.TempJob);
            for(int j  = 0; j < activeEnemies[i].influencingLights.Count; j++)
            {
                influencingLights[j] = activeEnemies[i].influencingLights[j].transform.position;
            }
            relativePositions[i] = influencingLights;
        }
        NativeArray<Vector2> targets = new NativeArray<Vector2>(activeEnemies.Count, Allocator.TempJob);
        FindTargetsJob findTargets = new FindTargetsJob
        {
            relativeTargets = relativePositions,
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
        transformAccessArray.Dispose();
        targets.Dispose();
        foreach (NativeArray<Vector2> ts in relativePositions)
        {
            ts.Dispose();
        }
        relativePositions.Dispose();
    }
    struct MoveEnemiesJob : IJobParallelForTransform
    {
        public float deltaTime;
        public NativeArray<Vector2> targetPositions;
        public void Execute(int index, TransformAccess transform)
        {
            transform.position = Vector2.Lerp(transform.position, targetPositions[index], deltaTime);
        }
    }
    struct FindTargetsJob : IJobParallelForTransform
    {
        [ReadOnly]
        public NativeArray<NativeArray<Vector2>> relativeTargets;
        public NativeArray<Vector2> results;
        public Unity.Mathematics.Random random;
        public void Execute(int index, TransformAccess transform)
        {
            Vector2 closest = Vector2.zero;
            float lastClosest = float.MaxValue;
            if (relativeTargets.Length == 0)
            {
                results[index] = random.NextFloat2Direction() * 10f;
            }
            else
            {
                foreach (Vector2 target in relativeTargets[index])
                {
                    float distance = Vector2.Distance(transform.position, target);
                    if (distance < lastClosest)
                    {
                        closest = target;
                        lastClosest = distance;
                    }
                }
                results[index] = closest;
            }
        }
    }
}
