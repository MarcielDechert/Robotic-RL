using AssetConfigurator;
using UnityEngine;
using System.Collections.Generic;
using AssetConfigurator.Source.UIComponents;
using AssetConfigurator.UIComponents;
using UnityEngine.EventSystems;

using UnityEngine.UI;

public class AssetConfigurationController : MonoBehaviour
{
    public Camera SceneCamera;
    public Text txtActiveAssetName;
    
    public SceneObjectController sceneObjectController;
    public AssetMaterialOptionController MaterialOptionController;
    public AssetAnimationController AnimationController;
    public AssetMorphZoneController MorphZoneController;
    public AssetBlendZoneController BlendZoneController;
    public AssetToggleController ToggleController;
    public ControllerSettingsUI SettingsWindow;

    public RectTransform HelpWindow;
    public Transform AssetTargetParent;
    public Vector3 AssetStartLocation = Vector3.zero;
    public bool IsTurnTableActive = false;
    public List<AssetConfigurationData> AssetPreviewList = new List<AssetConfigurationData>();

    private int ActiveAssetID = 0;
    private Transform ActiveTransform = null;
    private GameObject ActiveGameObject = null;
    private AssetConfigurationData _activeConfigurationData = null;
    
    private float _TurntableSpeed = 4f;
    private float _MouseSensativity = 10f;
    private float _MouseZoomFactor = 10f;
    private float _KeyboardRotationSpeed = 5f;
    
    private bool _IsMouseOverUI = false;

    public bool IsMouseOverUI
    {
        get { return _IsMouseOverUI; }
        set { _IsMouseOverUI = value; }
    }

    public float TurntableSpeed
    {
        get { return _TurntableSpeed; }
        set { _TurntableSpeed = value; }
    }

    public float MouseSensativity
    {
        get { return _MouseSensativity; }
        set { _MouseSensativity = value; }
    }

    public float MouseZoomFactor
    {
        get { return _MouseZoomFactor; }
        set { _MouseZoomFactor = value; }
    }

    public float KeyboardRotationSpeed
    {
        get { return _KeyboardRotationSpeed; }
        set { _KeyboardRotationSpeed = value; }
    }


    private void Reset()
    {
        EventSystem sceneEventSystem = FindObjectOfType<EventSystem>();
        if (sceneEventSystem == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }
    }

    private void Awake()
    {
        /*Ensures there is an event system in the scene so that the UI works*/
        EventSystem sceneEventSystem = FindObjectOfType<EventSystem>();
        if (sceneEventSystem == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }
        
        HideAllWindows();
        
        if (AssetPreviewList.Count == 0)
            return;

        for (int i = (AssetPreviewList.Count - 1); i >=0; i--)
        {
            if (AssetPreviewList[i] == null)
                AssetPreviewList.RemoveAt(i);
        }

        
        ShowActiveAsset();
        
    }

    public void ToggleSceneObjectController()
    {
        bool isVisible = sceneObjectController.gameObject.activeSelf;
        HideAllWindows();
        sceneObjectController.gameObject.SetActive(!isVisible);
    }
    
    public void ToggleAssetToggleController()
    {
        bool isVisible = ToggleController.gameObject.activeSelf;
        HideAllWindows();
        ToggleController.gameObject.SetActive(!isVisible);
    }
    
    public void ToggleMaterialOptionController()
    {
        bool isVisible = MaterialOptionController.gameObject.activeSelf;
        HideAllWindows();
        MaterialOptionController.gameObject.SetActive(!isVisible);
    }

    public void ToggleSettingsWindow()
    {
        bool isVisible = SettingsWindow.gameObject.activeSelf;
        HideAllWindows();
        
        SettingsWindow.gameObject.SetActive(!isVisible);
    }

    public void ToggleAnimationWindow()
    {
        bool isVisible = AnimationController.gameObject.activeSelf;
        HideAllWindows();
        AnimationController.gameObject.SetActive(!isVisible);
    }

