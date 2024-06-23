using System;
using Unity.Collections;
using Unity.Mathematics;

namespace UnitySensors.Data.LaserScan
{
    public struct Range : IRangeInterface
    {
        private float _range;
        public float intensity;
        public float range { get => _range; set => _range = value;}

        public void CopyTo(NativeArray<byte> dst)
        {
            dst.GetSubArray(0, 4).CopyFrom(BitConverter.GetBytes(_range));
            dst.GetSubArray(4, 4).CopyFrom(BitConverter.GetBytes(intensity));
        }
    }
}