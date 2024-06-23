using UnityEngine;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;
using RosMessageTypes.Sensor;
using UnitySensors.Data.LaserScan;
using UnitySensors.Sensor;
using UnitySensors.ROS.Serializer.LaserScan;

namespace UnitySensors.ROS.Publisher.LaserScan
{
    public class LaserScanMsgPublisher<T> : RosMsgPublisher<T, LaserScanMsgSerializer<T>, LaserScanMsg>
        where T : UnitySensor, ILaserScanInterface
    {
        private void OnDestroy()
        {
            _serializer.Dispose();
        }
    }
}