    public void ToggleBlendShapeWindow()
    {
        bool isVisible = BlendZoneController.gameObject.activeSelf;
        HideAllWindows();
        BlendZoneController.gameObject.SetActive((!isVisible));
    }
    public void ToggleHelpWindow()
    {
        bool isVisible = HelpWindow.gameObject.activeSelf;
        HideAllWindows();
        
        HelpWindow.gameObject.SetActive(!isVisible);
    }
    
    

    
    
    public void SetLoopAnimations(bool shouldLoop)
    {
        _activeConfigurationData.TargetAnimator.SetBool("Loop", shouldLoop);
    }

    public void HideAllWindows()
    {
        ToggleController.gameObject.SetActive(false);
        sceneObjectController.gameObject.SetActive(false);
        AnimationController.gameObject.SetActive(false);
        MaterialOptionController.gameObject.SetActive(false);
        BlendZoneController.gameObject.SetActive(false);
        SettingsWindow.gameObject.SetActive(false);
        HelpWindow.gameObject.SetActive(false);
    }
    
    public void ShowActiveAsset()
    {

        if (AssetPreviewList == null)
            return;

        if (AssetPreviewList.Count == 0)
            return;

        if (ActiveAssetID < 0 || ActiveAssetID >= AssetPreviewList.Count)
            return;

        if (ActiveGameObject != null)
            Destroy(ActiveGameObject);

        if (_activeConfigurationData != null)
            Destroy(_activeConfigurationData);

        ActiveTransform = null;


        ActiveGameObject = GameObject.Instantiate(AssetPreviewList[ActiveAssetID].gameObject);
        ActiveGameObject.name = ActiveGameObject.name.Replace("(Clone)", "");    
        _activeConfigurationData = ActiveGameObject.GetComponentInChildren<AssetConfigurationData>();
        ActiveTransform = ActiveGameObject.transform;
        ActiveGameObject.SetActive((true));
        if (AssetTargetParent != null)
            ActiveGameObject.transform.SetParent(AssetTargetParent);
        
        ActiveGameObject.transform.localPosition = AssetStartLocation;
        txtActiveAssetName.text = ActiveGameObject.name;
        SceneCamera.transform.LookAt(ActiveGameObject.transform.position + _activeConfigurationData.CameraLookPosition);
        
        if (MaterialOptionController != null)
            MaterialOptionController.SetTarget(_activeConfigurationData);


        if (AnimationController != null)
            AnimationController.SetTarget(_activeConfigurationData);
        
        if (MorphZoneController != null)
            MorphZoneController.SetTarget(_activeConfigurationData);
        
        if (BlendZoneController != null)
            BlendZoneController.SetTarget(_activeConfigurationData);
        
        if (sceneObjectController != null)
            ToggleController.SetAssetToggles(_activeConfigurationData.ToggleObjects);

    }
    
    public void NextCharacter()
    {
        ActiveAssetID++;
        if (ActiveAssetID >= AssetPreviewList.Count)
            ActiveAssetID = 0;

        ShowActiveAsset();
    }

    public void PreviousCharacter()
    {
        ActiveAssetID--;
        if (ActiveAssetID < 0)
            ActiveAssetID = AssetPreviewList.Count - 1;

        ShowActiveAsset();
    }

    public void ToggleTurnTable()
    {
        IsTurnTableActive = !IsTurnTableActive;
    }


    private void LateUpdate()
    {
        if (IsTurnTableActive)
        {
            if (ActiveGameObject != null)
            {
                ActiveGameObject.transform.Rotate(Vector3.up, (360 *Time.deltaTime) / (21f - _TurntableSpeed));
            }
        }
    }


    public void SetAssetMaterial(int subMesh, Material material)
    {
        Material[] mats;
        switch (_activeConfigurationData.meshType)
        {
            case MeshType.Mesh:
                mats = _activeConfigurationData.meshRenderer.sharedMaterials;
                mats[subMesh] = material;
                _activeConfigurationData.meshRenderer.sharedMaterials = mats;
                break;
            case MeshType.SkinnedMesh:
                mats = _activeConfigurationData.skinnedMeshRenderer.sharedMaterials;
                mats[subMesh] = material;
                _activeConfigurationData.skinnedMeshRenderer.sharedMaterials = mats;
                break;
        }
    }

