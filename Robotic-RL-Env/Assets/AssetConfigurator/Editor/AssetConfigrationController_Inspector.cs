using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.VersionControl;
using UnityEngine.SceneManagement;

namespace AssetConfigurator.EditorUI
{

    [CustomEditor(typeof(AssetConfigurationController))]
    public class AssetConfigrationController_Inspector: Editor
    {

        private Vector2 AssetListScrollPos = Vector2.zero;
        private Vector2 SceneObjectScrollPos = Vector2.zero;
        public static bool showGeneralSettings = true;
        public static bool showAssetList = false;
        public static bool showSceneList = false;
        public static bool showDefaultEditor = false;

        const string SettingsTooltip = "Settings\r\nThe basic configuration settings for Asset Configurator.";
        const string AssetListTooltip = "Asset List\r\nThis is a list of all the assets you wish to have available for preview.";
        const string SceneObjectTooltip = "Scene Objects\r\nThis is a list of scene objects you would like the character to be able to toggle on and off from the runtime UI.";

        private const string DefaultInspectorTooltip =
            "Default Inspector\r\nThe default inspector view. Used for advanced configuration of Asset Configurator";
        
        private void Awake()
        {
            AssetConfigurationController controller = (AssetConfigurationController) target;
            
            for (int i = (controller.AssetPreviewList.Count - 1); i >=0; i--)
            {
                if (controller.AssetPreviewList[i] == null)
                    controller.AssetPreviewList.RemoveAt(i);
                
                
            }
        }

        private GUIContent SettingsContent;
        

        public override void OnInspectorGUI()
        {
            if (EditorApplication.isPlaying)
            {
                GUI.backgroundColor = Color.red;
                GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Height(40) );
                GUILayout.FlexibleSpace();
                GUILayout.Label("PLAY MODE: CHANGES WILL NOT BE SAVED", EditorStyles.largeLabel);
                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();
                GUI.backgroundColor = Color.white;

            }
            GUILayout.Space(10);
            AssetConfigurationController controller = (AssetConfigurationController) target;

            GUILayout.BeginHorizontal(EditorStyles.helpBox);

            if (SettingsContent == null)
                SettingsContent = new GUIContent();
            
            SettingsContent.text = "Settings";
            SettingsContent.tooltip = SettingsTooltip;
            

            if (controller.SceneCamera == null)
                SettingsContent.image = EditorGUIUtility.IconContent("console.warnicon").image;
            else 
                SettingsContent.image = null;
                
            if (GUILayout.Button(SettingsContent, EditorStyles.toolbarButton))
            {
                showGeneralSettings = true;
                showAssetList = false;
                showSceneList = false;
                showDefaultEditor = false;
            }

            if (GUILayout.Button(new GUIContent("Asset List", AssetListTooltip), EditorStyles.toolbarButton))
            {
                showGeneralSettings = false;
                showAssetList = true;
                showSceneList = false;
                showDefaultEditor = false;
            }

            if (GUILayout.Button(new GUIContent("Scene Objects",SceneObjectTooltip), EditorStyles.toolbarButton))
            {
                showGeneralSettings = false;
                showAssetList = false;
                showSceneList = true;
                showDefaultEditor = false;
            }

            if (GUILayout.Button(new GUIContent("Default Editor", DefaultInspectorTooltip), EditorStyles.toolbarButton))
            {
                showGeneralSettings = false;
                showAssetList = false;
                showSceneList = false;
                showDefaultEditor = true;
            }

            string val = ActiveEditorTracker.sharedTracker.isLocked ? "Unlock" : "Lock ";
            if (ActiveEditorTracker.sharedTracker.isLocked)
                GUI.backgroundColor = Color.red;
            else
                GUI.backgroundColor = Color.yellow;
            
            if (GUILayout.Button(val, EditorStyles.toolbarButton))
            {
                ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
            }

                GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();

            if (showGeneralSettings)
            {
                GUILayout.Label("Settings", EditorStyles.centeredGreyMiniLabel);
                GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.BeginHorizontal();
                        GUILayout.Label("Scene Camera: ", GUILayout.Width(140));
                        controller.SceneCamera = (Camera) EditorGUILayout.ObjectField(controller.SceneCamera, typeof(Camera), true);
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                        GUILayout.Label("Asset Scene Parent: ", GUILayout.Width(140));
                        controller.AssetTargetParent = (Transform) EditorGUILayout.ObjectField(controller.AssetTargetParent, typeof(Transform), true);
                        GUILayout.Label("(optional)");
                
                    GUILayout.EndHorizontal();            
                    GUILayout.BeginHorizontal();
                        GUILayout.Label("Asset Start Location: ", GUILayout.Width(140));
                        controller.AssetStartLocation = EditorGUILayout.Vector3Field("", controller.AssetStartLocation);
                    GUILayout.EndHorizontal();

                GUILayout.EndVertical();
            }

            

