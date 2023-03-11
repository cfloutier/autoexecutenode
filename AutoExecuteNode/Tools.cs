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

            TimeSpan t = TimeSpan.FromSeconds(secs);

            return string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                t.Hours,
                t.Minutes,
                t.Seconds,
                t.Milliseconds);
        }

        static public KSP.Game.GameInstance Game()
        {
            return GameManager.Instance.Game;
        }


        static public VesselComponent active_vessel()
        {
            return Game().ViewController.GetActiveSimVessel();
        }

        static public VesselVehicle active_vessel_vehicle()
        {
            if (!Game().ViewController.TryGetActiveVehicle(out var vehicle)) return null;
            return vehicle as VesselVehicle;
        }


        static public CelestialBodyComponent current_body()
        {
            return active_vessel().mainBody;
        }

        private static void SetThrottle(float throttle)
        {

            var active_Vehicle = active_vessel_vehicle();
            if (active_Vehicle == null) return;

            var update = new FlightCtrlStateIncremental
            {
                mainThrottle = throttle
            };
            active_Vehicle.AtomicSet(update);
        }
    }


}