    private void Update()
    {
        if (SceneCamera != null && ActiveTransform != null && _activeConfigurationData != null)
            SceneCamera.transform.LookAt(ActiveTransform.position + _activeConfigurationData.CameraLookPosition);
    }

    public void OnGUI()
    {
        Event e = Event.current;

        switch (e.type)
        {
            
            case EventType.ScrollWheel:
                if (!IsMouseOverUI)
                {
                    SceneCamera.transform.Translate(new Vector3(0, 0, (e.delta.y * -1f) / (21f-_MouseZoomFactor)));
                    SceneCamera.transform.LookAt(ActiveTransform.position + _activeConfigurationData.CameraLookPosition);
                }

                break;
            
            case EventType.KeyDown:

                switch (e.keyCode)
                {
                    case KeyCode.W:

                        SceneCamera.transform.Translate((Vector3.up * _KeyboardRotationSpeed * Time.deltaTime));
                        SceneCamera.transform.LookAt(ActiveTransform.position + _activeConfigurationData.CameraLookPosition);
                        break;
                    
                    case KeyCode.S:
                        
                        SceneCamera.transform.Translate((Vector3.down * _KeyboardRotationSpeed * Time.deltaTime));
                        SceneCamera.transform.LookAt(ActiveTransform.position + _activeConfigurationData.CameraLookPosition);
                        break;
                    
                    case KeyCode.D:

                        SceneCamera.transform.Translate((Vector3.right * _KeyboardRotationSpeed * Time.deltaTime));
                        SceneCamera.transform.LookAt(ActiveTransform.position + _activeConfigurationData.CameraLookPosition);
                        break;

                    case KeyCode.A:

                        SceneCamera.transform.Translate((Vector3.left * _KeyboardRotationSpeed * Time.deltaTime));
                        SceneCamera.transform.LookAt(ActiveTransform.position + _activeConfigurationData.CameraLookPosition);
                        break;
                    
                    case KeyCode.Escape:
                        HideAllWindows();
                        break;
                    
                    case KeyCode.T:
                        ToggleTurnTable();
                        break;
                    
                    case KeyCode.H:
                        ToggleHelpWindow();
                        break;
                }

                break;
            
            case EventType.MouseDrag:
                if (e.button == 1)
                {
                    if (e.delta.x < 0)
                    {
                        SceneCamera.transform.Translate((Vector3.right* _MouseSensativity * Time.deltaTime ));
                        SceneCamera.transform.LookAt(ActiveTransform.position + _activeConfigurationData.CameraLookPosition);
                        
                    }else if (e.delta.x > 0)
                    {
                        SceneCamera.transform.Translate((Vector3.left* _MouseSensativity * Time.deltaTime ));
                        SceneCamera.transform.LookAt(ActiveTransform.position + _activeConfigurationData.CameraLookPosition);
                    }
                    
                    
                    if (e.delta.y < 0)
                    {
                        SceneCamera.transform.Translate((Vector3.down* _MouseSensativity * Time.deltaTime ));
                        SceneCamera.transform.LookAt(ActiveTransform.position + _activeConfigurationData.CameraLookPosition);
                        
                    }else if (e.delta.y > 0)
                    {
                        SceneCamera.transform.Translate((Vector3.up* _MouseSensativity * Time.deltaTime ));
                        SceneCamera.transform.LookAt(ActiveTransform.position + _activeConfigurationData.CameraLookPosition);
                    }
                    
                }

                break;
        }
        
        
    }

    public void ResetTargetModelPosition()
    {
        ActiveGameObject.transform.localPosition = AssetStartLocation;
        ActiveGameObject.transform.rotation = Quaternion.identity;
        SceneCamera.transform.LookAt(ActiveGameObject.transform.position + _activeConfigurationData.CameraLookPosition);
    }
}
