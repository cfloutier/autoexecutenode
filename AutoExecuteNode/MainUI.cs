using System.Collections.Generic;
using System.Linq;

using UnityEngine;


using KSP.Sim;
using KSP.Sim.impl;
using KSP.Sim.Maneuver;
using KSP.Game;
using KSP.Sim.ResourceSystem;

using SpaceWarp.API.UI;

using BepInEx.Logging;
using KSP.Messages.PropertyWatchers;


namespace AutoExecuteNode
{



    public class MainUI
    {
        public ManualLogSource logger;
        private static bool guiLoaded = false;

        private static GUIStyle boxStyle, errorStyle, warnStyle, peStyle, apStyle;

        private static Color labelColor;

        #region interfaces modes

        public enum InterfaceMode { ExeNode, SAS, Vessel }
        private static string[] interfaceModes = { "Auto Execute", "SAS Infos", "Vessel Infos" };

        private InterfaceMode CurrentInterfaceMode
        {
            get => AutoExecuteNode.settings.defaultMode;
            set
            {
                if (value == AutoExecuteNode.settings.defaultMode) return;

                AutoExecuteNode.settings.defaultMode = value;
                AutoExecuteNode.Instance.SaveSetting();
            }
        }

        #endregion

        public MainUI(ManualLogSource src_logger)
        {
            logger = src_logger;
            logger.LogMessage("MainGUI");
        }

        public void onGui()
        {
            if (!guiLoaded)
                GetStyles();

            if (Tools.active_vessel() == null)
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label("No active vessel.", errorStyle);
                GUILayout.FlexibleSpace();
                return;
            }

            if (Tools.current_body() == null)
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label("No active body.", errorStyle);
                GUILayout.FlexibleSpace();

                return;
            }
            GUILayout.BeginVertical();

            GUILayout.Label($"Active Vessel: {Tools.active_vessel()}");
            GUILayout.Label($"body : {Tools.current_body()}");

            // Mode selection.
            GUILayout.BeginHorizontal();
            CurrentInterfaceMode = (InterfaceMode)GUILayout.SelectionGrid((int)CurrentInterfaceMode, interfaceModes, 4);
            GUILayout.EndHorizontal();

            // GUILayout.EndVertical();

            // return;
            // Draw one of the modes.
            switch (CurrentInterfaceMode)
            {
                case InterfaceMode.ExeNode:
                    if (AutoExecuteManeuver.Instance != null)
                        AutoExecuteManeuver.Instance.GUI();
                    else
                        logger.LogError("Missing AutoExecuteManeuver");
                    break;
                case InterfaceMode.SAS: SASInfos(); break;
                case InterfaceMode.Vessel: VesselInfo(); break;
                default:
                    break;
            } 

            GUILayout.EndVertical();



           // GUI.DragWindow(new Rect(0, 0, 10000, 500));
        }

        private void GetStyles()
        {
            if (boxStyle != null)
                return;

            boxStyle = GUI.skin.GetStyle("Box");
            errorStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
            warnStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
            apStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
            peStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
            errorStyle.normal.textColor = Color.red;
            warnStyle.normal.textColor = Color.yellow;
            labelColor = GUI.skin.GetStyle("Label").normal.textColor;

            guiLoaded = true;
        }

        void SASInfos()
        {
            var sas = Tools.active_vessel().Autopilot.SAS;
            if (sas == null)
            {
                GUILayout.Label("NO SAS");
                return;
            }

            GUILayout.Label($"sas.dampingMode {sas.dampingMode}");
            GUILayout.Label($"sas.ReferenceFrame {sas.ReferenceFrame}");
            GUILayout.Label($"sas.AutoTune {sas.AutoTune}");
            GUILayout.Label($"sas.lockedMode {sas.lockedMode}");
            GUILayout.Label($"sas.LockedRotation {Tools.print_vector(sas.LockedRotation.eulerAngles)}");


            GUILayout.Label($"sas.TargetOrientation {Tools.print_vector(sas.TargetOrientation)}");
            GUILayout.Label($"sas.PidLockedPitch {sas.PidLockedPitch}");
            GUILayout.Label($"sas.PidLockedRoll {sas.PidLockedRoll}");
            GUILayout.Label($"sas.PidLockedYaw {sas.PidLockedYaw}");
        }

        void VesselInfo()
        {
            var vehicle = Tools.active_vessel_vehicle();

            if (vehicle == null)
            {
                GUILayout.Label("NO vessel");
                return;
            }

            GUILayout.Label($"mainThrottle {vehicle.mainThrottle}");
            GUILayout.Label($"pitch {vehicle.pitch:n2} yaw {vehicle.yaw:n2} roll {vehicle.roll}");

            GUILayout.Label($"AltitudeFromTerrain {vehicle.AltitudeFromTerrain:n2}");
            GUILayout.Label($"Lat {vehicle.Latitude:n2} Lon {vehicle.Longitude:n2}");
            GUILayout.Label($"IsInAtmosphere {vehicle.IsInAtmosphere}");
            GUILayout.Label($"LandedOrSplashed {vehicle.LandedOrSplashed}");

            GUILayout.Label($"rotation {vehicle.Rotation}");






            // 
            // GUILayout.Label($"AngularMomentum {Tools.print_vector(vehicle.AngularMomentum.vector)}");





        }
    }
}