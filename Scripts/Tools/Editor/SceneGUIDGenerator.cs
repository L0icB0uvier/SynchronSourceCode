using SavingSystem;
using UnityEditor;
using UnityEngine;

namespace Tools.Editor
{
    public class SceneGUIDGenerator : EditorWindow
    {
        [MenuItem("Tools/Scene GUID Generator")]
        public static void ShowWindow()
        {
            SceneGUIDGenerator window = (SceneGUIDGenerator)GetWindow(typeof(SceneGUIDGenerator), false,"Scene GUID Generator");
            window.Show();
        }

        private void OnGUI()
        {
            if(GUILayout.Button("Generate GUID"))
            {
                GenerateSceneObjectGuid();
            }
        }

        private static void GenerateSceneObjectGuid()
        {
            var guids = FindObjectsOfType<ObjectUniqueIdentifier>();
            foreach (var guid in guids)
            {
                guid.GenerateID();
            }
        }
    }
}
