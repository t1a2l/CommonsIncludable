using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ColossalFramework;
using TransportLinesManager.Utils;
using UnityEngine;

namespace Commons.Utils
{
    public class VehicleUtils
    {
        #region Vehicle Utils
        public static VehicleInfo GetRandomModel(IEnumerable<string> assetList, out string selectedModel)
        {
            selectedModel = null;

            var models = assetList?.Where(x => !string.IsNullOrEmpty(x)).ToList();
            if (models == null || models.Count == 0)
            {
                return null;
            }

            int idx = Singleton<SimulationManager>.instance.m_randomizer.Int32(0, models.Count - 1);

            LogUtils.DoLog("GetRandomModel count={0}, idx={1}, models=[{2}]", models.Count, idx, string.Join(" | ", [.. models]));

            if (idx < 0 || idx >= models.Count)
            {
                LogUtils.DoErrorLog($"GetRandomModel invalid index {idx} for count {models.Count}");
                return null;
            }

            selectedModel = models[idx];

            VehicleInfo result = PrefabCollection<VehicleInfo>.FindLoaded(selectedModel ?? "");
            if (result == null)
            {
                LogUtils.DoLog("MODEL DOESN'T EXIST!");
                return null;
            }
            return result;
        }
        public static int GetCapacity(VehicleInfo info) => GetCapacity(info, info.m_vehicleAI);
        private static readonly Dictionary<Type, FieldInfo> m_cachedCapacityFieldForAiType = [];
        private static readonly Dictionary<Type, FieldInfo> m_cachedTransportInfoFieldsForAiType = [];
        public static int GetCapacity<AI>(VehicleInfo info, AI ai, bool noLoop = false) where AI : VehicleAI
        {
            if (info == null)
            {
                return -1;
            }
            System.Reflection.FieldInfo fieldInfo;
            fieldInfo = GetVehicleCapacityField(ai);
            if (fieldInfo != null)
            {
                int capacity = ReflectionUtils.GetGetFieldDelegate<AI, int>(fieldInfo)(ai);
                try
                {
                    if (!noLoop && info.m_trailers is not null)
                    {
                        foreach (VehicleInfo.VehicleTrailer trailer in info.m_trailers)
                        {
                            capacity += trailer.m_info == null ? 0 : GetCapacity(trailer.m_info, trailer.m_info.m_vehicleAI, true);
                        }
                    }
                }
                catch (Exception e)
                {
                    LogUtils.DoLog($"ERRO AO OBTER CAPACIDADE: [{info}] {e} {e.Message}\n{e.StackTrace}");
                }
                return capacity;
            }
            else
            {
                LogUtils.DoLog($"AI \"{ai.GetType()}\" in asset {info} has no passenger Field!");
                return 0;
            }
        }

        public static FieldInfo GetVehicleCapacityField<AI>(AI ai) where AI : VehicleAI
        {
            var result = m_cachedCapacityFieldForAiType.TryGetValue(ai.GetType(), out FieldInfo fieldInfo);
            if(result && fieldInfo != null)
			{
                return fieldInfo;
			} 
            else
            {
                Type typeTry = ai.GetType();
                do
                {
                    fieldInfo = typeTry.GetField("m_passengerCapacity", BindingFlags.Instance | BindingFlags.Public);
                    typeTry = typeTry.BaseType;
                } while (typeTry != typeof(VehicleAI) && fieldInfo == null);
                m_cachedCapacityFieldForAiType[ai.GetType()] = fieldInfo;
            }
            return fieldInfo;
        }

        public static FieldInfo GetTransportInfoField<AI>(AI ai) where AI : VehicleAI
        {
            var result = m_cachedTransportInfoFieldsForAiType.TryGetValue(ai.GetType(), out FieldInfo fieldInfo);
            if(result && fieldInfo != null)
			{
                return fieldInfo;
			} 
            else
            {
                Type typeTry = ai.GetType();
                do
                {
                    fieldInfo = typeTry.GetField("m_transportInfo", BindingFlags.Instance | BindingFlags.Public);
                    typeTry = typeTry.BaseType;
                } while (typeTry != typeof(VehicleAI) && fieldInfo == null);
                m_cachedTransportInfoFieldsForAiType[ai.GetType()] = fieldInfo;
            }
            return fieldInfo;
        }

        public static Dictionary<string, float> GetCapacityRelative(VehicleInfo info)
        {
            var relativeParts = new Dictionary<string, float>();
            GetCapacityRelative(info, info.m_vehicleAI, ref relativeParts, out _);
            return relativeParts;
        }

