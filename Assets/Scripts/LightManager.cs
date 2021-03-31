using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

public class LightManager : MonoBehaviour
{
    public static LightManager instance;
    public List<LightDrop> lights = new List<LightDrop>();
    public List<Campfire> campfires = new List<Campfire>();
    private Queue<Vector2> spawnPositions = new Queue<Vector2>(new Vector2[100]);
    private float OriginalRefreshTimer = 10;
    private float spawnPositionRefreshTimer;

    private void Awake()
    {
        if (instance != null) Destroy(instance);
        instance = this;
    }

    private void Start()
    {
        RefreshSpawnQueue();

        spawnPositionRefreshTimer = 10;
    }

    #region decay job
    private void Update()
    {
        DecayLights();
        spawnPositionRefreshTimer -= Time.deltaTime;
    }

    private void LateUpdate()
    {

        decayLightsHandle.Complete();

        ApplyLightDecay();
        lightDropStructs.Dispose();

    }

    private void ApplyLightDecay()
    {
        for (int i = 0; i < lightDropStructs.Length && i < lights.Count; i++)
        {
            if (lightDropStructs[i].canDecay)
            {
                if (lightDropStructs[i].remainingTime < .01f)
                    Destroy(lights[i].gameObject);
                else
                    lights[i].fromLightDropStruct(lightDropStructs[i]);
            }
        }
    }

    private NativeArray<LightDropStruct> lightDropStructs;
    private JobHandle decayLightsHandle;

    private void DecayLights()
    {
        lightDropStructs = new NativeArray<LightDropStruct>(lights.Count, Allocator.TempJob);
        for (int i = 0; i < lights.Count; i++)
        {
            lightDropStructs[i] = lights[i].toLightDropStruct();
        }
        decayLightsHandle = new DecayJob { lightDrops = lightDropStructs, deltaTime = Time.deltaTime }.Schedule(lightDropStructs.Length, 1);
    }

    private struct DecayJob : IJobParallelFor
    {
        public NativeArray<LightDropStruct> lightDrops;
        public float deltaTime;

        public void Execute(int index)
        {
            lightDrops[index].Decay(deltaTime);
            if (lightDrops[index].canDecay)
            {
                float remainingTime = math.max(0, lightDrops[index].remainingTime - deltaTime * lightDrops[index].decaySpeed);
                float remaining = lightDrops[index].remainingTime / lightDrops[index].decayTime;
                lightDrops[index] = new LightDropStruct
                {
                    canDecay = lightDrops[index].canDecay,
                    remainingTime = math.max(0, lightDrops[index].remainingTime - deltaTime * lightDrops[index].decaySpeed),
                    pointLightOuterRadius = lightDrops[index].startingLightRadius * remaining,
                    colliderRadius = lightDrops[index].startingColliderRadius * remaining,
                };
            }
        }
    }

    #endregion
    public void add(LightDrop light)
    {
        if (light is Campfire campfire)
            campfires.Add(campfire);
        lights.Add(light);
        spawnPositionRefreshTimer = 0;
    }

    public void remove(LightDrop light)
    {
        if (light is Campfire campfire)
            campfires.Remove(campfire);
        lights.Remove(light);
        spawnPositionRefreshTimer = 0;
    }

    public Vector2 FindValidSpawnPosition()
    {
        
        if (spawnPositionRefreshTimer < .01f)
        {
            RefreshSpawnQueue();
            spawnPositionRefreshTimer = OriginalRefreshTimer;
        }
        Vector2 position;
        for (int i = 0; i < spawnPositions.Count; i++)
        {
            position = spawnPositions.Dequeue();
            spawnPositions.Enqueue(position);
            if (!Player.instance.IsPositionInFOV(position))
                return position;
        }
        RefreshSpawnQueue();

        return FindValidSpawnPosition();
    }

    private void RefreshSpawnQueue()
    {
        NativeArray<Vector2> lightPos = new NativeArray<Vector2>(lights.Count, Allocator.TempJob);
        NativeArray<float> lightRadii = new NativeArray<float>(lights.Count, Allocator.TempJob);
        NativeArray<Vector2> currentSpawnPositions = new NativeArray<Vector2>(spawnPositions.ToArray(), Allocator.TempJob);
        for (int i = 0; i < lights.Count; i++)
        {
            lightPos[i] = lights[i].transform.position;
            lightRadii[i] = lights[i].light.pointLightOuterRadius;
        }
        FindSpawnPositionsJob findSpawnPositionsJob = new FindSpawnPositionsJob
        {
            lightPositions = lightPos,
            lightRadiis = lightRadii,
            spawnPositions = currentSpawnPositions,
            random = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(1, 100000))
        };
        findSpawnPositionsJob.Schedule().Complete();
        spawnPositions = new Queue<Vector2>(findSpawnPositionsJob.spawnPositions);
        lightPos.Dispose();
        lightRadii.Dispose();
        currentSpawnPositions.Dispose();
    }

    private struct FindSpawnPositionsJob : IJob
    {
        [ReadOnly]
        public NativeArray<Vector2> lightPositions;
        [ReadOnly]
        public NativeArray<float> lightRadiis;

        public NativeArray<Vector2> spawnPositions;
        public Unity.Mathematics.Random random;

        public void Execute()
        {
            for (int i = 0; i < spawnPositions.Length; i++)
            {
                if (!ValidatePosition(spawnPositions[i]))
                    spawnPositions[i] = getNewPosition();
            }
        }

        private Vector2 getNewPosition()
        {
            Vector2 newPos;
            do
            {
                float innerRadius = lightRadiis[0];
                float ratio = innerRadius / 100;
                float radius = Mathf.Sqrt(random.NextFloat(ratio * ratio, 1f)) * 100;
                newPos = random.NextFloat2Direction() * radius;
            } while (!ValidatePosition(newPos));
            return newPos;
        }

        private bool ValidatePosition(Vector2 position)
        {
            for (int i = 0; i < lightPositions.Length; i++)
            {
                if (Vector2.Distance(position, lightPositions[i]) < lightRadiis[i])
                    return false;
            }
            for (int i = 0; i < spawnPositions.Length; i++)
            {
                if (spawnPositions[i].Equals(position))
                    continue;
                else if (Vector2.Distance(position, spawnPositions[i]) < 10)
                    return false;
            }
            return true;
        }
    }
}
