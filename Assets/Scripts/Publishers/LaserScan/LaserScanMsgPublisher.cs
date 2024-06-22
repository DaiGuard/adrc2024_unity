using UnityEngine;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;
using RosMessageTypes.Sensor;
using UnitySensors.Data.LaserScan;
using UnitySensors.Sensor;
using UnitySensors.ROS.Serializer.LaserScan;

namespace UnitySensors.ROS.Publisher.LaserScan
{
    public class LaserScanMsgPublisher<T, TT> : RosMsgPublisher<T, LaserScanMsgSerializer<T, TT>, LaserScanMsg>
        where T : UnitySensor, ILaserScanInterface
        where TT : struct, IRangeInterface
    {
        private void OnDestroy()
        {
            _serializer.Dispose();
        }
    }
}