        private static void GetCapacityRelative<AI>(VehicleInfo info, AI ai, ref Dictionary<string, float> relativeParts, out int totalCapacity, bool noLoop = false) where AI : VehicleAI
        {
            if (info == null)
            {
                totalCapacity = 0;
                return;
            }

            totalCapacity = ReflectionUtils.GetGetFieldDelegate<AI, int>(GetVehicleCapacityField(ai))(ai);
            relativeParts[info.name] = totalCapacity;
            if (!noLoop)
            {
                try
                {
                    if (info.m_trailers is not null)
                    {
                        foreach (VehicleInfo.VehicleTrailer trailer in info.m_trailers)
                        {
                            if (trailer.m_info != null)
                            {
                                GetCapacityRelative(trailer.m_info, trailer.m_info.m_vehicleAI, ref relativeParts, out int capacity, true);
                                totalCapacity += capacity;
                            }
                        }
                    }
                    for (int i = 0; i < relativeParts.Keys.Count; i++)
                    {
                        relativeParts[relativeParts.Keys.ElementAt(i)] /= totalCapacity;
                    }
                }
                catch (Exception e)
                {
                    LogUtils.DoLog($"ERRO AO OBTER CAPACIDADE REL: [{info}] {e} {e.Message}\n{e.StackTrace}");
                }
            }
        }

        public static bool IsTrailer(VehicleInfo prefab) 
        { 
            if(prefab.GetAI() is CarTrailerAI || prefab.GetAI() is TrolleybusTrailerAI)
            {
                return true;
            }
            return prefab.m_placementStyle != ItemClass.Placement.Automatic;
        }   
 
        public static void ReplaceVehicleModel(ushort idx, VehicleInfo newInfo)
        {
            VehicleManager instance = VehicleManager.instance;
            CitizenManager.instance.ReleaseUnits(instance.m_vehicles.m_buffer[idx].m_citizenUnits);
            instance.m_vehicles.m_buffer[idx].Unspawn(idx);
            instance.m_vehicles.m_buffer[idx].Info = newInfo ?? throw new ArgumentNullException("newInfo cannot be null!");
            instance.m_vehicles.m_buffer[idx].Spawn(idx);
            newInfo.m_vehicleAI.CreateVehicle(idx, ref instance.m_vehicles.m_buffer[idx]);
        }

        private static int GetUnitsCapacity(VehicleAI vehicleAI)
        {
            VehicleAI vehicleAI2 = vehicleAI as AmbulanceAI;
            if (vehicleAI2 != null)
            {
                return ((AmbulanceAI)vehicleAI2).m_patientCapacity + ((AmbulanceAI)vehicleAI2).m_paramedicCount;
            }
            vehicleAI2 = (vehicleAI as BusAI);
            if (vehicleAI2 != null)
            {
                return ((BusAI)vehicleAI2).m_passengerCapacity;
            }
            vehicleAI2 = (vehicleAI as HearseAI);
            if (vehicleAI2 != null)
            {
                return ((HearseAI)vehicleAI2).m_corpseCapacity + ((HearseAI)vehicleAI2).m_driverCount;
            }
            vehicleAI2 = (vehicleAI as PassengerPlaneAI);
            if (vehicleAI2 != null)
            {
                return ((PassengerPlaneAI)vehicleAI2).m_passengerCapacity;
            }
            vehicleAI2 = (vehicleAI as PassengerShipAI);
            if (vehicleAI2 != null)
            {
                return ((PassengerShipAI)vehicleAI2).m_passengerCapacity;
            }
            vehicleAI2 = (vehicleAI as PassengerTrainAI);
            if (vehicleAI2 != null)
            {
                return ((PassengerTrainAI)vehicleAI2).m_passengerCapacity;
            }
            vehicleAI2 = (vehicleAI as TramAI);
            if (vehicleAI2 != null)
            {
                return ((TramAI)vehicleAI2).m_passengerCapacity;
            }
            vehicleAI2 = (vehicleAI as CableCarAI);
            if (vehicleAI2 != null)
            {
                return ((CableCarAI)vehicleAI2).m_passengerCapacity;
            }
            vehicleAI2 = (vehicleAI as PassengerFerryAI);
            if (vehicleAI2 != null)
            {
                return ((PassengerFerryAI)vehicleAI2).m_passengerCapacity;
            }
            vehicleAI2 = (vehicleAI as PassengerBlimpAI);
            if (vehicleAI2 != null)
            {
                return ((PassengerBlimpAI)vehicleAI2).m_passengerCapacity;
            }
            return -1;
        }

        private static int GetTotalUnitGroups(uint unitID)
        {
            int num = 0;
            while (unitID != 0u)
            {
                CitizenUnit citizenUnit = Singleton<CitizenManager>.instance.m_units.m_buffer[(int)((UIntPtr)unitID)];
                unitID = citizenUnit.m_nextUnit;
                num++;
            }
            return num;
        }