            if (showAssetList)
            {
                GUILayout.Label("Asset List", EditorStyles.centeredGreyMiniLabel);
                bool checkAssets = false;
                GUILayout.BeginVertical(EditorStyles.helpBox);
                AssetListScrollPos = GUILayout.BeginScrollView(AssetListScrollPos, GUILayout.MinHeight(100), GUILayout.ExpandHeight(true));
                for (int i = 0; i < controller.AssetPreviewList.Count; i++)
                {
                    EditorGUI.BeginChangeCheck();
                    controller.AssetPreviewList[i] =
                        (AssetConfigurationData) EditorGUILayout.ObjectField(controller.AssetPreviewList[i],
                            typeof(AssetConfigurationData), true);
                    if (EditorGUI.EndChangeCheck())
                        checkAssets = true;

                }

                if (checkAssets == true)
                {
                    for (int i = (controller.AssetPreviewList.Count - 1); i >= 0; i--)
                    {
                        if (controller.AssetPreviewList[i] == null)
                            controller.AssetPreviewList.RemoveAt(i);
                    }
                }

                GUILayout.EndScrollView();

                GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Height(40));
                GUILayout.Label("Drag and Drop Addtional Assets Here\r\nTo add them to the list",
                    EditorStyles.centeredGreyMiniLabel);
                GUILayout.EndVertical();
                Rect dropArea = GUILayoutUtility.GetLastRect();

                if (dropArea.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.DragUpdated)
                        DragAndDrop.visualMode = DragAndDropVisualMode.Link;


                    if (Event.current.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        Object[] refs = DragAndDrop.objectReferences;
                        foreach (Object obj in refs)
                        {
                            if (obj.GetType() == typeof(GameObject))
                            {
                                
                                if (PrefabUtility.GetPrefabType(obj) == PrefabType.None)
                                {
                                    EditorUtility.DisplayDialog("Unable To Add Asset",
                                        "Unable To Add Asset " + obj.name + ". Assets must be added from the project folder, not the scene.", "Ok");
                                    continue;
                                }
                                else
                                {
                                    if (PrefabUtility.GetPrefabParent(obj) == null)
                                    {
                                        controller.sceneObjectController.ToggleableObjects.Add((GameObject) obj);    
                                    }
                                    else
                                    {
                                        EditorUtility.DisplayDialog("Unable To Add Asset",
                                            "Unable To Add Asset " + obj.name + ". Assets must be added from the project folder, not the scene.", "Ok");
                                        continue;
                                    }
                                }
                                
                                
                                GameObject go = (GameObject) obj;
                                AssetConfigurationData acd = go.GetComponent<AssetConfigurationData>();

                                if (acd != null)
                                {
                                    controller.AssetPreviewList.Add(acd);
                                }
                                else
                                {
                                    

                                    
                                    
                                    SkinnedMeshRenderer smr = go.GetComponent<SkinnedMeshRenderer>();
                                    if (smr == null)
                                        smr = go.GetComponentInChildren<SkinnedMeshRenderer>();

                                    if (smr == null)
                                    {
                                        EditorUtility.DisplayDialog("Invalid Asset", "This asset does not contain a skinned mesh rendered, and this can not be automatically configured currently. Please manually configure this asset, then add it.", "Ok");
                                    }
                                    else
                                    {
                                        if (EditorUtility.DisplayDialog("Add Asset Configuration Data?", "This asset does not contain any configuration data, which is required. Should the component be added?", "Indeed", "Negative"))
                                        {
                                            go.AddComponent<AssetConfigurationData>();
                                            acd = go.GetComponent<AssetConfigurationData>();
                                            acd.meshType = MeshType.SkinnedMesh;
                                            Animator anim = go.GetComponent < Animator>();
                                            if (anim != null)
                                                acd.TargetAnimator = anim;
                                            
                                            controller.AssetPreviewList.Add(acd);
                                            
                                            

                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                GUILayout.EndVertical();
            }



            if (showSceneList)
            {
                GUILayout.Label("Scene Objects", EditorStyles.centeredGreyMiniLabel);
                GUILayout.BeginVertical(EditorStyles.helpBox);
                SceneObjectScrollPos = GUILayout.BeginScrollView(SceneObjectScrollPos, GUILayout.MinHeight(100));
                if (controller.sceneObjectController != null)
                {
                    List <GameObject> gos = controller.sceneObjectController.ToggleableObjects;
                    for (int i = 0; i < gos.Count; i++)
                    {
                        gos[i] = (GameObject)EditorGUILayout.ObjectField(gos[i], typeof(GameObject), true);
                    }
                    for (int i = (gos.Count-1); i >=0; i--)
                        if (gos[i] == null)
                            gos.RemoveAt(i);
                }
                GUILayout.EndScrollView();
                
                
                
                GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Height(40));
                GUILayout.Label("Drag and Drop Addtional Assets Here\r\nTo add them to the list",
                    EditorStyles.centeredGreyMiniLabel);
                GUILayout.EndVertical();
                Rect dropArea = GUILayoutUtility.GetLastRect();

                if (dropArea.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.DragUpdated)
                        DragAndDrop.visualMode = DragAndDropVisualMode.Link;


                    if (Event.current.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        Object[] refs = DragAndDrop.objectReferences;
                        foreach (Object obj in refs)
                        {
                            if (obj.GetType() == typeof(GameObject))
                            {
  
                                        controller.sceneObjectController.ToggleableObjects.Add((GameObject) obj);    
                              }
                        }
                    }
                }

                GUILayout.EndVertical();
            }


            
            if (showDefaultEditor)
                DrawDefaultInspector();
            
            GUILayout.Space(10);
        }
        
        
        
    }
}