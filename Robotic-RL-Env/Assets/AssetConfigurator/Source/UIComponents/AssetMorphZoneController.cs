using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace AssetConfigurator.UIComponents
{
    public class AssetMorphZoneController:MonoBehaviour
    {
        public AssetMorphZoneUI MorphZoneControllerPrefab;
        public RectTransform ControllerContainer;

        private AssetConfigurationData Target;

        
        private int[] MorphSubmeshIndexs;
        private Dictionary<int, AssetMorphData[]> morphTable = new Dictionary<int, AssetMorphData[]>();

        public void SetTarget(AssetConfigurationData target)
        {
            morphTable.Clear();
            Target = target;


            
            MorphSubmeshIndexs = target.MorphOptions.Where(m => m.MorphType == MorphTargetTypes.SubMesh).Select(t => t.SubmeshID).Distinct().ToArray();
            
            
            for (int i = 0; i < MorphSubmeshIndexs.Length; i++)
            {
                int sMorphIndex = MorphSubmeshIndexs[i];
                AssetMorphData[] sMorphs = target.MorphOptions.Where(m => m.MorphType == MorphTargetTypes.SubMesh && m.SubmeshID == sMorphIndex).ToArray();
                if (sMorphs != null)
                {
                    if (sMorphs.Length > 0)
                    {
                        morphTable.Add(sMorphIndex, sMorphs);
                        List<string> morphNames = sMorphs.Select(m => m.TargetMesh.name).ToList();
                        GameObject tGO = GameObject.Instantiate(MorphZoneControllerPrefab.gameObject, ControllerContainer);
                        AssetMorphZoneUI subZoneController = tGO.GetComponent<AssetMorphZoneUI>();
                        subZoneController.txtMorphZone.text = target.SubmeshNames[sMorphIndex];
                        subZoneController.drpMorphOptions.AddOptions(morphNames);
                
                        subZoneController.MorphOptions = sMorphs;
                        tGO.SetActive(true);
                        subZoneController.MorphSlider.onValueChanged.AddListener((v) => handleMorphValueChanged(subZoneController, v));        
                    }
                }
                    
                
            }
            
            
        }

        private void handleMorphValueChanged(AssetMorphZoneUI controller, float value)
        {
            Debug.Log("0");
            controller.txtMorphSlider.text = value.ToString();
            Debug.Log("1");
            Target.ApplyMorph(controller.MorphOptions[controller.drpMorphOptions.value], value);
            Debug.Log("2");
        }


        public void HandleZoneOptionChanged()
        {
            
        }



    }
}