        public static IEnumerator UpdateCapacityUnits()
        {
            int count = 0;
            Array16<Vehicle> vehicles = Singleton<VehicleManager>.instance.m_vehicles;
            int i = 0;
            while (i < (long)((ulong)vehicles.m_size))
            {
                if ((vehicles.m_buffer[i].m_flags & Vehicle.Flags.Spawned) == Vehicle.Flags.Spawned)
                {
                    int capacity = GetUnitsCapacity(vehicles.m_buffer[i].Info.m_vehicleAI);
                    if (capacity != -1)
                    {
                        CitizenUnit[] units = Singleton<CitizenManager>.instance.m_units.m_buffer;
                        uint unit = vehicles.m_buffer[i].m_citizenUnits;
                        int currentUnitCount = GetTotalUnitGroups(unit);
                        int newUnitCount = Mathf.CeilToInt(capacity / 5f);
                        if (newUnitCount < currentUnitCount)
                        {
                            uint j = unit;
                            for (int k = 1; k < newUnitCount; k++)
                            {
                                j = units[(int)((UIntPtr)j)].m_nextUnit;
                            }
                            Singleton<CitizenManager>.instance.ReleaseUnits(units[(int)((UIntPtr)j)].m_nextUnit);
                            units[(int)((UIntPtr)j)].m_nextUnit = 0u;
                            count++;
                        }
                        else if (newUnitCount > currentUnitCount)
                        {
                            uint l = unit;
                            while (units[(int)((UIntPtr)l)].m_nextUnit != 0u)
                            {
                                l = units[(int)((UIntPtr)l)].m_nextUnit;
                            }
                            int newCapacity = capacity - currentUnitCount * 5;
                            Singleton<CitizenManager>.instance.CreateUnits(out units[(int)((UIntPtr)l)].m_nextUnit, ref Singleton<SimulationManager>.instance.m_randomizer, 0, (ushort)i, 0, 0, 0, newCapacity, 0);
                            count++;
                        }
                    }
                }
                if (i % 256 == 255)
                {
                    yield return null;
                }
                i++;
            }
            yield break;
        }

        public static VehicleInfo GetModelByPercentageOrCount(List<TransportAsset> assetList, ushort lineId, out string modelName)
        {
            modelName = null;
            if (assetList == null || assetList.Count == 0)
            {
                return null;
            }

            var budgetData = TLMLineUtils.GetEffectiveConfigForLine(lineId).BudgetEntries.GetAtHourExact(TLMLineUtils.ReferenceTimer);
            int index = budgetData.Second; // budgetData.Second = current active slot index
            if (index < 0) index = 0;

            var ext = TransportLinesManager.Data.DataContainers.TLMTransportLineExtension.Instance;

            bool isAbsolute = ext.IsUsingCustomConfig(lineId) && ext.IsDisplayAbsoluteValues(lineId);

            if (isAbsolute)
            {
                // COUNT MODE: pick any asset where usedCount < totalCount
                var candidates = assetList.Where(a => a.count.ContainsKey(index.ToString()) && a.count[index.ToString()].TotalCount > a.count[index.ToString()].UsedCount).ToList();

                if (candidates.Count == 0)
                {
                    return null;
                }

                // Pick randomly among eligible candidates
                var chosen = candidates[SimulationManager.instance.m_randomizer.Int32(0, candidates.Count - 1)];
                modelName = chosen.name;
            }
            else
            {
                // PERCENT MODE: weighted random draw by spawn_percent
                var eligible = assetList.Where(a => a.spawn_percent.ContainsKey(index.ToString()) && a.spawn_percent[index.ToString()].Value > 0).ToList();

                if (eligible.Count == 0)
                {
                    return null;
                }

                int totalWeight = eligible.Sum(a => a.spawn_percent[index.ToString()].Value);
                if (totalWeight <= 0)
                {
                    return null;
                }

                int roll = SimulationManager.instance.m_randomizer.Int32(0, totalWeight - 1);
                int cumulative = 0;
                modelName = null;
                foreach (var asset in eligible)
                {
                    cumulative += asset.spawn_percent[index.ToString()].Value;
                    if (roll < cumulative)
                    {
                        modelName = asset.name;
                        break;
                    }
                }

                if (modelName == null)
                {
                    modelName = eligible[eligible.Count - 1].name;
                }
            }

            VehicleInfo info = PrefabCollection<VehicleInfo>.FindLoaded(modelName);
            if (info == null)
            {
                LogUtils.DoLog($"GetModelByPercentageOrCount: model '{modelName}' not found in PrefabCollection!");
            }
            return info;
        }

        #endregion
    }

}