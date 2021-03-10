using UnityEngine;
using System.Collections.Generic;

using UnityEngine.UI;

public class SceneObjectController : MonoBehaviour
{

    public RectTransform ButtonContainer;
    public Button ButtonPrefab;
    public List<GameObject> ToggleableObjects = new List<GameObject>();
    

    private void Awake()
    {
        for (int i = 0; i < ToggleableObjects.Count; i++)
        {
            GameObject target = ToggleableObjects[i];
            if (target == null)
                continue;
            
            Button newButton = Instantiate(ButtonPrefab, ButtonContainer);
            newButton.gameObject.SetActive(true);
            Text buttonText = newButton.gameObject.GetComponentInChildren<Text>();
            string onOff = target.activeSelf == true ? "(on) " : "(off) ";
            buttonText.text = onOff + target.name;
            newButton.onClick.AddListener( () => ToggleSceneObject(target, newButton));
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
