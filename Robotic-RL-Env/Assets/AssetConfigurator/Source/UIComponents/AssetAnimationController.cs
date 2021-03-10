using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace AssetConfigurator
{
    public class AssetAnimationController : MonoBehaviour
    {
        public Button AnimationButtonPrefab;
        public RectTransform AnimationButtonContainer;
        
        private AssetConfigurationData Target;
        private List<Button> animationButtons = new List<Button>();

        private void Awake()
        {
            
        }

        public void SetTarget(AssetConfigurationData target)
        {
            Target = target;

            for (int i = 0; i < animationButtons.Count; i++)
            {
                Destroy(animationButtons[i].gameObject);
            }
            animationButtons.Clear();

            if (target.AnimationOptions != null)
            {
                for (int i = 0; i < target.AnimationOptions.Count; i++)
                {
                    if (target.AnimationOptions[i].Animation == null)
                        continue;
                    
                    int animationID = i;
                    Button newButton = GameObject.Instantiate<Button>(AnimationButtonPrefab);
                    //newButton.name = TargetPreviewData.MaterialOptions[i].MaterialOption.name;
                    newButton.transform.SetParent(AnimationButtonContainer);
                    newButton.gameObject.SetActive(true);
                    newButton.onClick.AddListener(()=>Target.TargetAnimator.Play(Target.AnimationOptions[animationID].Animation.name));
                    newButton.GetComponentInChildren<Text>().text = target.AnimationOptions[animationID].Animation.name;
                    animationButtons.Add(newButton);
                }
            }
        }
    }
}