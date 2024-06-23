// using System;

using UnityEngine;

using Unity.Mathematics;
using Unity.Collections;

namespace UnitySensors.Sensor.ToF
{
    public abstract class ToFSensor : UnitySensor
    {
        [SerializeField]
        private float _minRange = 0.5f;
        [SerializeField]
        private float _maxRange = 100.0f;
        [SerializeField]
        private float _gaussianNoiseSigma = 0.0f;

        private float _range;

        public float minRange { get => _minRange; }
        public float maxRange { get => _maxRange; }

        public float range { get => _range; set => _range = value; }

        protected float gaussianNoiseSigma { get => _gaussianNoiseSigma; }        
        

        protected override void Init()
        {
            _range = 0.0f;
        }

        protected override void OnSensorDestroy()
        {

        }
    }
}
