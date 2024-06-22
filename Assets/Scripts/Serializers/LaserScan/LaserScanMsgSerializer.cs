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
    public class LaserScanMsgSerializer<T, TT> : RosMsgSerializer<T, LaserScanMsg>
        where T : UnitySensor, ILaserScanInterface
        where TT : struct, IRangeInterface
    {
        [SerializeField]
        private HeaderSerializer _header;

        private JobHandle _jobHandle;
        private IRangesToLaserScanMsgJob<TT> _rangesToLaserScanMsgJob;
        private NativeArray<byte> _data;

        public override void Init(T sensor)
        {
            base.Init(sensor);
            _header.Init(sensor);

            // _msg.height = 1;
            // _msg.width = (uint)sensor.pointsNum;
            // _msg.fields = new PointFieldMsg[3];
            // for (int i = 0; i < 3; i++)
            // {
            //     _msg.fields[i] = new PointFieldMsg();
            //     _msg.fields[i].name = ((char)('x' + i)).ToString();
            //     _msg.fields[i].offset = (uint)(4 * i);
            //     _msg.fields[i].datatype = 7;
            //     _msg.fields[i].count = 1;
            // }
            // _msg.is_bigendian = false;
            // _msg.point_step = 12;
            // _msg.row_step = (uint)sensor.pointsNum * 12;
            // _msg.data = new byte[(uint)sensor.pointsNum * 12];
            // _msg.is_dense = true;

            // _data = new NativeArray<byte>(sensor.pointsNum * 12, Allocator.Persistent);

            // _pointsToPointCloud2MsgJob = new IPointsToPointCloud2MsgJob<TT>()
            // {
            //     points = sensor.pointCloud.points,
            //     data = _data
            // };
        }

        public override LaserScanMsg Serialize()
        {
            _msg.header = _header.Serialize();
            // _jobHandle = _pointsToPointCloud2MsgJob.Schedule(sensor.pointsNum, 1);
            // _jobHandle.Complete();
            // _pointsToPointCloud2MsgJob.data.CopyTo(_msg.data);
            return _msg;
        }

        public void Dispose()
        {
            _jobHandle.Complete();
            if(_data.IsCreated) _data.Dispose();
        }
    }
}