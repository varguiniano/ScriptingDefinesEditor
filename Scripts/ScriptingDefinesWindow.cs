using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Varguiniano.ScriptingDefineEditor
{
    /// <inheritdoc />
    /// <summary>
    /// Window to set the scripting defines of the project.
    /// </summary>
    public class ScriptingDefinesWindow : EditorWindow
    {
        /// <summary>
        /// Currently selected target group.
        /// </summary>
        private BuildTargetGroup targetGroup;

        /// <summary>
        /// Flag to know if this is the first load of the window.
        /// </summary>
        private bool firstLoad = true;

        /// <summary>
        /// Flag to know if the target group changed.
        /// </summary>
        private bool targetGroupChanged;

        /// <summary>
        /// Ui element to create a reorderable list of all the choices and consequences.
        /// </summary>
        private ReorderableList reorderableDefineList;

        /// <summary>
        /// Called when the window is showed.
        /// </summary>
        [MenuItem("Window/Varguiniano/Scripting Defines")]
        public static void ShowWindow() => GetWindow(typeof(ScriptingDefinesWindow), false, "Scripting Defines");

        /// <summary>
        /// Paint the interface.
        /// </summary>
        private void OnGUI()
        {
            if (Application.isPlaying)
            {
                EditorGUILayout.HelpBox("This tool can't be used in play mode.", MessageType.Warning);
                return;
            }

            if (firstLoad) targetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);

            LoadConfig();
            LoadDefinesEditor();

            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.LabelField("Scripting defines");

                var newTargetGroup = (BuildTargetGroup) EditorGUILayout.EnumPopup("Target group", targetGroup);
                if (targetGroup != newTargetGroup)
                {
                    targetGroupChanged = true;
                    targetGroup = newTargetGroup;
                }
                else
                    targetGroupChanged = false;

                if (targetGroup == BuildTargetGroup.Unknown)
                    EditorGUILayout.HelpBox("Are you sure you want to set defines for a target that can't be compiled?",
                        MessageType.Warning);

                reorderableDefineList.DoLayoutList();

                EditorUtility.SetDirty(config);

                EditorGUILayout.HelpBox("Remember that saving the scripting defines recompiles the project!",
                    MessageType.Warning);

                if (GUILayout.Button("Save to player settings"))
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, config.DefinesString);
            }
            EditorGUILayout.EndVertical();

            firstLoad = false;
        }

        #region Config

        /// <summary>
        /// Reference to the defines config.
        /// </summary>
        private DefinesConfig config;

        /// <summary>
        /// Path to save the config to.
        /// </summary>
        private const string ConfigPath = "Assets/Varguiniano/ScriptingDefinesEditor/Data/";

        /// <summary>
        /// Config file name.
        /// </summary>
        private const string ConfigFileName = "DefinesConfig.asset";

        /// <summary>
        /// Load the config.
        /// </summary>
        private void LoadConfig()
        {
            if (!targetGroupChanged && config != null) return;
            if (!Directory.Exists(ConfigPath))
            {
                Directory.CreateDirectory(ConfigPath);
                AssetDatabase.Refresh();
            }

            config = AssetDatabase.LoadAssetAtPath<DefinesConfig>(ConfigPath + targetGroup + ConfigFileName);
            if (config != null)
            {
                config.DefinesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
                return;
            }

            config = CreateInstance<DefinesConfig>();
            AssetDatabase.CreateAsset(config, ConfigPath + targetGroup + ConfigFileName);
            AssetDatabase.Refresh();

            config.DefinesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
        }

        /// <summary>
        /// Loads the editor for the defines.
        /// </summary>
        private void LoadDefinesEditor()
        {
            if (!targetGroupChanged && reorderableDefineList != null) return;
            reorderableDefineList =
                new ReorderableList(config.DefinesList, typeof(DefineStatusPair), true, false, true, true);

            reorderableDefineList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var defineStatusPair = (DefineStatusPair) reorderableDefineList.list[index];

                rect.y += 2;
                rect.width = 550;

                defineStatusPair.Define = EditorGUI.TextField(rect, defineStatusPair.Define);

                rect.x = 600;
                rect.width = 50;

                defineStatusPair.Status = EditorGUI.Toggle(rect, defineStatusPair.Status);
            };
        }

        #endregion
    }
}
