using UnityEngine;

namespace AssetConfigurator
{
    
    [System.Serializable]
    public class MaterialOptionData
    {
        [SerializeField] public int SubMeshID;
        [SerializeField] public Material MaterialOption;

        public MaterialOptionData(int meshID, Material mat)
        {
            this.SubMeshID = meshID;
            this.MaterialOption = mat;
        }
    }
}