using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;

using UnityEngine;
using Newtonsoft.Json;

using KSP.Sim;
using KSP.Sim.impl;
using KSP.Sim.Maneuver;
using KSP.Game;
using KSP.Sim.ResourceSystem;

using BepInEx;
using SpaceWarp.API;
using SpaceWarp.API.Mods;
using KSP.UI.Binding;
using KSP.Messages.PropertyWatchers;

using SpaceWarp.API.UI.Appbar;
using SpaceWarp.API.UI;

using SpaceWarp;
using SpaceWarp.API.Assets;
using BepInEx.Logging;

namespace First_test
{
    [BepInPlugin("com.location.name.First_Test", "First_Test", "0.0.0")]
    [BepInDependency(SpaceWarpPlugin.ModGuid, SpaceWarpPlugin.ModVer)]
    public class First_test : BaseSpaceWarpPlugin
    {
        public static First_test Instance { get; set; }
        #region Fields

        public ManualLogSource logger;

        public static First_testSettings settings { get; set; }

        // Main.
        public static bool loaded = false;

        // Paths.
        private static string _assemblyFolder;
        private static string AssemblyFolder =>
            _assemblyFolder ?? (_assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

        private static string _settingsPath;
        private static string SettingsPath =>
            _settingsPath ?? (_settingsPath = Path.Combine(AssemblyFolder, "Settings.json"));

        // GUI.
        private static GameState[] validScenes = new[] { GameState.FlightView, GameState.Map3DView };

        private static bool ValidScene => validScenes.Contains(GameManager.Instance.Game.GlobalGameState.GetState());

        private MainUI main_ui;

        #endregion

        #region Main

        public override void OnInitialized()
        {
            base.OnInitialized();
            logger = BepInEx.Logging.Logger.CreateLogSource("First_test");

            Instance = this;

            if (loaded)
            {
                Destroy(this);
            }

            loaded = true;

            //new AutoExecuteManeuver();

            gameObject.hideFlags = HideFlags.HideAndDontSave;
            DontDestroyOnLoad(gameObject);

            LoadSettings();

            main_ui = new MainUI();

            Appbar.RegisterAppButton(
                "My very first Test",
                "BTN-First_testButton_2",
                AssetManager.GetAsset<Texture2D>($"{SpaceWarpMetadata.ModID}/images/icon.png"),
                ToggleButton
            );

            logger.LogInfo("OnInitialized");
        }

        public void Awake()
        {
            logger.LogInfo("Awake");
            main_ui.Awake();
        }

        public void Update()
        {
             logger.LogInfo("Update");
            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.D) && ValidScene)
                ToggleButton(!main_ui.visible);
        }

        public void OnGUI()
        {
            if (drawUI && ValidScene)
            {
                if (!guiLoaded)
                    GetStyles();

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
            GameObject.Find("BTN-First_testButton_2")?.GetComponent<UIValue_WriteBool_Toggle>()?.SetValue(toggle);
        }

        #endregion

        #region Settings

        public void SaveSetting()
        {
            File.WriteAllText(SettingsPath, JsonConvert.SerializeObject(settings));
        }

        private void LoadSettings()
        {
            try
            {
                settings = JsonConvert.DeserializeObject<First_testSettings>(File.ReadAllText(SettingsPath));
            }
            catch (FileNotFoundException)
            {
                settings = new First_testSettings();
            }
        }

        #endregion
    }

    public class AutoExecuteNodeSettings
    {
        public InterfaceMode defaultMode = InterfaceMode.Main;
    }
}