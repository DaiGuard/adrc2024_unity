using System;
using UnityEngine;

using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

using UnitySensors.Utils.Noise;
using UnitySensors.Data.LaserScan;

using Random = Unity.Mathematics.Random;

namespace UnitySensors.Sensor.ToF
{
    public class RaycastToFSensor: ToFSensor
    {
        private Transform _transform;
        private float3 _direction;
        private Random _random;

        protected override void Init()
        {
            base.Init();

            _transform = this.transform;
            _random = new Random((uint)Environment.TickCount);

            LoadScanData();
        }

        private void LoadScanData()
        {
            _direction.x = 1.0f;
            _direction.y = 0.0f;
            _direction.z = 0.0f;
        }

        protected override void UpdateSensor()
        {
            if(Physics.Raycast(_transform.position, _transform.localToWorldMatrix * (Vector3)_direction, out var hit, maxRange))
            {
                var rand2 = _random.NextFloat();
                var rand3 = _random.NextFloat();
                float normrand =
                    (float)Math.Sqrt(-2.0f * Math.Log(rand2)) *
                    (float)Math.Cos(2.0f * Math.PI * rand3);
                float noise = gaussianNoiseSigma * normrand;

                this.range = hit.distance + noise;
            }

            if (onSensorUpdated != null)
                onSensorUpdated.Invoke();
        }

        protected override void OnSensorDestroy()
        {
            base.OnSensorDestroy();
        }
    }
}