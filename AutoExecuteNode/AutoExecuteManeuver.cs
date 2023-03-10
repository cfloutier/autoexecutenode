
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using KSP.Sim;
using KSP.Sim.impl;
using KSP.Sim.Maneuver;
using KSP.Game;
using KSP.Sim.ResourceSystem;

using BepInEx.Logging;
namespace AutoExecuteNode
{
    public class AutoExecuteManeuver
    {
        public ManualLogSource logger;

        public static AutoExecuteManeuver Instance { get; set; }

        private ManeuverNodeData nextNode = null;
        List<ManeuverNodeData> activeNodes;

        TimeWarp time_warp;

        public AutoExecuteManeuver(ManualLogSource logger)
        {
            this.logger = logger;
            logger.LogMessage("AutoExecuteManeuver !");
            Instance = this;
        }

        public bool active = false;

        public void GUI()
        {
            var game = GameManager.Instance.Game;
            activeNodes = game.SpaceSimulation.Maneuvers.GetNodesForVessel(GameManager.Instance.Game.ViewController.GetActiveVehicle(true).Guid);
            nextNode = (activeNodes.Count() > 0) ? activeNodes[0] : null;
            time_warp = GameManager.Instance.Game.ViewController.TimeWarp;

            if (nextNode == null)
            {
                GUILayout.Label("no Maneuvre node");
                return;
            }

            if (!active)
            {
                if (GUILayout.Button("Run"))
                   Run();
            }
            else
            {
                if (GUILayout.Button("Stop !"))
                    Stop();
            }

            GUILayout.Label($"BurnDuration {nextNode.BurnDuration}");
            GUILayout.Label($"BurnRequiredDV {nextNode.BurnRequiredDV}");
            GUILayout.Label($"BurnVector {Tools.print_vector(nextNode.BurnVector)}");
            //GUILayout.Label($"Time {nextNode.Time}");

            var dt = nextNode.Time - game.UniverseModel.UniversalTime;

            GUILayout.Label($"tic tac {Tools.print_duration(dt)} s ");

            GUILayout.Label($"CurrentRateIndex {time_warp.CurrentRateIndex}");
        }

        void Run()
        {
            active = !active;
        }

        void Stop()
        {
            active = !active;
        }
    }
}