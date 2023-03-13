
using UnityEngine;
using KSP.Sim;
using BepInEx.Logging;

namespace AutoExecuteNode
{
    public class Styles
    {
        private static bool guiLoaded = false;

        public static GUIStyle box, errorStyle, warnStyle, peStyle, apStyle;
        public static Color labelColor;

        public static void Init()
        {
            if (!guiLoaded)
            {
                GetStyles();
            }
        }

        public static void GetStyles()
        {
            if (box != null)
                return;

            box = GUI.skin.GetStyle("Box");
            errorStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
            warnStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
            apStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
            peStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
            errorStyle.normal.textColor = Color.red;
            warnStyle.normal.textColor = Color.yellow;
            labelColor = GUI.skin.GetStyle("Label").normal.textColor;

            guiLoaded = true;
        }
    }

    public class MainUI
    {
        public ManualLogSource logger;

        #region interfaces modes

        public enum InterfaceMode { ExeNode, SAS, Vessel }
        private static string[] interfaceModes = { "Auto Execute", "SAS Infos", "Vessel Infos" };

        private InterfaceMode CurrentInterfaceMode
        {
            get => Settings.settings.defaultMode;
            set
            {
                if (value == Settings.settings.defaultMode) return;
                // logger.LogInfo("Value is different to " + AutoExecuteNode.settings.defaultMode);
                logger.LogInfo("InterfaceMode set " + value);
                Settings.settings.defaultMode = value;

                Settings.Save();
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
            Styles.Init();

            if (VesselInfos.currentVessel() == null)
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label("No active vessel.", Styles.errorStyle);
                GUILayout.FlexibleSpace();
                return;
            }

            if (VesselInfos.currentBody() == null)
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label("No active body.", Styles.errorStyle);
                GUILayout.FlexibleSpace();

                return;
            }

            GUILayout.BeginVertical();

            // GUILayout.Label($"Active Vessel: {VesselInfos.CurrentVessel()}");
            // GUILayout.Label($"body : {VesselInfos.CurrentBody()}");

            // Mode selection.
            // GUILayout.BeginHorizontal();
            // CurrentInterfaceMode = (InterfaceMode)GUILayout.SelectionGrid((int)CurrentInterfaceMode, interfaceModes, 4);
            // GUILayout.EndHorizontal();

            CurrentInterfaceMode = InterfaceMode.ExeNode;

            // GUILayout.EndVertical();

            // return;
            // Draw one of the modes.
            switch (CurrentInterfaceMode)
            {
                case InterfaceMode.ExeNode:
                    if (AutoExecuteManeuver.Instance != null)
                        AutoExecuteManeuver.Instance.onGUI();
                    else
                        logger.LogError("Missing AutoExecuteManeuver");
                    break;
                case InterfaceMode.SAS: SASInfos(); break;
                case InterfaceMode.Vessel: VesselInfo(); break;
                default:
                    break;
            }

            GUILayout.EndVertical();


            GUI.DragWindow(new Rect(0, 0, 10000, 500));
        }

        public static void SASInfos()
        {
            var sas = VesselInfos.currentVessel().Autopilot.SAS;
            if (sas == null)
            {
                GUILayout.Label("NO SAS");
                return;
            }

            GUILayout.Label($"sas.dampingMode {sas.dampingMode}");
            GUILayout.Label($"sas.ReferenceFrame {sas.ReferenceFrame}");
            GUILayout.Label($"sas.AutoTune {sas.AutoTune}");
            GUILayout.Label($"sas.lockedMode {sas.lockedMode}");
            GUILayout.Label($"sas.LockedRotation {Tools.printVector(sas.LockedRotation.eulerAngles)}");

            GUILayout.Label($"sas.TargetOrientation {Tools.printVector(sas.TargetOrientation)}");
            GUILayout.Label($"sas.PidLockedPitch {sas.PidLockedPitch}");
            GUILayout.Label($"sas.PidLockedRoll {sas.PidLockedRoll}");
            GUILayout.Label($"sas.PidLockedYaw {sas.PidLockedYaw}");
        }

        void VesselInfo()
        {
            var vehicle = VesselInfos.currentVehicle();

            if (vehicle == null)
            {
                GUILayout.Label("NO vessel");
                return;
            }

            GUILayout.Label($"mainThrottle {vehicle.mainThrottle}");
            GUILayout.Label($"pitch {vehicle.pitch:n3} yaw {vehicle.yaw:n3} roll {vehicle.roll:n3}");

            GUILayout.Label($"AltitudeFromTerrain {vehicle.AltitudeFromTerrain:n2}");
            GUILayout.Label($"Lat {vehicle.Latitude:n2} Lon {vehicle.Longitude:n2}");
            GUILayout.Label($"IsInAtmosphere {vehicle.IsInAtmosphere}");
            GUILayout.Label($"LandedOrSplashed {vehicle.LandedOrSplashed}");


            var body = VesselInfos.currentBody();
            var coord = body.coordinateSystem;
            var body_location = Rotation.Reframed(vehicle.Rotation, coord);

            GUILayout.Label($"coordinate_system {vehicle.Rotation.coordinateSystem}");
            GUILayout.Label($"body_location {Tools.printVector(body_location.localRotation.eulerAngles)}");

            // GUILayout.Label($"AngularMomentum {Tools.print_vector(vehicle.AngularMomentum.vector)}");
        }
    }
}