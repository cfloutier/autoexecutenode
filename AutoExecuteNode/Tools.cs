using System;
using System.Collections.Generic;
using BepInEx;
using KSP;
using KSP.Messages;
using KSP.Sim;
using KSP.Sim.impl;
using KSP.Sim.Maneuver;
using KSP.Sim.State;
using KSP.Game;
using KSP.UI.Binding;
using SpaceWarp;
using SpaceWarp.API.Assets;
using SpaceWarp.API.Mods;
using SpaceWarp.API.UI.Appbar;
using UnityEngine;

using System.Reflection;


namespace AutoExecuteNode
{
    public class Tools
    {

        static public string print_vector(Vector3d vec)
        {
            return $"{vec.x:n2} {vec.y:n2} {vec.z:n2}";
        }

        static public string print_duration(double secs)
        {
            if (secs < 0)
            {
                secs = -secs;
                TimeSpan t = TimeSpan.FromSeconds(secs);

                return string.Format("- {0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                    t.Hours,
                    t.Minutes,
                    t.Seconds,
                    t.Milliseconds);
                }
            else
            {
                TimeSpan t = TimeSpan.FromSeconds(secs);

                return string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                    t.Hours,
                    t.Minutes,
                    t.Seconds,
                    t.Milliseconds);
            }
        }

        static public KSP.Game.GameInstance Game()
        {
            return GameManager.Instance.Game;
        }


        public static Vector3d correct_euler(Vector3d euler)
        {
            Vector3d result = euler;
            if (result.x > 180)
            {
                result.x -= 360;
            }
            if (result.y > 180)
            {
                result.y -= 360;
            }
            if (result.z > 180)
            {
                result.z -= 360;
            }

            return result;
        }

        public static double remainingStartTime(ManeuverNodeData node)
        {
            var dt = node.Time - Game().UniverseModel.UniversalTime;
            return dt;
        }

        public static double remainingEndTime(ManeuverNodeData node)
        {
            var dt = node.Time + node.BurnDuration - Game().UniverseModel.UniversalTime;
            return dt;
        }

    }

    public class Reflex
    {

        /// <summary>
        /// Uses reflection to get the field value from an object.
        /// </summary>
        ///
        /// <param name="type">The instance type.</param>
        /// <param name="instance">The instance object.</param>
        /// <param name="fieldName">The field's name which is to be fetched.</param>
        ///
        public static object GetInstanceField(Type type, object instance, string fieldName)
        {
            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                | BindingFlags.Static;
            FieldInfo field = type.GetField(fieldName, bindFlags);
            object value = field.GetValue(instance);
            return value;
        }
    }

    public class SASInfos
    {
        public static VesselAutopilot current_autoPilot()
        {
            return VesselInfos.CurrentVessel().Autopilot;
        }

        public static VesselSAS current_sas()
        {
            return VesselInfos.CurrentVessel().Autopilot.SAS;
        }

        public static double getSasResponsePC()
        {
            if (current_sas() == null)
                return 0;


            var my_obj = Reflex.GetInstanceField(typeof(VesselSAS), current_sas(), "sasResponse");
            return  ((Vector3d) my_obj).magnitude * 100;
        }

        public static Vector3d geSASAngularDelta()
        {
            if (current_sas() == null)
                return Vector3d.zero;

            var my_obj = Reflex.GetInstanceField(typeof(VesselSAS), current_sas(), "angularDelta");
            return Tools.correct_euler(((Vector3d)  my_obj));
        }
    }


    class VesselInfos
    {
        static public VesselComponent CurrentVessel()
        {
            return Tools.Game().ViewController.GetActiveSimVessel();
        }
        static public VesselVehicle CurrentVehicle()
        {
            if (!Tools.Game().ViewController.TryGetActiveVehicle(out var vehicle)) return null;
            return vehicle as VesselVehicle;
        }

        static public CelestialBodyComponent CurrentBody()
        {
            if (CurrentVessel() == null) return null;
            return CurrentVessel().mainBody;
        }

        public static void SetThrottle(float throttle)
        {
            var active_Vehicle = CurrentVehicle();
            if (active_Vehicle == null) return;

            var update = new FlightCtrlStateIncremental
            {
                mainThrottle = throttle
            };

            active_Vehicle.AtomicSet(update);
        }

        public static Vector GetAngularSpeed()
        {
            var active_Vehicle = CurrentVehicle();
            return active_Vehicle.AngularVelocity.relativeAngularVelocity;
        }

        public static double GetRotatingPC()
        {
            return GetAngularSpeed().magnitude * 100;
        }
    }

    public class TimeWarpTools
    {
        public static TimeWarp time_warp() { return GameManager.Instance.Game.ViewController.TimeWarp; }

        public static float index_to_ratio(int index)
        {
            var levels = time_warp().GetWarpRates();
            if (index < 0 || index >= levels.Length) return 0f;

            return levels[index].TimeScaleFactor;
        }

        public static int ratio_to_index(float ratio)
        {
            var levels = time_warp().GetWarpRates();
            for (int index = 0; index < levels.Length; index++ )
            {
                float factor = levels[index].TimeScaleFactor;
                if (ratio < factor)
                    return index;
            }

            return levels.Length -1;
        }
    }

}

