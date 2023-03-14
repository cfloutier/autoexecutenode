
using UnityEngine;
using KSP.Sim;
using BepInEx.Logging;
using SpaceWarp.API.Assets;

namespace COSMAT
{
    public class Styles
    {
        private static bool guiLoaded = false;

        public static GUIStyle box, window, error, warning, button, button_on, small_button;

        public static GUIStyle console, phase_ok, phase_warning, phase_error;

        public static Texture2D gear;
        public static Color labelColor;

        public static void Init()
        {
            if (!guiLoaded)
            {
                GetStyles();
            }
        }

        // BEPEXVersion
        public static Texture2D loadIcon(string path)
        {
           var imageTexture = AssetManager.GetAsset<Texture2D>($"{AutoExecuteNode.mod_id}/images/{path}.png");

            //   Check if the texture is null
            if (imageTexture == null)
            {
                // Print an error message to the Console
                Debug.LogError("Failed to load image texture from path: " + path);

                // Print the full path of the resource
                Debug.Log("Full resource path: " + Application.dataPath + "/" + path);

                // Print the type of resource that was expected
                Debug.Log("Expected resource type: Texture2D");
            }

            return imageTexture;
        }

        // Unity_Editor_Version
        // static public Texture2D loadIcon(string path)
        // {
        //     var imageTexture = Resources.Load<Texture2D>(path);

        //     // Check if the texture is null
        //     if (imageTexture == null)
        //     {
        //         // Print an error message to the Console
        //         Debug.LogError("Failed to load image texture from path: " + path);

        //         // Print the full path of the resource
        //         Debug.Log("Full resource path: " + Application.dataPath + "/" + path);

        //         // Print the type of resource that was expected
        //         Debug.Log("Expected resource type: Texture2D");
        //     }

        //     return imageTexture;
        // }

        private static void resetToNormal(GUIStyle style)
        {
            style.hover = style.normal;
            style.active = style.normal;
            style.focused = style.normal;
            style.onNormal = style.normal;
            style.onHover = style.normal;
            style.onActive = style.normal;
            style.onFocused = style.normal;
        }

        public static void GetStyles()
        {
            if (box != null)
                return;

            box = GUI.skin.GetStyle("Box");
            // Set the background color of the Box
            box.normal.background = Texture2D.whiteTexture;
            // Set the font size of the Box
            box.fontSize = 20;
            // Set the alignment of the Box
            box.alignment = TextAnchor.MiddleCenter;

            // WINDOW

            // Define the GUIStyle for the window
            window = new GUIStyle(GUI.skin.window);

            window.border = new RectOffset(25, 25, 35, 25);
            window.margin = new RectOffset(83, 43, 20, 20);
            window.padding = new RectOffset(20, 13, 44, 17);
            window.overflow = new RectOffset(0, 0, 0, 0);

            window.fontSize = 20;
            window.contentOffset = new Vector2(7, -36);

            // Set the background color of the window
            window.normal.background = loadIcon("window");
            window.normal.textColor = Color.black;
            resetToNormal(window);
            window.alignment = TextAnchor.UpperLeft;
            window.stretchWidth = true;

            // Button //////////////////////

            button = new GUIStyle(GUI.skin.GetStyle("Button"));
            button.normal.background = loadIcon("Button-normal");
            resetToNormal(button);
            button.hover.background = loadIcon("Button-over");
            button.active.background = loadIcon("Button-active");
            button.border = new RectOffset(8, 10, 8, 10);
            button.margin = new RectOffset(0, 0, 0, 5);
            button.padding = new RectOffset(6, 6, 3, 7);
            button.overflow = new RectOffset(4, 4, 4, 4);

            // On Button 

            button_on = new GUIStyle(GUI.skin.GetStyle("Button"));
            button_on.normal.background = loadIcon("Button-active");
            resetToNormal(button_on);
            button_on.hover.background = loadIcon("Button-over");
            button_on.active.background = loadIcon("Button-normal");

            button_on.border = new RectOffset(8, 10, 8, 10);
            button_on.margin = new RectOffset(0, 0, 0, 5);
            button_on.padding = new RectOffset(6, 6, 3, 7);
            button_on.overflow = new RectOffset(4, 4, 4, 4);


            // Small Button

            small_button = new GUIStyle(GUI.skin.GetStyle("Button"));
            small_button.normal.background = loadIcon("Button-normal");
            resetToNormal(small_button);
            small_button.hover.background = loadIcon("Button-over");
            small_button.active.background = loadIcon("Button-active");
            small_button.onNormal.background = loadIcon("Button-active");
            small_button.border = new RectOffset(8, 10, 8, 10);
            small_button.margin = new RectOffset(0, 0, 0, 5);
            small_button.padding = new RectOffset(5, 5, 5, 5);
            small_button.overflow = new RectOffset(0, 0, 0, 2);
            small_button.fontSize = 15;

            // GEAR icon
            gear = loadIcon("gear");

            error = new GUIStyle(GUI.skin.GetStyle("Label"));
            warning = new GUIStyle(GUI.skin.GetStyle("Label"));
            error.normal.textColor = Color.red;
            warning.normal.textColor = Color.yellow;
            labelColor = GUI.skin.GetStyle("Label").normal.textColor;

            phase_ok = new GUIStyle(GUI.skin.GetStyle("Label"));
            phase_ok.normal.textColor = ColorTools.parseColor("#00BC16");
            phase_ok.fontSize = 20;

            phase_warning = new GUIStyle(GUI.skin.GetStyle("Label"));
            phase_warning.normal.textColor = ColorTools.parseColor("#BC9200");
            phase_warning.fontSize = 20;

            phase_error = new GUIStyle(GUI.skin.GetStyle("Label"));
            phase_error.normal.textColor = ColorTools.parseColor("#B30F0F");
            phase_error.fontSize = 20;

            console = new GUIStyle(GUI.skin.GetStyle("Label"));
            console.normal.textColor = ColorTools.parseColor("#090A0A");
            console.fontSize = 15;

            guiLoaded = true;
        }
    }

}