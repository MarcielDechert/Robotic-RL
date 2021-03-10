using UnityEngine;
using System.Collections.Generic;
using AssetConfigurator.DataContainers;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;

namespace AssetConfigurator
{


    [CustomEditor(typeof(AssetConfigurationData))]
    public class AssetConfigurationData_Inspector : Editor
    {
        
        
        /*
         * New option fields.
         */


        
        /*
         * Scrollview position fields
         */
        private List<Vector2> scrollPositions = new List<Vector2>();
        private Vector2 _animactionScrollPos = Vector2.zero;

        private Vector2 ToggleObjectPos = Vector2.zero;
        
        /*
         * Indexs for option removal.
         */
        private int removeAnimationIndex = -1;
        private int removeMaterialIndex = -1;

        private int removeBlendIndex = -1;
        /*
         * Section display toggle fields
         */
        private bool showGeneralSettings = true;
        private bool showAnimations = false;
        private bool showMaterialOptions = false;
        private bool showSubmeshNames = false;
        private bool showMorphTargets = false;
        private bool showToggleObjects = false;
        
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
            Material[] mats = null;
            int subMeshCount = 0;
            AssetConfigurationData myTarget = (AssetConfigurationData)target;

            /*
             * General Settings
             */
            
            GUILayout.Space(5);
            GUI.backgroundColor = Color.gray;
            if (GUILayout.Button("General Settings", EditorStyles.toolbarButton))
            {
                showGeneralSettings = !showGeneralSettings;
            }
            GUI.backgroundColor = Color.white;
            

            GUILayout.BeginHorizontal();
            GUILayout.Label("Mesh Renderer Type: ");
            myTarget.meshType = (MeshType)EditorGUILayout.EnumPopup(myTarget.meshType);
            GUILayout.EndHorizontal();
            if (myTarget.meshType == MeshType.None)
            {
                myTarget.MaterialOptions.Clear();
                return;
            }

            if (myTarget.meshType == MeshType.SkinnedMesh)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Target Renderer");
                EditorGUI.BeginChangeCheck();
                myTarget.skinnedMeshRenderer = (SkinnedMeshRenderer)EditorGUILayout.ObjectField(myTarget.skinnedMeshRenderer, typeof(SkinnedMeshRenderer), true);
                if (EditorGUI.EndChangeCheck())
                {
                    HandleRendererChanged(myTarget);
                }
                GUILayout.EndHorizontal();
                
                if (myTarget.skinnedMeshRenderer != null)
                {
                    mats = myTarget.skinnedMeshRenderer.sharedMaterials;
                    if (mats != null)
                    {
                        for (int i = 0; i < mats.Length; i++)
                        {
                            if (mats[i] == null)
                                continue;
                            if (myTarget.ContainsMaterialOption(i, mats[i]) == false)
                            {
                                myTarget.MaterialOptions.Insert(0,new MaterialOptionData(i, mats[i]));
                                
                            }
                        }
                    }
                }
                else
                {
                 GUILayout.Label("Please select a Skinned Mesh Renderer to continue");   
                }
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Target Renderer");
                myTarget.meshRenderer = (MeshRenderer)EditorGUILayout.ObjectField(myTarget.meshRenderer, typeof(MeshRenderer), true);
                GUILayout.EndHorizontal();
                if (myTarget.meshRenderer != null)
                {
                    mats = myTarget.meshRenderer.sharedMaterials;
                }
                else
                {
                    GUILayout.Label("Please select a Mesh Renderer to continue");   
                }
                
            }
            if (mats != null)
                subMeshCount = mats.Length;

            GUILayout.BeginHorizontal();
                GUILayout.Label("Camera Look Position: ");
                myTarget.CameraLookPosition = EditorGUILayout.Vector3Field("",myTarget.CameraLookPosition);
            GUILayout.EndHorizontal();
        
            /*
             * Toggle Objects
             */
            
            GUILayout.Space(5);
            GUI.backgroundColor = Color.gray;
            if (GUILayout.Button("Toggle Objects", EditorStyles.toolbarButton))
            {
                showToggleObjects = !showToggleObjects;
            }
            GUI.backgroundColor = Color.white;

