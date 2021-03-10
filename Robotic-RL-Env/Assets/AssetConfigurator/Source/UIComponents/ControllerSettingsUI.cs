using UnityEngine;
using UnityEngine.UI;

namespace AssetConfigurator.UIComponents
{
    public class ControllerSettingsUI : MonoBehaviour
    {

        public Slider MouseSensativitySlider;
        public Text MouseSensativityText;
        
        public Slider TurntableSpeedSlider;
        public Text TurntableSpeedText;
        
        public Slider MouseScrollSpeedSlider;
        public Text MouseScrollSpeedText;
        
        public Slider KeyboardSpeedSlider;
        public Text KeyboardSpeedText;


        public void SetMouseSensativity(float value)
        {
            MouseSensativitySlider.value = value;
            MouseSensativityText.text = value.ToString();
        }
        
        
        public void SetTurntableSpeed(float value)
        {
            TurntableSpeedSlider.value = value;
            TurntableSpeedText.text = value.ToString();
        }
        
        public void SetMouseScrollSpeed(float value)
        {
            MouseScrollSpeedSlider.value = value;
            MouseScrollSpeedText.text = value.ToString();
        }
        
        public void SetKeyboardSpeed(float value)
        {
            KeyboardSpeedSlider.value = value;
            KeyboardSpeedText.text = value.ToString();
        }



    }
}