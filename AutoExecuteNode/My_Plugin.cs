using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;

using UnityEngine;
using Newtonsoft.Json;

using KSP.Sim.impl;
using KSP.Game;
using KSP.Sim.ResourceSystem;
using KSP.UI.Binding;

using BepInEx;
using SpaceWarp;
using SpaceWarp.API;
using SpaceWarp.API.Mods;
using SpaceWarp.API.Assets;
using SpaceWarp.API.UI;
using SpaceWarp.API.UI.Appbar;
using BepInEx.Logging;

namespace AutoExecuteNode
{
    [BepInDependency(SpaceWarpPlugin.ModGuid,SpaceWarpPlugin.ModVer)]
    [BepInPlugin(ModGuid, ModName, ModVer)]
    public class AutoExecuteNode : BaseSpaceWarpPlugin
    {
        public static AutoExecuteNode Instance { get; set; }

        public const string ModGuid = "AutoExecuteNode";
        public const string ModName = "AutoExecuteNode";
        public const string ModVer = "0.0.0";

        #region Fields

        // Main.
        public static bool loaded = false;
        public static AutoExecuteNode instance;

        // Paths.
        private static string _assemblyFolder;
        private static string AssemblyFolder =>
            _assemblyFolder ?? (_assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

        private static string _settingsPath;
        private static string SettingsPath =>
            _settingsPath ?? (_settingsPath = Path.Combine(AssemblyFolder, "settings.json"));


        public ManualLogSource logger;


        // GUI.

        private bool drawUI = false;
        private Rect windowRect;
        private int windowWidth = 500;
        private int windowHeight = 700;

        private static GameState[] validScenes = new[] { GameState.FlightView, GameState.Map3DView };

        private static bool ValidScene()
        {
            if (Tools.Game() == null) return false;
            var state = Tools.Game().GlobalGameState.GetState();
            return validScenes.Contains(state);
        }


        MainUI main_ui;
        AutoExecuteManeuver auto_execute_maneuver;

        #endregion

        #region Main

        public override void OnInitialized()
        {
            if (loaded)
            {
                Destroy(this);
            }

            logger = BepInEx.Logging.Logger.CreateLogSource("AutoExecuteNode");

            loaded = true;
            instance = this;

            gameObject.hideFlags = HideFlags.HideAndDontSave;
            DontDestroyOnLoad(gameObject);

            logger.LogMessage("building AutoExecuteManeuver");
            auto_execute_maneuver = new AutoExecuteManeuver(logger);
            main_ui = new MainUI(logger);

            Appbar.RegisterAppButton(
                "TEST",
                "BTN-AutoExecuteNodeButton",
                AssetManager.GetAsset<Texture2D>($"{SpaceWarpMetadata.ModID}/images/icon.png"),
                ToggleButton);
        }

        void Awake()
        {
            windowRect = new Rect((Screen.width * 0.7f) - (windowWidth / 2), (Screen.height / 2) - (windowHeight / 2), 0, 0);
            LoadSettings();
        }

        void Update()
        {
            if (ValidScene())
            {
                if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.D) )
                    ToggleButton(!drawUI);

                if (auto_execute_maneuver != null)
                    auto_execute_maneuver.Update();
            }
        }

        void OnGUI()
        {
            if (drawUI && ValidScene())
            {
                GUI.skin = Skins.ConsoleSkin;

                windowRect = GUILayout.Window(
                    GUIUtility.GetControlID(FocusType.Passive),
                    windowRect,
                    FillWindow,
                    "<color=#696DFF>TEST</color>",
                    GUILayout.Height(0),
                    GUILayout.Width(350));
            }
        }

        public void ToggleButton(bool toggle)
        {
            drawUI = toggle;
            GameObject.Find("BTN-AutoExecuteNodeButton")?.GetComponent<UIValue_WriteBool_Toggle>()?.SetValue(toggle);
        }

        #endregion

        private void FillWindow(int windowID)
        {
            GUILayout.BeginVertical();

            if (GUI.Button(new Rect(windowRect.width - 18, 2, 16, 16), "X"))
                ToggleButton(false);

            main_ui.onGui();

            GUILayout.EndVertical();
        }

        #region Settings

        public static AutoExecuteNodeSettings settings { get; set; }

        public void SaveSetting()
        {
            File.WriteAllText(SettingsPath, JsonConvert.SerializeObject(settings));
        }

        private void LoadSettings()
        {
            try
            {
                settings = JsonConvert.DeserializeObject<AutoExecuteNodeSettings>(File.ReadAllText(SettingsPath));
            }
            catch (FileNotFoundException)
            {
                settings = new AutoExecuteNodeSettings();
            }
        }

        #endregion
    }

    public class AutoExecuteNodeSettings
    {
        public MainUI.InterfaceMode defaultMode = MainUI.InterfaceMode.ExeNode;
    }
}