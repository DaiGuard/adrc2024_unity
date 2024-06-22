using Unity.Collections;

namespace UnitySensors.Data.LaserScan
{
    public interface ILaserScanInterface
    {
        public LaserScan laserScan { get; }
        public float minAngle {get; }
        public float maxAngle {get; }
        public float minRange {get; }
        public float maxRange {get; }
        public int pointsNum { get; }
    }
}
