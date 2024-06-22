using System;
using UnityEngine;

using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

using UnitySensors.Utils.Noise;
using UnitySensors.Data.LaserScan;

using Random = Unity.Mathematics.Random;

namespace UnitySensors.Sensor.LiDAR
{
    public class Raycast2DLiDARSensor: LiDAR2DSensor
    {
        private Transform _transform;
        private JobHandle _jobHandle;
        private IUpdateRaycastCommandsJob _updateRaycastCommandsJob;
        private IUpdateGaussianNoisesJob _updateGaussianNoisesJob;
        private IRaycastHitsToRangesJob _raycastHitsToRangesJob;
        private NativeArray<float3> _directions;
        private NativeArray<RaycastCommand> _raycastCommands;
        private NativeArray<RaycastHit> _raycastHits;
        private NativeArray<float> _noises;

        protected override void Init()
        {
            base.Init();

            _transform = this.transform;

            LoadScanData();
            SetupJobs();
        }

        private void LoadScanData()
        {
            _directions = new NativeArray<float3>(pointsNum, Allocator.Persistent);

            float angle = Mathf.Deg2Rad * minAngle;
            float step = Mathf.Deg2Rad * (maxAngle - minAngle) / pointsNum;

            for(int i = 0; i < pointsNum; i++)
            {
                float3 dir;

                angle += step;

                dir.x = Mathf.Cos(angle);
                dir.y = Mathf.Sin(angle);
                dir.z = 0.0f;

                _directions[i] = dir;
            }
        }

        private void SetupJobs()
        {
            _raycastCommands = new NativeArray<RaycastCommand>(pointsNum, Allocator.Persistent);
            _raycastHits = new NativeArray<RaycastHit>(pointsNum, Allocator.Persistent);
            _noises = new NativeArray<float>(pointsNum, Allocator.Persistent);

            _updateRaycastCommandsJob = new IUpdateRaycastCommandsJob()
            {
                origin = _transform.position,
                localToWorldMatrix = _transform.localToWorldMatrix,
                maxRange = maxRange,
                directions = _directions,
                indexOffset = 0,
                raycastCommands = _raycastCommands,
            };

            _updateGaussianNoisesJob = new IUpdateGaussianNoisesJob()
            {
                sigma = gaussianNoiseSigma,
                random = new Random((uint)Environment.TickCount),
                noises = _noises,
            };

            _raycastHitsToRangesJob = new IRaycastHitsToRangesJob()
            {
                minRange = minRange,
                minRange_sqr = minRange * minRange,
                maxRange = maxRange,
                maxIntensity = maxIntensity,
                directions = _directions,
                indexOffset = 0,
                raycastHits = _raycastHits,
                noises = _noises,
                ranges = laserScan.ranges,
            };
        }

        protected override void UpdateSensor()
        {
            _updateRaycastCommandsJob.origin = _transform.position;
            _updateRaycastCommandsJob.localToWorldMatrix = _transform.localToWorldMatrix;

            JobHandle updateRaycastCommandsJobHandle = _updateRaycastCommandsJob.Schedule(pointsNum, 1);
            JobHandle updateGaussianNoisesJobHandle = _updateGaussianNoisesJob.Schedule(pointsNum, 1, updateRaycastCommandsJobHandle);
            JobHandle raycastJobHandle = RaycastCommand.ScheduleBatch(_raycastCommands, _raycastHits, pointsNum, updateGaussianNoisesJobHandle);
            _jobHandle = _raycastHitsToRangesJob.Schedule(pointsNum, 1, raycastJobHandle);

            JobHandle.ScheduleBatchedJobs();
            _jobHandle.Complete();

            _updateRaycastCommandsJob.indexOffset = (_updateRaycastCommandsJob.indexOffset + pointsNum) % pointsNum;
            _raycastHitsToRangesJob.indexOffset = (_raycastHitsToRangesJob.indexOffset + pointsNum) % pointsNum;

            if (onSensorUpdated != null)
                onSensorUpdated.Invoke();
        }

        protected override void OnSensorDestroy()
        {
            _jobHandle.Complete();
            _noises.Dispose();
            _directions.Dispose();
            _raycastCommands.Dispose();
            _raycastHits.Dispose();
            base.OnSensorDestroy();
        }
    }
}