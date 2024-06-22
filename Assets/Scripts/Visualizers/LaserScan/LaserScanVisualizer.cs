using System;
using System.Collections.Generic;
using UnityEngine;
using UnitySensors.Sensor;
using UnitySensors.Data.LaserScan;

namespace UnitySensors.Visualization.LaserScan
{
    public class LaserScanVisualizer<T> : Visualizer<T>
        where T : UnitySensor, ILaserScanInterface
    {
        private Material _mat;
        private Mesh _mesh;

        private ComputeBuffer _rangesBuffer;
        private ComputeBuffer _argsBuffer;

        private uint[] _args = new uint[5] { 0, 0, 0, 0, 0 };

        private int _cachedPointsCount = -1;

        private int _bufferSize;

        private static readonly Dictionary<Type, int> BUFFER_SIZE_DICTIONARY = new Dictionary<Type, int>()
        {
            { typeof(UnitySensors.Data.LaserScan.Range), 8 },
        };

        private static readonly Dictionary<Type, string> SHADER_NAME_DICTIONARY = new Dictionary<Type, string>()
        {
            { typeof(UnitySensors.Data.LaserScan.Range), "UnitySensors/Range" },
        };

        protected override void Init()
        {
            _bufferSize = BUFFER_SIZE_DICTIONARY[typeof(UnitySensors.Data.LaserScan.Range)];
            _mat = new Material(Shader.Find(SHADER_NAME_DICTIONARY[typeof(UnitySensors.Data.LaserScan.Range)]));
            _mat.renderQueue = 3000;
            _mesh = new Mesh();
            _mesh.vertices = new Vector3[1] { Vector3.zero };
            _mesh.SetIndices(new int[1] { 0 }, MeshTopology.Points, 0);
            _argsBuffer = new ComputeBuffer(1, _args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
            UpdateBuffers();
        }

        protected override void Visualize()
        {
            if (sensor.pointsNum != _cachedPointsCount) UpdateBuffers();

            float step = (sensor.maxAngle - sensor.minAngle) / sensor.pointsNum;

            _mat.SetMatrix("LocalToWorldMatrix", sensor.transform.localToWorldMatrix);
            _mat.SetFloat("startAngle", sensor.minAngle);
            _mat.SetFloat("stepAngle", step);
            _mat.SetFloat("minRange", sensor.minRange);
            _mat.SetFloat("maxRange", sensor.maxRange);
            _rangesBuffer.SetData(sensor.laserScan.ranges);
        }

        private void Update()
        {
            Graphics.DrawMeshInstancedIndirect(_mesh, 0, _mat, new Bounds(Vector3.zero, new Vector3(100.0f, 100.0f, 100.0f)), _argsBuffer);
        }

        private void UpdateBuffers()
        {
            if (_rangesBuffer != null) _rangesBuffer.Release();
            _rangesBuffer = new ComputeBuffer(sensor.pointsNum, _bufferSize);
            _rangesBuffer.SetData(sensor.laserScan.ranges);
            _mat.SetBuffer("RangesBuffer", _rangesBuffer);

            uint numIndices = (_mesh != null) ? (uint)_mesh.GetIndexCount(0) : 0;
            _args[0] = numIndices;
            _args[1] = (uint)sensor.pointsNum;
            _argsBuffer.SetData(_args);

            _cachedPointsCount = sensor.pointsNum;
        }

        private void OnDisable()
        {
            if (_rangesBuffer != null) _rangesBuffer.Release();
            _rangesBuffer = null;
            if (_argsBuffer != null) _argsBuffer.Release();
            _argsBuffer = null;
        }
    }
}