            if (showToggleObjects)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
                ToggleObjectPos = GUILayout.BeginScrollView(ToggleObjectPos, GUILayout.MinHeight(100));
                for (int i = 0; i < myTarget.ToggleObjects.Count; i++)
                {
                    myTarget.ToggleObjects[i] = (Transform)EditorGUILayout.ObjectField(myTarget.ToggleObjects[i], typeof(Transform), true);
                    
                        
                }
                GUILayout.EndScrollView();
                GUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.Height(40));
                GUILayout.Label("Drag and Drop Toggle-able Objects here\r\nto add them to the list", EditorStyles.centeredGreyMiniLabel, GUILayout.Height(40));
                GUILayout.EndHorizontal();
                Rect morphDropArea = GUILayoutUtility.GetLastRect();

                if (morphDropArea.Contains(Event.current.mousePosition))
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
                                GameObject go = (GameObject) obj;
                                if (go.transform.IsChildOf(myTarget.transform))
                                {
                                    myTarget.ToggleObjects.Add(go.transform);
                                }
                            }
                        }
                    }
                }

                GUILayout.EndVertical();

                for (int i = myTarget.ToggleObjects.Count - 1; i >= 0; i--)
                {
                    if (myTarget.ToggleObjects[i] == null)
                        myTarget.ToggleObjects.RemoveAt(i);
                }
                if (GUILayout.Button("Clear Toggle List"))
                {
                    myTarget.ToggleObjects.Clear();
                }
            }
            
            /*
             * Animations Panel
             */
            #region Animation Panel
            
            
            GUILayout.Space(5);
            GUI.backgroundColor = Color.gray;
            if (GUILayout.Button("Animations", EditorStyles.toolbarButton))
            {
                showAnimations = !showAnimations;
            }
            GUI.backgroundColor = Color.white;

            if (showAnimations)
            {

                EditorGUI.BeginChangeCheck();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Animator:");
                myTarget.TargetAnimator = (Animator) EditorGUILayout.ObjectField(myTarget.TargetAnimator, typeof(Animator), true);
                GUILayout.EndHorizontal();

                if (EditorGUI.EndChangeCheck())
                {
                    if (myTarget.TargetAnimator == null)
                    {
                        myTarget.AnimationOptions.Clear();
                    }
                    else
                    {

                        if (myTarget.TargetAnimator.runtimeAnimatorController != null)
                        {

                            AnimationClip[] clips = myTarget.TargetAnimator.runtimeAnimatorController.animationClips;
                            for (int i = 0; i < clips.Length; i++)
                                myTarget.AnimationOptions.Add(new AnimationPreviewData(clips[i]));
                        }
                    }
                }


                GUILayout.BeginHorizontal();
                    GUILayout.Label("Default Animation");
                    myTarget.DefaultAnimation = (AnimationClip) EditorGUILayout.ObjectField(myTarget.DefaultAnimation, typeof(AnimationClip), true);
                GUILayout.EndHorizontal();


                /* Display Current Animation List */
                if (myTarget.AnimationOptions != null)
                {

                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUI.backgroundColor = Color.gray;
                    GUILayout.BeginHorizontal(EditorStyles.helpBox);
                    GUILayout.Label("Animation Name", EditorStyles.miniBoldLabel, GUILayout.Width(100));
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("Remove", EditorStyles.miniBoldLabel);
                    GUILayout.EndHorizontal();
                    GUI.backgroundColor = Color.white;
                    _animactionScrollPos = GUILayout.BeginScrollView(_animactionScrollPos, GUILayout.MinHeight(100));
                    for (int i = 0; i < myTarget.AnimationOptions.Count; i++)
                    {
                        if (myTarget.AnimationOptions[i].Animation == null)
                            continue;

                        GUILayout.BeginHorizontal();

                        GUILayout.Label(myTarget.AnimationOptions[i].Animation.name);
                        //myTarget.AnimationOptions[i].Loop = GUILayout.Toggle(myTarget.AnimationOptions[i].Loop, "", GUILayout.Width(40));
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("X", GUILayout.Width(24)))
                        {
                            removeAnimationIndex = i;
                        }

                        GUILayout.EndHorizontal();
                    }

                    GUILayout.EndScrollView();
                    GUILayout.EndVertical();
                }


                if (removeAnimationIndex != -1)
                {
                    myTarget.AnimationOptions.RemoveAt(removeAnimationIndex);
                    removeAnimationIndex = -1;
                    return;
                }

                /* Drag And Drop Animation Zone & Handler */
                 
                                 GUILayout.BeginHorizontal(EditorStyles.helpBox);
                                 GUILayout.Label("Drag and Drop AnimationOptions here to add them.", EditorStyles.centeredGreyMiniLabel, GUILayout.Height(40));
                                 GUILayout.EndHorizontal();
                                 Rect dropArea = GUILayoutUtility.GetLastRect();
                 
                 
                                 if (dropArea.Contains(Event.current.mousePosition))
                                 {
                                     if (Event.current.type == EventType.DragUpdated)
                                         DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                 
                 
                                     if (Event.current.type == EventType.DragPerform)
                                     {
                                         DragAndDrop.AcceptDrag();
                                         Object[] refs = DragAndDrop.objectReferences;
                                         if (refs.Length > 0)
                                         {
                                             for (int i = 0; i < refs.Length; i++)
                                             {
                                                 if (refs[i].GetType() == typeof(AnimationClip))
                                                 {
                                                     AddAnimationToTarget(myTarget, (AnimationClip) refs[i]);
                                                     Event.current.Use();
                                                 }
                                                 else
                                                 {
                                                     if (refs[i].GetType() == typeof(GameObject))
                                                     {
                                                         AnimationClip[] clips = AnimationUtility.GetAnimationClips((GameObject) refs[i]);
                                                         if (clips.Length > 0)
                                                         {
                                                             for (int n = 0; n < clips.Length; n++)
                                                             {
                                                                 AddAnimationToTarget(myTarget, clips[i]);
                                                             }
                                                             Event.current.Use();
                                                         }
                                                         else
                                                         {
                                                             
                                                             GameObject go = (GameObject)refs[i];
                                                             string path = AssetDatabase.GetAssetPath(go);
                                                             Object[] objs = AssetDatabase.LoadAllAssetsAtPath(path);
                                                             foreach(Object obj in objs)
                                                                 if (obj.GetType() == typeof(AnimationClip))
                                                                     AddAnimationToTarget(myTarget, (AnimationClip)obj);
                                                             
                                                             
                                                             


                                                         }
                                                     }
                                                 }
                                             }
                                         }
                 
                                         
                 
                                     }
                                 }

                if (GUILayout.Button("Generate Mecanim Controller"))
                {
                    GenerateController(myTarget);
                }
                
                if (GUILayout.Button("Clear Animation List"))
                {
                    myTarget.AnimationOptions.Clear();
                }


            }
            #endregion
            
            
            if (subMeshCount <= 0)
                return;

            if (myTarget.SubmeshNames.Count > subMeshCount)
            {
                while (myTarget.SubmeshNames.Count > subMeshCount)
                    myTarget.SubmeshNames.RemoveAt(myTarget.SubmeshNames.Count - 1);
                
            }else if (myTarget.SubmeshNames.Count < subMeshCount)
            {
                while (myTarget.SubmeshNames.Count < subMeshCount)
                    myTarget.SubmeshNames.Add("Submesh " + myTarget.SubmeshNames.Count);
            }


            for (int i = (myTarget.MaterialOptions.Count - 1); i >= 0; i--)
            {
                if (myTarget.MaterialOptions[i].MaterialOption == null)
                    myTarget.MaterialOptions.RemoveAt(i);
            }

            
            /*
             * Mecanim Panel
             */
            /*
            GUILayout.Space(5);
            GUI.backgroundColor = Color.gray;
            GUILayout.BeginVertical(EditorStyles.helpBox);
            if (GUILayout.Button("Mecanim Controllers", EditorStyles.miniBoldLabel))
            {
                showMecanimControllers = !showMecanimControllers;
            }
            GUILayout.EndVertical();
            GUI.backgroundColor = Color.white;
            
            
            if (showMecanimControllers)
            {
                for (int i = 0; i < myTarget.MecanimControllers.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(myTarget.MecanimControllers[i].name);
                    myTarget.SubmeshNames[i] = GUILayout.TextField(myTarget.SubmeshNames[i]);
                    GUILayout.EndHorizontal();
                }
                
                GUILayout.BeginVertical();
                GUILayout.Label("Add Controller", EditorStyles.centeredGreyMiniLabel);
                _newController = (RuntimeAnimatorController) EditorGUILayout.ObjectField(_newController, typeof(RuntimeAnimatorController));
                if (GUILayout.Button("Add Material Option"))
                {
                    if (newMaterial != null)
                    {
                        myTarget.MecanimControllers.Add(_newController);
                        newMaterial = null;
                    }
                }
                GUILayout.EndVertical();
                
            }*/
            
            
            /*
             * Submesh Names
             */
            
            #region SubmeshName Panel
            
            GUILayout.Space(5);
            GUI.backgroundColor = Color.gray;
            if (GUILayout.Button("Submesh Names", EditorStyles.toolbarButton))
            {
                showSubmeshNames = !showSubmeshNames;
            }

            GUI.backgroundColor = Color.white;
            if (showSubmeshNames)
            {
                for (int i = 0; i < myTarget.SubmeshNames.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Submesh " + i );
                    myTarget.SubmeshNames[i] = GUILayout.TextField(myTarget.SubmeshNames[i]);
                    GUILayout.EndHorizontal();
                }
            }
            
            #endregion

            /*
             * Material Options Panel
             */
            
            #region Material Options Panel
            
            GUILayout.Space(5);
            GUI.backgroundColor = Color.gray;

            if (GUILayout.Button("Material Options", EditorStyles.toolbarButton))
            {
                showMaterialOptions = !showMaterialOptions;
            }
            GUI.backgroundColor = Color.white;
            if (showMaterialOptions)
            {
                

                for (int i = 0; i < subMeshCount; i++)
                {
                    while (scrollPositions.Count < (i + 1))
                        scrollPositions.Add(Vector2.zero);


                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Label(myTarget.SubmeshNames[i] + " (Submesh " + i + ")", EditorStyles.boldLabel);
                    scrollPositions[i] = GUILayout.BeginScrollView(scrollPositions[i], GUILayout.MinHeight(100));

                    for (int n = 0; n < myTarget.MaterialOptions.Count; n++)
                    {
                        if (myTarget.MaterialOptions[n].SubMeshID == i)
                        {
                            GUILayout.BeginHorizontal();

                            if (myTarget.MaterialOptions[n].MaterialOption != null)
                            {
                                GUILayout.BeginHorizontal();
                                GUILayout.Label(myTarget.MaterialOptions[n].MaterialOption.name);
                                if (GUILayout.Button("X", GUILayout.Width(24)))
                                {
                                    removeMaterialIndex = n;
                                }

                                GUILayout.EndHorizontal();
                            }




                            GUILayout.EndHorizontal();
                        }
                    }

                    GUILayout.EndScrollView();

                    if (removeMaterialIndex != -1)
                    {
                        myTarget.MaterialOptions.RemoveAt(removeMaterialIndex);
                        removeMaterialIndex = -1;
                    }
                    
                    /* Drag And Drop Material Zone & Handler */
                 
                    GUILayout.BeginHorizontal(EditorStyles.helpBox);
                    GUILayout.Label("Drag and Drop Additional materials here to add them.", EditorStyles.centeredGreyMiniLabel, GUILayout.Height(40));
                    GUILayout.EndHorizontal();
                    Rect dropArea = GUILayoutUtility.GetLastRect();
                    
                    
                    if (dropArea.Contains(Event.current.mousePosition))
                    {
                        if (Event.current.type == EventType.DragUpdated)
                            DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                    
                    
                        if (Event.current.type == EventType.DragPerform)
                        {
                             DragAndDrop.AcceptDrag();
                             Object[] refs = DragAndDrop.objectReferences;
                             if (refs.Length > 0)
                             {
                                 for (int n = 0; n < refs.Length; n++)
                                 {
                                     if (refs[n].GetType() == typeof(Material))
                                     {
                                        myTarget.MaterialOptions.Add(new MaterialOptionData(i, (Material)refs[n]));                                         
                                         Event.current.Use();
                                     }
                                 }
                             }
                        }
                    }
                    
                    GUILayout.EndVertical();

                    
                }
            }
            
            #endregion
            
            
            GUILayout.Space(5);
            GUI.backgroundColor = Color.gray;
            if (GUILayout.Button("Blend Shapes", EditorStyles.toolbarButton))
            {
                showMorphTargets = !showMorphTargets;
            }
            GUI.backgroundColor = Color.white;

            if (showMorphTargets)
            {
                for (int i = 0; i < myTarget.BlendShapeData.Count; i++)
                {
                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Label(myTarget.BlendShapeData[i].Name);
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Display Name: ");
                    myTarget.BlendShapeData[i].DisplayName = GUILayout.TextField(myTarget.BlendShapeData[i].DisplayName);
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Min: ");
                    myTarget.BlendShapeData[i].MinValue = EditorGUILayout.FloatField(myTarget.BlendShapeData[i].MinValue);
                    GUILayout.Label("Max");
                    myTarget.BlendShapeData[i].MaxValue = EditorGUILayout.FloatField(myTarget.BlendShapeData[i].MaxValue);
                    if (GUILayout.Button("X", GUILayout.Width(24)))
                    {
                        removeBlendIndex = i;
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                }

                if (removeBlendIndex != -1)
                {
                    myTarget.BlendShapeData.RemoveAt(removeBlendIndex);
                    removeBlendIndex = -1;
                }
                
                if (GUILayout.Button("Clear Blend Shapes List"))
                {
                    RefreshBlendShapes(myTarget);
                }
/*
                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUI.backgroundColor = Color.gray;
                GUILayout.BeginHorizontal(EditorStyles.helpBox);
                GUILayout.Label("Mesh", GUILayout.Width(120));
                GUILayout.Label("Target Type", GUILayout.Width(80));
                GUILayout.Label("Submesh", GUILayout.Width(80));
                GUILayout.FlexibleSpace();
                GUILayout.Label("Remove");
                GUILayout.EndHorizontal();
                GUI.backgroundColor = Color.white;
                _morphScrollPos = GUILayout.BeginScrollView(_morphScrollPos, GUILayout.Height(100));
                for (int i = 0; i < myTarget.MorphOptions.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(myTarget.MorphOptions[i].TargetMesh.name, GUILayout.Width(120));
                    myTarget.MorphOptions[i].MorphType =
                        (MorphTargetTypes) EditorGUILayout.EnumPopup(myTarget.MorphOptions[i].MorphType,
                            GUILayout.Width(80));
                    if (myTarget.MorphOptions[i].MorphType == MorphTargetTypes.SubMesh)
                    {
                        myTarget.MorphOptions[i].SubmeshID =
                            EditorGUILayout.IntField(myTarget.MorphOptions[i].SubmeshID, GUILayout.Width(80));
                    }

                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("X", GUILayout.Width(24)))
                    {
                        removeMorphIndex = i;
                    }

                    GUILayout.EndHorizontal();
                }

                GUILayout.EndScrollView();
                GUILayout.EndVertical();

                if (removeMorphIndex != -1)
                {
                    myTarget.MorphOptions.RemoveAt(removeMorphIndex);
                    removeMorphIndex = -1;
                }
                */

                /* Drag And Drop Morph Zone & Handler */
/*
                GUILayout.BeginHorizontal(EditorStyles.helpBox);
                GUILayout.Label("Drag and Drop Morph Meshes here to add them.");
                GUILayout.EndHorizontal();
                Rect morphDropArea = GUILayoutUtility.GetLastRect();

                if (morphDropArea.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.DragUpdated)
                        DragAndDrop.visualMode = DragAndDropVisualMode.Link;


                    if (Event.current.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        Object[] refs = DragAndDrop.objectReferences;
                        if (refs.Length > 0)
                        {
                            for (int n = 0; n < refs.Length; n++)
                            {
                                if (refs[n].GetType() == typeof(Mesh))
                                {
                                    myTarget.MorphOptions.Add(new AssetMorphData((Mesh) refs[n]));
                                    Event.current.Use();
                                }
                            }
                        }
                    }
                }*/
            }

            GUILayout.Space(5);
            GUI.backgroundColor = Color.gray;
            if (GUILayout.Button("Controls", EditorStyles.toolbarButton))
            {
             
            }
            GUI.backgroundColor = Color.white;
            
            if (GUILayout.Button("Rebuild Blend Shape List"))
            {
                RefreshBlendShapes(myTarget);
            }
            
            UnityEngine.Object prefabParent = PrefabUtility.GetPrefabParent(myTarget.gameObject);
            if (prefabParent != null)
            {
                if (GUILayout.Button("Apply Changes to Prefab"))
                {
                    GameObject gameObject = PrefabUtility.FindValidUploadPrefabInstanceRoot(myTarget.gameObject);
                    if (gameObject != null)
                    {
                        PrefabUtility.ReplacePrefab(gameObject, prefabParent, ReplacePrefabOptions.ConnectToPrefab);
                    }
                }
            }






        }

        private void RefreshBlendShapes(AssetConfigurationData target)
        {
            target.BlendShapeData.Clear();
            if (target.skinnedMeshRenderer != null)
            {
                Mesh sharedMesh = target.skinnedMeshRenderer.sharedMesh;
                int blendCount = sharedMesh.blendShapeCount;
                for (int i = 0; i < blendCount; i++)
                {
                    string blendName = sharedMesh.GetBlendShapeName(i);
                   
                    target.BlendShapeData.Add(new AssetBlendShapeData(i, blendName));
                }

            }
        }

        public void HandleRendererChanged(AssetConfigurationData target)
        {

            
            RefreshBlendShapes(target);

        }

        private void GenerateController(AssetConfigurationData target)
        {

            if (target.TargetAnimator == null)
                return;

            target.TargetAnimator.runtimeAnimatorController = null;

            string path = EditorUtility.SaveFilePanelInProject("Mecanim Controller Save Location", "NewController", "controller", "Message?");
            if (string.IsNullOrEmpty(path))
            {
                EditorUtility.DisplayDialog("Generation Canceled",
                    "You must select a valid path for the generator. Generation Cancelled.", "Ok");
                return;
            }
            var controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath(path);
            
            controller.AddParameter("Loop", AnimatorControllerParameterType.Bool);
            
            var rootStateMachine = controller.layers[0].stateMachine;
            var defaultState = rootStateMachine.AddState("default");
            defaultState.motion = target.DefaultAnimation;
            
            
            for (int i = 0; i < target.AnimationOptions.Count; i++)
            {
                if (target.AnimationOptions[i].Animation == null)
                    continue;
                
                var state = rootStateMachine.AddState(target.AnimationOptions[i].Animation.name);
                state.motion = target.AnimationOptions[i].Animation;
                var entranceTransition = defaultState.AddTransition(state);
                entranceTransition.AddCondition(AnimatorConditionMode.If,0, target.AnimationOptions[i].Animation.name);
                
                /*Add Exit transition from this state back to default state*/
                var exittransition = state.AddTransition(defaultState);
                exittransition.AddCondition(AnimatorConditionMode.IfNot, 0, "Loop");
                exittransition.exitTime = 1;
                exittransition.hasExitTime = true;

                var looptrans = state.AddTransition(state);
                looptrans.AddCondition(AnimatorConditionMode.If, 1, "Loop");
                looptrans.exitTime = 1;
                looptrans.hasExitTime = true;
                
                var anyTrans = rootStateMachine.AddAnyStateTransition(state);
                /* Adds a transition from any state to this one if this states trigger is toggled */
                anyTrans.AddCondition(AnimatorConditionMode.If, 1, target.AnimationOptions[i].Animation.name);
                anyTrans.exitTime = 1;
                anyTrans.hasExitTime = true;
                
                
                controller.AddParameter(target.AnimationOptions[i].Animation.name, AnimatorControllerParameterType.Trigger);
                

                
            }

            target.TargetAnimator.runtimeAnimatorController = controller;



        }

        private void AddAnimationToTarget(AssetConfigurationData target, AnimationClip clip)
        {

            if (clip.name.StartsWith("__preview"))
                return;
            
            
         
            for (int i = 0; i < target.AnimationOptions.Count; i++)
            {
                if (target.AnimationOptions[i].Animation == clip)
                {
                    EditorUtility.DisplayDialog("Animation Not Added", target.AnimationOptions[i].Animation.name + " is a duplicate AnimationOptions and was not added.", "Cool");
                    return;
                }
                
            }

            if (clip.name.ToLower().IndexOf("idle") > -1)
            {
                if (EditorUtility.DisplayDialog("Set default Animation", clip.name +  "\r\nIt looks like this may be an idle animation. Would you like to set it as the default animation?", "Yes", "No"))
                {
                    target.DefaultAnimation = clip;
                    return;
                }
            }
            
            target.AnimationOptions.Add(new AnimationPreviewData(clip));
            
        }
    }
}