using UnityEngine;

namespace AssetConfigurator
{
    [System.Serializable]
    public class AnimationPreviewData
    {
        [SerializeField]public AnimationClip Animation;
        [SerializeField]public bool Loop;
        [SerializeField]public float PlayLength;


        public AnimationPreviewData()
        {
            Animation = null;
            Loop = false;
            PlayLength = 0;
        }

        public AnimationPreviewData(AnimationClip clip, bool loop, float playLength)
        {
            Animation = clip;
            Loop = loop;
            PlayLength = playLength;
        }
        
        public AnimationPreviewData(AnimationClip clip)
        {
            Animation = clip;
            Loop = false;
            PlayLength = clip.length;
        }
        
        
    }
}