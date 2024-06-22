using UnityEngine;

using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

using UnitySensors.Data.LaserScan;

namespace UnitySensors.Sensor.LiDAR
{
    [BurstCompile]
    public struct IRaycastHitsToRangesJob : IJobParallelFor
    {
        [ReadOnly]
        public float minRange;
        [ReadOnly]
        public float minRange_sqr;
        [ReadOnly]
        public float maxRange;
        [ReadOnly]
        public float maxIntensity;
        [ReadOnly, NativeDisableParallelForRestriction]
        public NativeArray<float3> directions;
        [ReadOnly]
        public int indexOffset;
        [ReadOnly]
        public NativeArray<RaycastHit> raycastHits;
        [ReadOnly]
        public NativeArray<float> noises;

        public NativeArray<Range> ranges;

        public void Execute(int index)
        {
            float distance = raycastHits[index].distance;
            float distance_noised = distance + noises[index];
            distance = (minRange < distance && distance < maxRange && minRange < distance_noised && distance_noised < maxRange) ? distance_noised : 0;
            Range range = new Range()
            {
                range = distance,
                // position = directions[index + indexOffset] * distance,
                intensity = (distance != 0) ? maxIntensity * minRange_sqr / (distance * distance) : 0
            };
            // PointXYZI point = new PointXYZI()
            // {
            //     position = directions[index + indexOffset] * distance,
            //     intensity = (distance != 0) ? maxIntensity * minRange_sqr / (distance * distance) : 0
            // };
            ranges[index] = range;
        }
    }
}
