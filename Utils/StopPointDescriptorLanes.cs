using ColossalFramework.Math;
using Commons.Utils.StructExtensions;
using UnityEngine;

namespace Commons.Utils
{
    public struct StopPointDescriptorLanes
    {
        public Bezier3 platformLine;
        public float width;
        public VehicleInfo.VehicleType vehicleType;
        public uint laneId;
        public sbyte subBuildingId;
        public Vector3 directionPath;
        public uint platformLaneId;

        public readonly long UniquePlatformId => ((platformLaneId & 0x7FFFFFFF) << 31) | (laneId & 0x7FFFFFFF);

        public override string ToString() => $"{platformLine.Position(0.5f)} (w={width} | {vehicleType} | {subBuildingId} | {laneId} | DIR = {directionPath} ({directionPath.GetAngleXZ()}°))";
    }

}
