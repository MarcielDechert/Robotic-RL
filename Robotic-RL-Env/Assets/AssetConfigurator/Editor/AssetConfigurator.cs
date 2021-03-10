using UnityEditor;
using UnityEngine;

namespace AssetConfigurator
{
    public class AssetConfigurator
    {


        [MenuItem("GameObject/Asset Configurator/Create Preview Controller", false, 0)]
        public static void CreateAssetPreviewController()
        {

            string[] results = AssetDatabase.FindAssets("AssetPreviewController");

            AssetConfigurationController sceneController = GameObject.FindObjectOfType<AssetConfigurationController>();

            if (sceneController != null)
            {
                EditorUtility.DisplayDialog("Create Asset Preview Controller Failed", "There is already an Asset Preview Controller in the current scene.", "Ok");
                return;
            }

            if (results == null || results.Length == 0)
            {
                EditorUtility.DisplayDialog("Create Asset Preview Controller Failed",
                    "Creation of Asset Preview Controller failed. Unable to locate prefab in project.", "Ok");
                return;
            }

            Object obj = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(results[0]));
            GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(obj);
            PrefabUtility.DisconnectPrefabInstance(go);

            AssetConfigurationController acc = go.GetComponentInChildren<AssetConfigurationController>();
            if (acc != null)
            {
                acc.SceneCamera = Camera.main;
            }

            Selection.activeGameObject = go;

        }
    }
}