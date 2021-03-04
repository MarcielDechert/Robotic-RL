using UnityEngine;

namespace AssetConfigurator.DataContainers
{
    [System.Serializable]
    public class AssetBlendShapeData
    {
        [SerializeField] public int Index = 0;
        [SerializeField] public string Name = "";
        [SerializeField] public string DisplayName = "";
        [SerializeField] public float MinValue = 0;
        [SerializeField] public float MaxValue = 100;

        public AssetBlendShapeData()
        {
                
        }

        public AssetBlendShapeData(int index, string name)
        {
            Index = index;
            Name = name;
            DisplayName = name;
            MinValue = 0;
            MaxValue = 0;
        }
    }
}