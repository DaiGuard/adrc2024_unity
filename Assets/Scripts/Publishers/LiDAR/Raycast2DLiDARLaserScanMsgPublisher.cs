using UnityEngine;
using UnitySensors.Data.PointCloud;
using UnitySensors.Sensor.LiDAR;
using UnitySensors.ROS.Publisher.LaserScan;
using UnitySensors.Data.LaserScan;

namespace UnitySensors.ROS.Publisher.LiDAR
{
    [RequireComponent(typeof(Raycast2DLiDARSensor))]
    public class Raycast2DLiDARLaserScanMsgPublisher : LaserScanMsgPublisher<Raycast2DLiDARSensor>
    {
    }
}
