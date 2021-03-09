using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;

public class TurretManager : MonoBehaviour
{
    public static TurretManager instance;
    public List<TurretGunController> _Turrets;

    private void Awake()
    {
        if (instance != null) Destroy(instance);
        instance = this;
    }
    [DeallocateOnJobCompletion]
    NativeArray<float2> enemies;
    NativeArray<JobHandle> FindTargetJobHandles;
    FindClosestEnemyJob[] FindTargetJobs;
    [DeallocateOnJobCompletion]
    NativeArray<float2> _TurretTargets;
    [DeallocateOnJobCompletion]
    TransformAccessArray _TurretTransformAccessArray;
    JobHandle _RotateTurretJobHandle;
    private void Update()
    {
        _RotateTurretJobHandle.Complete();
        if (_TurretTargets.IsCreated)
            _TurretTargets.Dispose();
        if (_Turrets.Count == 0)
            return;
        enemies = EnemyManager.instance.GetActiveEnemyPositionsAsNativeArray(Allocator.TempJob);
        FindTargetJobHandles = new NativeArray<JobHandle>(_Turrets.Count, Allocator.TempJob);
        FindTargetJobs = new FindClosestEnemyJob[_Turrets.Count];
        if (_TurretTransformAccessArray.isCreated) _TurretTransformAccessArray.Dispose();
        _TurretTransformAccessArray = new TransformAccessArray(_Turrets.Count, 32);
        for (int i = 0; i < _Turrets.Count; i++)
        {
            //Find turrets target
            FindTargetJobs[i] = new FindClosestEnemyJob
            {
                Position = (Vector2)_Turrets[i].transform.position,
                EnemyPositions = enemies,
                Distance = float.MaxValue,
                Result = new NativeArray<float2>(1, Allocator.TempJob)
            };
            FindTargetJobHandles[i] = FindTargetJobs[i].Schedule(enemies.Length, new JobHandle());

            _TurretTransformAccessArray.Add(_Turrets[i].transform);
        }
    }
    private void LateUpdate()
    {
        if (_Turrets.Count == 0) return;
        _TurretTargets = new NativeArray<float2>(_Turrets.Count, Allocator.TempJob);
        JobHandle.CompleteAll(FindTargetJobHandles);
        enemies.Dispose();
        FindTargetJobHandles.Dispose();
        for (int i = 0; i < _Turrets.Count && i < FindTargetJobs.Length; i++)
        {
            float2 result = FindTargetJobs[i].Result[0];
            _TurretTargets[i] = result;
            _Turrets[i].TargetPos = result;
            FindTargetJobs[i].Result.Dispose();
        }
        _RotateTurretJobHandle = new RotateTowardsTarget
        {
            _TargetPositions = _TurretTargets,
            deltaTime = Time.deltaTime,
        }.Schedule(_TurretTransformAccessArray);
    }
    private struct FindClosestEnemyJob : IJobFor
    {
        [ReadOnly]
        public float2 Position;
        [ReadOnly]
        public NativeArray<float2> EnemyPositions;
        [WriteOnly]
        public NativeArray<float2> Result;

        public float Distance;
        public void Execute(int index)
        {
            float newDistance = math.distance(Position, EnemyPositions[index]);
            if (newDistance < Distance)
            {
                Result[0] = EnemyPositions[index];
                Distance = newDistance;
            }
        }
    }
    private struct RotateTowardsTarget : IJobParallelForTransform
    {
        [ReadOnly]
        public NativeArray<float2> _TargetPositions;
        [ReadOnly]
        public float deltaTime;
        public void Execute(int index, TransformAccess transform)
        {
            float2 worldPos = _TargetPositions[index];
            float2 dir = worldPos - (float2)(Vector2)transform.position;
            float targetAngle = math.degrees(math.atan2(dir.y, dir.x));
            transform.rotation = Quaternion.AngleAxis(targetAngle, Vector3.forward);
        }
    }
    private struct ReduceCooldowns : IJobFor
    {
        public NativeArray<float> CurrentCooldowns;
        [ReadOnly]
        public float deltaTime;

        public void Execute(int index)
        {
            CurrentCooldowns[index] -= deltaTime;
        }
    }
}




