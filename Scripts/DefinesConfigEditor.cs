using UnityEditor;

namespace Varguiniano.ScriptingDefinesEditor
{
    /// <inheritdoc />
    /// <summary>
    /// Custom editor for the defines config.
    /// </summary>
    [CustomEditor(typeof(DefinesConfig))]
    public class DefinesConfigEditor : Editor
    {
        /// <inheritdoc />
        /// <summary>
        /// Paint the UI.
        /// </summary>
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("To edit this configuration, use the Defines Tool.", MessageType.Info);
            EditorGUI.BeginDisabledGroup(true);
            base.OnInspectorGUI();
            EditorGUI.EndDisabledGroup();
        }
    }
}