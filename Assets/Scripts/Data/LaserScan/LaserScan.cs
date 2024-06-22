using System;
using Unity.Collections;
using Unity.Mathematics;

namespace UnitySensors.Data.LaserScan
{
    public struct LaserScan : IDisposable
    {
        public NativeArray<Range> ranges;

        public void Dispose()
        {
            ranges.Dispose();
        }
    }
}