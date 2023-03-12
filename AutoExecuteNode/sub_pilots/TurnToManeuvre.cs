
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
    public class TurnToManeuvre : GenericPilot
    {
        public AutoExecuteManeuver parent;

        public TurnToManeuvre(AutoExecuteManeuver parent)
        {
            this.parent = parent;
        }

        public override void Start()
        {
            // reset time warp
            var time_warp = TimeWarpTools.time_warp();
            time_warp.SetRateIndex(0, false);
        }

        public override void onUpdate()
        {
            finished = false;

            var autopilot = SASInfos.currentAutoPilot();
            if (autopilot == null)
                return;
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

            status_line = "ok";

            finished = true;
        }

        public override void onGui()
        {
            GUILayout.Label("-- Turn GUI --");

            var autopilot = VesselInfos.currentVessel().Autopilot;
            var sas = autopilot.SAS;


            var pc_sas = SASInfos.getSasResponsePC();
            var delta_angle = SASInfos.geSASAngularDelta();

            // var angulor_vel_coord = VesselInfos.GetAngularSpeed().coordinateSystem;
            var angularVelocity = VesselInfos.GetAngularSpeed().vector;
            var angular_rotation_pc = VesselInfos.GetRotatingPC();

            // GUILayout.Label($"sas.sas_response v {Tools.print_vector(sas_response)}");

            if (parent.debug_infos)
            {
                GUILayout.Label($"pc_sas {pc_sas}");

                GUILayout.Label($"delta_angle {Tools.printVector(delta_angle)}");
                GUILayout.Label($"delta_angle mag {delta_angle.magnitude}");

                // GUILayout.Label($"angulor_vel_coord {angulor_vel_coord}");
                GUILayout.Label($"angularVelocity {Tools.printVector(angularVelocity)}");
                GUILayout.Label($"angular_rotation_pc {angular_rotation_pc}");

                GUILayout.Label($"autopilot {autopilot.AutopilotMode}");

                if (GUILayout.Button("Force SaS"))
                    autopilot.SetMode(AutopilotMode.Maneuver);

                GUILayout.Label($"finished {finished}");
            }

            GUILayout.Label(status_line);
        }
    }

}