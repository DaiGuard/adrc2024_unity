// using System;

using UnityEngine;

using Unity.Mathematics;
using Unity.Collections;

using UnitySensors.Data.LaserScan;

namespace UnitySensors.Sensor.LiDAR
{
    public abstract class LiDAR2DSensor : UnitySensor, ILaserScanInterface
    {
        // [SerializeField]
        // private ScanPattern _scanPattern;
        [SerializeField]
        private int _pointsNumPerScan = 1;
        [SerializeField]
        private float _minAngle = -180.0f;
        [SerializeField]
        private float _maxAngle = 180.0f;        
        [SerializeField]
        private float _minRange = 0.5f;
        [SerializeField]
        private float _maxRange = 100.0f;
        [SerializeField]
        private float _gaussianNoiseSigma = 0.0f;
        [SerializeField]
        private float _maxIntensity = 255.0f;

        private LaserScan _laserScan;

        // protected ScanPattern scanPattern { get => _scanPattern; }
        public float minAngle { get => _minAngle; }
        public float maxAngle { get => _maxAngle; }
        public float minRange { get => _minRange; }
        public float maxRange { get => _maxRange; }

        protected float gaussianNoiseSigma { get => _gaussianNoiseSigma; }
        protected float maxIntensity { get => _maxIntensity; }
        public LaserScan laserScan { get => _laserScan; }
        public int pointsNum { get => _pointsNumPerScan; }

        protected override void Init()
        {
            _laserScan = new LaserScan()
            {
                ranges = new NativeArray<Range>(_pointsNumPerScan, Allocator.Persistent)
            };
        }

        protected override void OnSensorDestroy()
        {
            _laserScan.Dispose();
        }
    }
}
