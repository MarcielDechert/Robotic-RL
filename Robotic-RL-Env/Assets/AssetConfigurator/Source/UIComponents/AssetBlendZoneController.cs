using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using AssetConfigurator.DataContainers;

namespace AssetConfigurator.UIComponents
{
    public class AssetBlendZoneController:MonoBehaviour
    {
        public AssetBlendZoneUI BlendZoneControllerPrefab;
        public RectTransform ControllerContainer;

        private AssetConfigurationData Target;
        private AssetBlendShapeData[] blendShapes = null;
        
        private List<AssetBlendZoneUI> activeBlendZoneControls = new List<AssetBlendZoneUI>();

        public void SetTarget(AssetConfigurationData target)
        {


            for (int i = 0; i < activeBlendZoneControls.Count; i++)
            {
                Destroy(activeBlendZoneControls[i].gameObject);
            }
            activeBlendZoneControls.Clear();
            
            Target = target;


            blendShapes = target.BlendShapeData.ToArray();
            for (int i = 0; i < blendShapes.Length; i++)
            {
                AssetBlendShapeData blendData = blendShapes[i];
                

                GameObject tGO = GameObject.Instantiate(BlendZoneControllerPrefab.gameObject, ControllerContainer);
                AssetBlendZoneUI subZoneController = tGO.GetComponent<AssetBlendZoneUI>();
                subZoneController.BlendData = blendData;
                subZoneController.txtBlendZone.text = blendData.DisplayName;
                tGO.SetActive(true);
                subZoneController.BlendSlider.minValue = blendData.MinValue;
                subZoneController.BlendSlider.maxValue = blendData.MaxValue;
                
                subZoneController.BlendSlider.onValueChanged.AddListener((v) => handleBlendValueChanged(subZoneController, v));
                activeBlendZoneControls.Add(subZoneController);
                
            }
            
            
        }

        private void handleBlendValueChanged(AssetBlendZoneUI controller, float value)
        {
            
            controller.txtBlendSlider.text = value.ToString();
            RuntimeAnimatorController rac = Target.TargetAnimator.runtimeAnimatorController;
            Target.TargetAnimator.runtimeAnimatorController = null;
            Target.skinnedMeshRenderer.SetBlendShapeWeight(controller.BlendData.Index, value);
            Debug.Log("Set Index: " + controller.BlendData.Index + " to Value: " + value);
            Target.TargetAnimator.runtimeAnimatorController = rac;

        }


        public void HandleZoneOptionChanged()
        {
            
        }



    }
}