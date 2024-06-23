using Unity.Collections;

namespace UnitySensors.Data.LaserScan
{
    public interface IRangeInterface
    {
        public void CopyTo(NativeArray<byte> dst);
    }
}

