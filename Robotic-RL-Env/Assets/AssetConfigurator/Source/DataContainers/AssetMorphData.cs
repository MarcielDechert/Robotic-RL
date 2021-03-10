using UnityEngine;

namespace AssetConfigurator
{

    [System.Serializable]
    public enum MorphTargetTypes
    {
        Mesh = 0,
        SubMesh = 1,
    }
    [System.Serializable]
    public class AssetMorphData
    {
        [SerializeField] public Mesh TargetMesh = null;
        [SerializeField] public MorphTargetTypes MorphType = MorphTargetTypes.Mesh;
        [SerializeField] public int SubmeshID = 0;


        public AssetMorphData()
        {
            TargetMesh = null;
            MorphType = MorphTargetTypes.Mesh;
            SubmeshID = 0;
        }

        public AssetMorphData(Mesh targetMesh)
        {
            TargetMesh = targetMesh;
            MorphType = MorphTargetTypes.Mesh;
            SubmeshID = 0;
        }
        
        public AssetMorphData(Mesh targetMesh, MorphTargetTypes morphType, int meshID)
        {
            TargetMesh = targetMesh;
            MorphType = morphType;
            SubmeshID = meshID;
        }
    }
    
    
}