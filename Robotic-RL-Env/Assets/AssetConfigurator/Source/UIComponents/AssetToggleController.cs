using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
namespace AssetConfigurator.Source.UIComponents
{
    public class AssetToggleController : MonoBehaviour
    {
        public RectTransform ButtonContainer;
        public Button ButtonPrefab;
        private List<Transform> AssetToggles = new List<Transform>();
        private List<Button> AssetToggleButtons = new List<Button>();
        
        
        
        
        public void SetAssetToggles(List<Transform> targets)
        {
            for (int i = 0; i < AssetToggleButtons.Count; i++)
            {
                Destroy(AssetToggleButtons[i].gameObject);
            }
            AssetToggleButtons.Clear();
            AssetToggles.Clear();
        
            AssetToggles.AddRange(targets);

            for (int i = 0; i < targets.Count; i++)
            {
                GameObject target = targets[i].gameObject;
                Button newButton = Instantiate(ButtonPrefab, ButtonContainer);
                newButton.gameObject.SetActive(true);
                Text buttonText = newButton.gameObject.GetComponentInChildren<Text>();
                string onOff = target.activeSelf == true ? "(on) " : "(off) ";
                buttonText.text = onOff + target.name;
                newButton.onClick.AddListener( () => ToggleSceneObject(target, newButton));
                AssetToggleButtons.Add(newButton);
            }
            
        }
        
        private void ToggleSceneObject(GameObject targetObject, Button controlButton)
        {
            targetObject.SetActive(!targetObject.activeSelf);
            Text buttonText = controlButton.gameObject.GetComponentInChildren<Text>();
            string onOff = targetObject.activeSelf == true ? "(on) " : "(off) ";
            buttonText.text = onOff + targetObject.name;
        }

    }
}