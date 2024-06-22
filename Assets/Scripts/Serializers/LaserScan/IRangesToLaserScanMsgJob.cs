using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

using UnitySensors.Data.LaserScan;

namespace UnitySensors.ROS.Serializer.LaserScan
{
    [BurstCompile]
    public struct IRangesToLaserScanMsgJob<T> : IJobParallelFor
        where T : struct, IRangeInterface
    {
        [ReadOnly]
        public NativeArray<T> points;

        public NativeArray<byte> data;

        public void Execute(int index)
        {
            NativeArray<float> tmp = new NativeArray<float>(3, Allocator.Temp);
            // tmp[0] = points[index].position.z;
            // tmp[1] = -points[index].position.x;
            // tmp[2] = points[index].position.y;
            var slice = new NativeSlice<float>(tmp).SliceConvert<byte>();
            slice.CopyTo(data.GetSubArray(index * 12, 12));

        }
    }
}