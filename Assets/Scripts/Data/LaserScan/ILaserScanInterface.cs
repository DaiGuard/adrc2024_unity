using Unity.Collections;

namespace UnitySensors.Data.LaserScan
{
    public interface ILaserScanInterface
    {
        public LaserScan laserScan { get; }
        public float minAngle {get; }
        public float maxAngle {get; }
        public int pointsNum { get; }
    }
}
