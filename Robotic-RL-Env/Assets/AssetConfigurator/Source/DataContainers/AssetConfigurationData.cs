using UnityEngine;
using System.Collections.Generic;
using AssetConfigurator.DataContainers;

namespace AssetConfigurator
{
    [System.Serializable]
    public enum MeshType
    {
        None = 0,
        Mesh = 1,
        SkinnedMesh = 2,
    }
    
    

    public class AssetConfigurationData : MonoBehaviour
    {

        [SerializeField] public MeshType meshType = MeshType.None;
        [SerializeField] public SkinnedMeshRenderer skinnedMeshRenderer;
        [SerializeField] public MeshRenderer meshRenderer = null;
        [SerializeField] public List<MaterialOptionData> MaterialOptions = new List<MaterialOptionData>();
        [SerializeField] public List<string> SubmeshNames = new List<string>();
        [SerializeField] public Vector3 CameraLookPosition = new Vector3(0,1.5f, 0);
        [SerializeField] public Animator TargetAnimator;
        [SerializeField] public AnimationClip DefaultAnimation;
        [SerializeField] public List<AnimationPreviewData> AnimationOptions = new List<AnimationPreviewData>();
        [SerializeField] public List<AssetMorphData> MorphOptions = new List<AssetMorphData>();
        [SerializeField] public List<AssetBlendShapeData> BlendShapeData = new List<AssetBlendShapeData>();
        [SerializeField] public List<Transform> ToggleObjects = new List<Transform>(); 
        
        private Mesh _OriginalMesh;
        private Mesh _MorphMesh;

        private void Reset()
        {
            Animator localAnimator = GetComponent<Animator>();

            if (localAnimator != null)
            {
                TargetAnimator = localAnimator;
            }
            
        }

        public void Awake()
        {
            for (int i = (MaterialOptions.Count - 1); i >= 0; i--)
            {
                if (MaterialOptions[i].MaterialOption == null)
                    MaterialOptions.RemoveAt(i);
            }

            switch (meshType)
            {
                case MeshType.Mesh:
                    MeshFilter mf = meshRenderer.GetComponent<MeshFilter>();
                    _OriginalMesh = mf.sharedMesh;
                    break;
                case MeshType.SkinnedMesh:
                    _OriginalMesh = skinnedMeshRenderer.sharedMesh;
                    break;
            }
            
        }
        
        public bool ContainsMaterialOption(int subMeshID, Material mat)
        {
            if (MaterialOptions == null)
                return false;
            
            for (int i = 0; i < MaterialOptions.Count; i++)
            {
                if (MaterialOptions[i].SubMeshID == subMeshID && MaterialOptions[i].MaterialOption == mat)
                    return true;
            }

            return false;
        }


        public int GetSubmeshCount()
        {
            switch (meshType)
            {
                case MeshType.None:
                    return 0;
                case MeshType.Mesh:
                    return meshRenderer.sharedMaterials.Length;
                case MeshType.SkinnedMesh:
                    return skinnedMeshRenderer.sharedMaterials.Length;
                
                default:
                    return 0;
                    
            }


        }

        public void ApplyMorph(AssetMorphData morphData, float value)
        {
            
            if (_MorphMesh == null)
                _MorphMesh = new Mesh();
            
            Vector3[] VertData = _OriginalMesh.vertices;
            Vector3[] MorphVerts = morphData.TargetMesh.vertices;
            for (int i = 0; i < VertData.Length; i++)
            {
                if (i >= VertData.Length || i >= MorphVerts.Length)
                    break;
                
                VertData[i] = Vector3.Lerp(VertData[i], MorphVerts[i], value);
            }
            _MorphMesh.vertices = VertData;

            int sCount = _OriginalMesh.subMeshCount;
            for (int i = 0; i < sCount; i++)
            {
                _MorphMesh.SetTriangles(_OriginalMesh.GetTriangles(i), i);
            }

            _MorphMesh.uv = _OriginalMesh.uv;
            _MorphMesh.uv2 = _OriginalMesh.uv2;
            _MorphMesh.uv3 = _OriginalMesh.uv3;
            _MorphMesh.uv4 = _OriginalMesh.uv4;
            _MorphMesh.normals = _OriginalMesh.normals;
            _MorphMesh.tangents = _OriginalMesh.tangents;
            _MorphMesh.colors = _OriginalMesh.colors;

            switch (meshType)
            {
                case MeshType.Mesh:
                    MeshFilter mf = meshRenderer.GetComponent<MeshFilter>();
                    mf.sharedMesh = _MorphMesh;
                    break;
                case MeshType.SkinnedMesh:
                    skinnedMeshRenderer.sharedMesh = _MorphMesh;
                    break;


            }

        }
        

    }

   
}