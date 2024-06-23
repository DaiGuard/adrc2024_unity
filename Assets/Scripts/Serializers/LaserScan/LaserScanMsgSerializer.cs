using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using RosMessageTypes.Sensor;

using UnitySensors.Data.LaserScan;
using UnitySensors.Sensor;

namespace UnitySensors.ROS.Serializer.LaserScan
{
    [System.Serializable]
    public class LaserScanMsgSerializer<T> : RosMsgSerializer<T, LaserScanMsg>
        where T : UnitySensor, ILaserScanInterface
    {
        [SerializeField]
        private HeaderSerializer _header;

        private JobHandle _jobHandle;
        // private IRangesToLaserScanMsgJob _rangesToLaserScanMsgJob;
        private NativeArray<byte> _data;

        public override void Init(T sensor)
        {
            base.Init(sensor);
            _header.Init(sensor);

            _msg.angle_min = sensor.minAngle * Mathf.Deg2Rad;
            _msg.angle_max = sensor.maxAngle * Mathf.Deg2Rad;
            _msg.angle_increment = (_msg.angle_max - _msg.angle_min) / sensor.pointsNum;
            _msg.time_increment = sensor.dt / sensor.pointsNum;
            _msg.scan_time = sensor.dt;
            _msg.range_min = sensor.minRange;
            _msg.range_max = sensor.maxRange;
            _msg.ranges = new float[sensor.pointsNum];
            _msg.intensities = new float[sensor.pointsNum];

            // _data = new NativeArray<byte>(sensor.pointsNum * 4, Allocator.Persistent);

            // _rangesToLaserScanMsgJob = new IRangesToLaserScanMsgJob()
            // {
            //     ranges = sensor.laserScan.ranges,
            //     data = _data,
            // };
        }

        public override LaserScanMsg Serialize()
        {
            _msg.header = _header.Serialize();
            // _jobHandle = _rangesToLaserScanMsgJob.Schedule(sensor.pointsNum, 1);
            // _jobHandle.Complete();

            for(int i=0; i<sensor.pointsNum; i++)
            {
                _msg.ranges[i] = sensor.laserScan.ranges[i].range;
                _msg.intensities[i] = sensor.laserScan.ranges[i].intensity;
            }
            
            return _msg;
        }

        public void Dispose()
        {
            _jobHandle.Complete();
            if(_data.IsCreated) _data.Dispose();
        }
    }
}