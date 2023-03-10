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
    public class TestUI
    {
        public ManualLogSource logger;
        private static bool guiLoaded = false;

        private static GUIStyle boxStyle, errorStyle, warnStyle, peStyle, apStyle;

        private static Color labelColor;

        private static VesselComponent activeVessel;
        private static CelestialBodyComponent currentBody;

        #region interfaces modes

        private static string[] interfaceModes = { "Main", "SAS" };

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

        public TestUI(ManualLogSource src_logger)
        {
            logger = src_logger;
            logger.LogMessage("MainGUI");
        }

        public void onGui()
        {
            if (!guiLoaded)
                GetStyles();

            if ((activeVessel = GameManager.Instance.Game.ViewController.GetActiveSimVessel()) == null)
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label("No active vessel.", errorStyle);
                GUILayout.FlexibleSpace();
                return;
            }

            if ((currentBody = activeVessel.mainBody) == null)
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label("No active body.", errorStyle);
                GUILayout.FlexibleSpace();
                return;
            }
            GUILayout.BeginVertical();

            GUILayout.Label($"Active Vessel: {activeVessel.DisplayName}");
            GUILayout.Label($"body : {currentBody.DisplayName}");

            // Mode selection.

            if (AutoExecuteManeuver.Instance != null)
                AutoExecuteManeuver.Instance.GUI();
            else
                logger.LogError("Missing AutoExecuteManeuver");

            //GUILayout.BeginHorizontal();
            //CurrentInterfaceMode = (InterfaceMode)GUILayout.SelectionGrid((int)CurrentInterfaceMode, interfaceModes, 4);
            //GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            return;

            // Draw one of the modes.
            switch (CurrentInterfaceMode)
            {
                case InterfaceMode.Main:
                    if (AutoExecuteManeuver.Instance != null)
                        AutoExecuteManeuver.Instance.GUI();
                    else
                        logger.LogError("Missing AutoExecuteManeuver");
                    break;
                case InterfaceMode.SAS: SASInfos(); break;
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
            var sas = activeVessel.Autopilot.SAS;
            if (sas == null)
            {
                GUILayout.Label("NO SAS");
                return;
            }

            GUILayout.Label($"sas.dampingMode{sas.dampingMode}");
            GUILayout.Label($"sas.ReferenceFrame{sas.ReferenceFrame}");
            GUILayout.Label($"sas.AutoTune {sas.AutoTune}");
            GUILayout.Label($"sas.lockedMode {sas.lockedMode}");
            GUILayout.Label($"sas.LockedRotation {Tools.print_vector(sas.LockedRotation.eulerAngles)}");


            GUILayout.Label($"sas.TargetOrientation {Tools.print_vector(sas.TargetOrientation)}");
            GUILayout.Label($"sas.PidLockedPitch {sas.PidLockedPitch}");
            GUILayout.Label($"sas.PidLockedRoll {sas.PidLockedRoll}");
            GUILayout.Label($"sas.PidLockedYaw {sas.PidLockedYaw}");
        }
    }

    public class MainUI
    {
        public ManualLogSource logger;


        public MainUI(ManualLogSource src_logger)
        {
            logger = src_logger;
            logger.LogMessage("MainUI()");
        }

        // GUI.
        private static bool guiLoaded = false;
        public bool visible = false;

        private static GUIStyle boxStyle, errorStyle, warnStyle, peStyle, apStyle;

        // private static Vector2 scrollPositionBodies;
        // private static Vector2 scrollPositionVessels;
        private static Color labelColor;
        private static GameState[] validScenes = new[] { GameState.FlightView, GameState.Map3DView };

        private static bool ValidScene => validScenes.Contains(GameManager.Instance.Game.GlobalGameState.GetState());


        #region interfaces modes

        private static string[] interfaceModes = { "Main", "SAS" };

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

        #region GUI

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

        public void onGui(int windowID)
        {
            if (!guiLoaded)
            {
                GetStyles();
            }


            //if ((activeVessel = GameManager.Instance.Game.ViewController.GetActiveSimVessel()) == null)
            //{
            //    GUILayout.FlexibleSpace();
            //    GUILayout.Label("No active vessel.", errorStyle);
            //    GUILayout.FlexibleSpace();
            //    return;
            //}
            //if ((currentBody = activeVessel.mainBody) == null)
            //{
            //    GUILayout.FlexibleSpace();
            //    GUILayout.Label("No active body.", errorStyle);
            //    GUILayout.FlexibleSpace();
            //    return;
            //}



            //GUILayout.BeginVertical();

            //GUILayout.Label($"Active Vessel: {activeVessel.DisplayName}");
            //GUILayout.Label($"body : {currentBody.DisplayName}");

            //// Mode selection.

            //GUILayout.BeginHorizontal();
            //CurrentInterfaceMode = (InterfaceMode)GUILayout.SelectionGrid((int)CurrentInterfaceMode, interfaceModes, 4);
            //GUILayout.EndHorizontal();

            //// Draw one of the modes.
            //switch (CurrentInterfaceMode)
            //{
            //    case InterfaceMode.Main: AutoExecuteManeuver.Instance.GUI(); break;
            //    case InterfaceMode.SAS: SASInfos(); break;
            //    default:
            //        break;
            //}

            //GUILayout.EndVertical();
            //GUI.DragWindow(new Rect(0, 0, 10000, 500));
        }

     

        void MainGUI()
        {
            //GUILayout.Label($"altitude : {activeVessel.AltitudeFromTerrain/1000:n2} km");
            //GUILayout.BeginHorizontal();

            //if (GUILayout.Button("Rotate"))
            //    Rotate();

            //if (GUILayout.Button("KillRot"))
            //    KillRot();

            //GUILayout.EndHorizontal();

            //SASInfos();
        }

        

        //void KillRot()
        //{
        //    foreach (PartComponent part in activeVessel.SimulationObject.PartOwner.Parts)
        //    {
        //        part.SimulationObject.Rigidbody?.KillAngularVelocity();
        //    }
        //}

        //void Rotate()
        //{
        //    //var sas = activeVessel.Autopilot.SAS;

        //    //var vector = new Vector(activeVessel.mainBody.coordinateSystem, new Vector3d(1, 0, 0));

        //    //sas.SetTargetOrientation(vector, true);

        //    activeVessel.Autopilot.SetMode(AutopilotMode.Prograde);



        //    // sas.lockedMode = true;

        //    // var system = currentBody.coordinateSystem;

        //    // var rotation = new Rotation(system, QuaternionD.Euler(new Vector3d(0, 90, 90)));
        //    // sas.LockRotation(rotation);
        //}

        //void Stop()
        //{
        //    var sas = activeVessel.Autopilot.SAS;
        //    // sas.lockedMode = false;
        //} 

        void FuelGUI()
        {
            // GUILayout.BeginHorizontal();
            // GUILayout.Label("Latitude (Degrees):", GUILayout.Width(windowWidth / 2));
            // latitudeString = GUILayout.TextField(latitudeString);
            // float.TryParse(latitudeString, out latitude);
            // GUILayout.EndHorizontal();

            // GUILayout.BeginHorizontal();
            // GUILayout.Label("Longitude (Degrees):", GUILayout.Width(windowWidth / 2));
            // longitudeString = GUILayout.TextField(longitudeString);
            // float.TryParse(longitudeString, out longitude);
            // GUILayout.EndHorizontal();

            // GUILayout.BeginHorizontal();
            // GUILayout.Label("Height (m):", GUILayout.Width(windowWidth / 2));
            // heightString = GUILayout.TextField(heightString);
            // float.TryParse(heightString, out height);
            // GUILayout.EndHorizontal();

            // BodySelectionGUI();

            if (GUILayout.Button("Re Fuel"))
                ReFuel();
        }

        // Draws the body selection GUI.
        #endregion

        #region Functions

        void ReFuel()
        {


        }

        // Sets vessel orbit according to the current interfaceMode.
        //void SetOrbit()
        //{
        //    GameInstance game = GameManager.Instance.Game;

        //    if (CurrentInterfaceMode == InterfaceMode.Simple)
        //    {
        //        // Set orbit using just altitude.
        //        game.SpaceSimulation.Lua.TeleportToOrbit(
        //            activeVessel.Guid,
        //            selectedBody,
        //            0,
        //            0,
        //            (double)altitudeKM * 1000f + GameManager.Instance.Game.CelestialBodies.GetRadius(selectedBody),
        //            0,
        //            0,
        //            0,
        //            0);
        //    }
        //    else
        //    {
        //        // Set orbit using semi-major axis and other orbital parameters.
        //        game.SpaceSimulation.Lua.TeleportToOrbit(
        //            activeVessel.Guid,
        //            selectedBody,
        //            inclinationDegrees,
        //            eccentricity,
        //            (double)semiMajorAxisKM * 1000f,
        //            ascendingNode,
        //            argOfPeriapsis,
        //            0,
        //            0);
        //    }
        //}



        #endregion

       
    }

    public enum InterfaceMode
    {
        Main,
        SAS,
    }
}