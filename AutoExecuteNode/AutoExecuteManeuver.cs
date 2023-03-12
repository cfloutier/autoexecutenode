
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using KSP.Sim;
using KSP.Sim.impl;
using KSP.Sim.Maneuver;
using KSP.Game;
using KSP.Sim.ResourceSystem;



using BepInEx.Logging;
using KSP.ScriptInterop.impl.moonsharp;

namespace AutoExecuteNode
{
    public class AutoExecuteManeuver
    {
        public ManualLogSource logger;

        public static AutoExecuteManeuver Instance { get; set; }

        private ManeuverNodeData next_node = null;
        List<ManeuverNodeData> activeNodes;

        TimeWarp time_warp;

        public AutoExecuteManeuver(ManualLogSource logger)
        {
            this.logger = logger;
            logger.LogMessage("AutoExecuteManeuver !");
            Instance = this;
        }

        public enum Mode{
            Off,
            Turn,
            Warp,
            Burn
        }

        public bool ok_for_next_mode = false;
        public string status_line;
        public bool debug_lines = false;

        public Mode mode = Mode.Off;


        void get_next_Node()
        {
            var game = GameManager.Instance.Game;
            activeNodes = game.SpaceSimulation.Maneuvers.GetNodesForVessel(GameManager.Instance.Game.ViewController.GetActiveVehicle(true).Guid);
            next_node = (activeNodes.Count() > 0) ? activeNodes[0] : null;
        }

        public void GUI()
        {
            get_next_Node();
            node_infos();

            if (next_node == null)
            {
                mode = Mode.Off;
                return;
            }

            debug_lines = GUILayout.Toggle(debug_lines, "debug mode");

            if (mode == Mode.Off)
            {
                if (GUILayout.Button("Run"))
                   Run();
            }
            else
            {
                if (GUILayout.Button("Stop !"))
                    Stop();
            }

            switch(mode)
            {
                case Mode.Off:
                    break;
                case Mode.Turn:
                    turn_gui();
                    break;
                case Mode.Warp:
                    warp_gui();
                    break;
                case Mode.Burn:
                    burn_gui();
                    break;
            }
        }

        public void Update()
        {
            // logger.LogInfo("AutoExecuteManeuver Update");
            switch(mode)
            {
                case Mode.Off:
                    break;
                case Mode.Turn:
                    turn_update();
                    break;
                case Mode.Warp:
                    warp_update();
                    break;
                case Mode.Burn:
                    burn_update();
                    break;
            }
        }

        void node_infos()
        {
            if (next_node == null)
            {
                GUILayout.Label("no Maneuvre node");
                return;
            }

            if (debug_lines)
            {
                var dt = Tools.remainingStartTime(next_node);
                GUILayout.Label($"tic tac {Tools.print_duration(dt)} s ");
                if (dt < 0)
                {
                    GUILayout.Label("In The Past");
                    return;
                }


                GUILayout.Label($"BurnDuration {next_node.BurnDuration}");
                GUILayout.Label($"BurnRequiredDV {next_node.BurnRequiredDV}");
                GUILayout.Label($"BurnVector {Tools.print_vector(next_node.BurnVector)}");
            }

            //GUILayout.Label($"Time {nextNode.Time}");
        }

        void Run()
        {
            mode = Mode.Turn;

            ok_for_next_mode = false;
            var autopilot = SASInfos.current_autoPilot();
            autopilot.SetActive(true);
            autopilot.AutopilotMode = AutopilotMode.Maneuver;
        }

        void Stop()
        {
            mode = Mode.Off;
            time_warp.SetRateIndex(0, true);
        }

        #region Turn

        void turn_update()
        {
            ok_for_next_mode = false;

            var autopilot = SASInfos.current_autoPilot();
            if (autopilot.AutopilotMode != AutopilotMode.Maneuver)
                autopilot.AutopilotMode = AutopilotMode.Maneuver;

            var sas = autopilot.SAS;
            var pc_sas = SASInfos.getSasResponsePC();

            double max_angle = 0.5;
            double max_angular_speed = 2;

            status_line = "Waiting for good sas direction";

            var delta_angle = SASInfos.geSASAngularDelta();
            if (System.Math.Abs(delta_angle.x) > max_angle)
                return;

            if (System.Math.Abs(delta_angle.y) > max_angle)
                return;

            if (System.Math.Abs(delta_angle.z) > max_angle)
                return;

            var angular_rotation_pc = VesselInfos.GetRotatingPC();

            status_line = "Waiting for rotation stabilisation";

            if (angular_rotation_pc > max_angular_speed)
                return;

            ok_for_next_mode = true;
        }

