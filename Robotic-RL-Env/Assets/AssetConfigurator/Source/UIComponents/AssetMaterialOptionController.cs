using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;

namespace AssetConfigurator
{
    [System.Serializable]
    public class AssetMaterialEvent : UnityEvent<int, Material>
    {
    }
    
    public class AssetMaterialOptionController:MonoBehaviour
    {
        public RectTransform ButtonContainer;
        public Button MaterialButtonPrefab;
        public Text SubmeshText;
        public AssetMaterialEvent OnMaterialClicked;
        
        
        private AssetConfigurationData _targetConfigurationData;
        
        private int SubMeshCount = 0;
        private int ActiveSubmeshID = 0;
        
        private List<Button> materialButtons = new List<Button>();
        
        public void SetTarget(AssetConfigurationData target)
        {
            _targetConfigurationData = target;

            if (_targetConfigurationData == null)
                return;

            SubMeshCount = _targetConfigurationData.GetSubmeshCount();
            
            DisplayActiveMaterialButtons();
        }


        
        public void DisplayActiveMaterialButtons()
        {
            for (int i = 0; i <materialButtons.Count; i++)
            {
                Destroy(materialButtons[i].gameObject);
            }
            
            materialButtons.Clear();
            
            for (int i = 0; i < _targetConfigurationData.MaterialOptions.Count; i++)
            {
                if (_targetConfigurationData.MaterialOptions[i].SubMeshID == ActiveSubmeshID)
                {
                    if (_targetConfigurationData.MaterialOptions[i].MaterialOption == null)
                    {
                        Debug.Log("Unity is a dumb cunt");
                        
                    }
                    else
                    {
                        Material material = _targetConfigurationData.MaterialOptions[i].MaterialOption; 
                        Button newButton = GameObject.Instantiate<Button>(MaterialButtonPrefab);
                        //newButton.name = _targetConfigurationData.MaterialOptions[i].MaterialOption.name;
                        newButton.transform.SetParent(ButtonContainer);
                        newButton.onClick.AddListener(()=>SwitchMaterials(material));
                        newButton.gameObject.SetActive(true);
                        newButton.GetComponentInChildren<Text>().text = material.name;
                        materialButtons.Add(newButton);
                    }

                }
            }

            
            SubmeshText.text = _targetConfigurationData.SubmeshNames[ActiveSubmeshID];
        }

        private void SwitchMaterials(Material mat)
        {
            OnMaterialClicked.Invoke(ActiveSubmeshID, mat);
        }

        public void NextSubmesh()
        {
            ActiveSubmeshID++;
            if (ActiveSubmeshID >= SubMeshCount)
                ActiveSubmeshID = 0;

            DisplayActiveMaterialButtons();
        }

        public void PreviousSubMesh()
        {
            ActiveSubmeshID--;
            if (ActiveSubmeshID < 0)
                ActiveSubmeshID = SubMeshCount -1;

            DisplayActiveMaterialButtons();
        }

    }
}