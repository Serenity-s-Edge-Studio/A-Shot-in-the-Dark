using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;
using Assets.Scripts.Utility;
using System.Linq;
using Unity.Burst;

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
    NativeArray<float2> _TurretTargets;
    [DeallocateOnJobCompletion]
    TransformAccessArray _TurretTransformAccessArray;
    [DeallocateOnJobCompletion]
    NativeArray<float> _Cooldowns;

    NativeArray<JobHandle> FindTargetJobHandles;
    FindClosestEnemyJob[] FindTargetJobs;
    JobHandle _RotateTurretJobHandle;
    JobHandle _ReduceCooldownsJobHandle;
    private void Update()
    {
        //Clean-up after jobs of the previous frame
        _RotateTurretJobHandle.Complete();
        _ReduceCooldownsJobHandle.Complete();
        
        if (_TurretTargets.IsCreated)
            _TurretTargets.Dispose();

        if (_TurretTransformAccessArray.isCreated) 
            _TurretTransformAccessArray.Dispose();
        
        //Exit early if there are no turrets
        if (_Turrets.Count == 0)
            return;

        //Allocate data for jobs
        FindTargetJobHandles = new NativeArray<JobHandle>(_Turrets.Count, Allocator.TempJob);
        FindTargetJobs = new FindClosestEnemyJob[_Turrets.Count];
        _Cooldowns = new NativeArray<float>(_Turrets.Count, Allocator.TempJob);
        //Schedule find target jobs
        //These involve a lot of distance checks and can be scheduled in parrallel.
        //Also cache cooldowns.
        for (int i = 0; i < _Turrets.Count; i++)
        {
            float2[] targets = GridManager.instance.GetGridObjectsInRadius<Enemy>(_Turrets[i].transform.position, 10f)
                .Select(enemy => new float2(enemy.transform.position.x, enemy.transform.position.y)).ToArray();
            targets = targets.Length > 0 ? targets : new float2[]{ (Vector2)(_Turrets[i].transform.up * 15f) };
            //Save job struct to access result later.
            FindTargetJobs[i] = new FindClosestEnemyJob
            {
                Position = (Vector2)_Turrets[i].transform.position,
                EnemyPositions = new NativeArray<float2>(targets, Allocator.TempJob),
                Distance = float.MaxValue,
                Result = new NativeArray<float2>(1, Allocator.TempJob)
            };
            FindTargetJobHandles[i] = FindTargetJobs[i].Schedule(targets.Length, new JobHandle());
            float cooldown = _Turrets[i].Cooldown;
            if (cooldown < 0.01f)
            {
                if (Vector2.Distance(_Turrets[i].transform.position, _Turrets[i].TargetPos) < 10f)
                {
                    _Turrets[i].ShootTurret();
                    cooldown = _Turrets[i].Cooldown;
                }
            }
            _Cooldowns[i] = cooldown;
        }
        //Schedule reduce cooldowns job
        _ReduceCooldownsJobHandle = new ReduceCooldowns
        {
            CurrentCooldowns = _Cooldowns,
            deltaTime = Time.deltaTime,
        }.Schedule(_Cooldowns.Length, new JobHandle());
    }
    private void LateUpdate()
    {
        //Exit early if there are no turrets
        if (_Turrets.Count == 0) return;
        //Allocate data for jobs
        _TurretTargets = new NativeArray<float2>(_Turrets.Count, Allocator.TempJob);
        _TurretTransformAccessArray = new TransformAccessArray(_Turrets.Count, 32);
        //Ensure all targets are found before continuing to rotate towards the targets.
        JobHandle.CompleteAll(FindTargetJobHandles);
        _ReduceCooldownsJobHandle.Complete();
        //Dispose of data used to find targets
        FindTargetJobHandles.Dispose();
        //Retrieve values from targeting jobs and prepare data for the rotation jobs.
        for (int i = 0; i < _Turrets.Count && i < FindTargetJobs.Length; i++)
        {
            //Cache result
            float2 result = FindTargetJobs[i].Result[0];
            _TurretTargets[i] = result;
            _Turrets[i].TargetPos = result;
            //Result has been retrieved now dispose of the data.
            FindTargetJobs[i].Result.Dispose();
            FindTargetJobs[i].EnemyPositions.Dispose();
            //Store Turret transform
            _TurretTransformAccessArray.Add(_Turrets[i].transform);
            _Turrets[i].Cooldown = _Cooldowns[i];
        }
        //Schedule rotation job.
        _RotateTurretJobHandle = new RotateTowardsTarget
        {
            _TargetPositions = _TurretTargets,
            deltaTime = Time.deltaTime,
        }.Schedule(_TurretTransformAccessArray);
        _Cooldowns.Dispose();
    }
    /// <summary>
    /// This job iterates through all EnemyPositions and runs distance checks to find the closest target.
    /// </summary>
    [BurstCompile]
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
    /// <summary>
    /// This job rotates all turret transforms to face the desired target
    /// TODO: Needs smoothing.
    /// </summary>
    [BurstCompile]
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
            transform.rotation = Quaternion.AngleAxis(targetAngle - 90f, Vector3.forward);
        }
    }
    [BurstCompile]
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