        void turn_gui()
        {
            GUILayout.Label("-- Turn GUI --");

            var autopilot = VesselInfos.CurrentVessel().Autopilot;
            var sas = autopilot.SAS;


            var pc_sas = SASInfos.getSasResponsePC();
            var delta_angle = SASInfos.geSASAngularDelta();

            // var angulor_vel_coord = VesselInfos.GetAngularSpeed().coordinateSystem;
            var angularVelocity = VesselInfos.GetAngularSpeed().vector;
            var angular_rotation_pc = VesselInfos.GetRotatingPC();

            // GUILayout.Label($"sas.sas_response v {Tools.print_vector(sas_response)}");

            if (debug_lines)
            {
                 GUILayout.Label($"pc_sas {pc_sas}");

                GUILayout.Label($"delta_angle {Tools.print_vector(delta_angle)}");
                GUILayout.Label($"delta_angle mag {delta_angle.magnitude}");

                // GUILayout.Label($"angulor_vel_coord {angulor_vel_coord}");
                GUILayout.Label($"angularVelocity {Tools.print_vector(angularVelocity)}");
                GUILayout.Label($"angular_rotation_pc {angular_rotation_pc}");

                GUILayout.Label($"autopilot {autopilot.AutopilotMode}");

                if (GUILayout.Button("Force SaS"))
                    autopilot.SetMode(AutopilotMode.Maneuver);

                GUILayout.Label($"ok_for_next_mode {ok_for_next_mode}");
                GUILayout.Label(status_line);
            }

            if (ok_for_next_mode)
            {
                if (GUILayout.Button("Next"))
                {
                    mode = Mode.Warp;
                    ok_for_next_mode = false;
                }
            }
            else
            {
                GUILayout.Label(status_line);
            }
        }

        #endregion

        #region Warp

        int wanted_warp_index = 0;

        void warp_update()
        {
            ok_for_next_mode = false;
            time_warp = TimeWarpTools.time_warp();

            var dt = Tools.remainingStartTime(next_node);
            if (dt < 0)
            {
                mode = Mode.Off;
            }

            wanted_warp_index = compute_wanted_warp_index(dt);
            float wanted_rate = TimeWarpTools.index_to_ratio(wanted_warp_index);

            status_line = $"TimeWarp to x{wanted_rate}";
            if (time_warp.CurrentRateIndex != wanted_warp_index)
                time_warp.SetRateIndex(wanted_warp_index, true);

            if (dt < 10)
            {
                ok_for_next_mode = true;
            }
        }

        int compute_wanted_warp_index(double dt)
        {
            double factor = 10;
            double ratio = dt / factor;

            return TimeWarpTools.ratio_to_index((float) ratio);
        }

        void warp_gui()
        {
            GUILayout.Label("-- Warp --");

            if (time_warp == null) return;

            if (debug_lines)
            {
                GUILayout.Label($"CurrentRateIndex {time_warp.CurrentRateIndex}");
                GUILayout.Label($"CurrentRate x{time_warp.CurrentRate}");
                GUILayout.Label($"index_rate x{TimeWarpTools.index_to_ratio(time_warp.CurrentRateIndex)}");
            }

            float wanted_rate = TimeWarpTools.index_to_ratio(wanted_warp_index);
            //if (GUILayout.Button($"Apply warp x{wanted_rate}"))
            time_warp.SetRateIndex(wanted_warp_index, true);

            if (ok_for_next_mode)
            {
                if (GUILayout.Button("Next"))
                {
                    mode = Mode.Burn;
                    ok_for_next_mode = false;
                }

            }
            else
            {
                GUILayout.Label(status_line);
            }
        }

        #endregion

       #region Burn

        void burn_update()
        {
            ok_for_next_mode = false;

            time_warp = TimeWarpTools.time_warp();
            if (time_warp.CurrentRateIndex != 0)
                time_warp.SetRateIndex(0, true);

            var dt = Tools.remainingStartTime(next_node);
            var end_dt = Tools.remainingEndTime(next_node);

            if (end_dt < 0)
            {
                status_line = $"ended";
                VesselInfos.SetThrottle(0);
               // mode = Mode.Off;
            }
            else if (dt < 0)
            {
                status_line = $"end in {Tools.print_duration(end_dt)}";
                VesselInfos.SetThrottle(1);
            }
            else{
                status_line = $"start in {Tools.print_duration(dt)}";
                VesselInfos.SetThrottle(0);
            }
        }

        void burn_gui()
        {
            GUILayout.Label("-- Burn --");

            GUILayout.Label(status_line);

            if (debug_lines)
            {
                var dt = Tools.remainingStartTime(next_node);
                var end_dt = Tools.remainingEndTime(next_node);

                GUILayout.Label($"dt {dt}");
                GUILayout.Label($"dt {end_dt}");
            }
        }

        #endregion

    }
}