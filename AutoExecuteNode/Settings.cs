using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using BepInEx.Logging;
using Newtonsoft.Json;

namespace AutoExecuteNode
{
    public class AutoExecuteNodeSettings
    {
        public MainUI.InterfaceMode defaultMode = MainUI.InterfaceMode.ExeNode;
    }

    public class Settings
    {
        public static AutoExecuteNodeSettings settings { get; set; }
        public static string settings_path;

        private static ManualLogSource logger;

        public static void Init(string settings_path, ManualLogSource logger )
        { 
            Settings.settings_path = settings_path;
            Settings.logger = logger;
            Load();
        }

        static public void Save()
        {
            try
            {
                File.WriteAllText(settings_path, JsonConvert.SerializeObject(settings));
            }
            catch (Exception ex)
            {
                logger.LogError($"Save exception {ex}");
            }
        }

        static void Load()
        {
            try
            {
                settings = JsonConvert.DeserializeObject<AutoExecuteNodeSettings>(File.ReadAllText(settings_path));
            }

            catch (FileNotFoundException ex)
            {
                Settings.logger.LogWarning($"Load exception {ex}");
                settings = new AutoExecuteNodeSettings();
            }
            catch (Exception ex)
            {
                Settings.logger.LogError($"Save exception {ex}");
                settings = new AutoExecuteNodeSettings();
            }
        }
    }

}




