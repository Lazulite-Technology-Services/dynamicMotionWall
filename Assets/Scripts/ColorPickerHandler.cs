using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColorPickerHandler : MonoBehaviour
{
    private Color32 newColor;

    

    [SerializeField]
    private Button setColor_Btn, SetBrightness_Btn, setSpeed_Btn, close_Btn;

    [SerializeField]
    private Slider brightnessSlider, animateSpeedSlider;    

    private void Awake()
    {
        
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        setColor_Btn.onClick.AddListener(GetAndSetRGBColorValues);
        SetBrightness_Btn.onClick.AddListener(SetBrightness);
        setSpeed_Btn.onClick.AddListener(SetSpeed);

        close_Btn.onClick.AddListener(GameManager.instance.EnableorDisableColorPickerPanel);
    }
    

    private void GetAndSetRGBColorValues()
    {
        string colval = string.Empty;

        newColor = HexToColor(GameManager.instance.FlColorPicker.hexInput.text);

        if (GameManager.instance.isSetDefaultColor)
        {
            GameManager.instance.hexValue = colval = $"*L,{newColor.r},{newColor.g},{newColor.b}#";
        }
        else
        {

            if (GameManager.instance.isAnimate)
            {
                GameManager.instance.hexValue = colval = $"*{newColor.r},{newColor.g},{newColor.b}#";
            }
            else if (GameManager.instance.isAnimate == false)
            {
                if (GameManager.instance.isSingle)
                {
                    Debug.Log("inside the issingle true");
                    GameManager.instance.hexValue = colval = $"*{GameManager.instance.ProductButtonIndex},{newColor.r},{newColor.g},{newColor.b}#";
                }
                else
                {
                    Debug.Log("inside the issingle false");
                    GameManager.instance.hexValue = colval = $"*{GameManager.instance.columnNumber},{newColor.r},{newColor.g},{newColor.b}#";
                    //*C1,Rval,Gval,Bval#
                }
                //    GameManager.instance.hexValue = colval = $"*{GameManager.instance.ProductButtonIndex},{newColor.r},{newColor.g},{newColor.b}#";
                
            }
        }

        Debug.Log($"Product hexVal : {GameManager.instance.hexValue}");

        GameManager.instance.SendCommandToArduino(colval);
    }

    public void SetColorValurForAll()
    {
        string colval = string.Empty;

        newColor = HexToColor(GameManager.instance.FlColorPicker.hexInput.text);

        GameManager.instance.hexValue = colval = $"*L{newColor.r},{newColor.g},{newColor.b}#";
        Debug.Log($"The color value for all is : {GameManager.instance.hexValue}");
    }

    private Color HexToColor(string hex)
    {
        if (hex.StartsWith("#")) hex = hex.Substring(1);

        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        byte a = (hex.Length == 8)
                 ? byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber)
                 : (byte)255;

        return new Color32(r, g, b, a);
    }

    private void SetBrightness()
    {
        string brightnessval;
        GameManager.instance.brightness = brightnessval = $"*B={brightnessSlider.value.ToString()}#";

        GameManager.instance.SendCommandToArduino(brightnessval);
    }

    private void SetSpeed()
    {
        string setspeed;
        GameManager.instance.animationSpeed = setspeed = $"*S={animateSpeedSlider.value.ToString()}#";

        GameManager.instance.SendCommandToArduino(setspeed);
    }

    public void UpdateBrightnessInUI(TextMeshProUGUI text)
    {
        text.text = ((int)(brightnessSlider.value)).ToString();
    }

    public void UpdateSpeedInUI(TextMeshProUGUI text)
    {
        text.text = ((int)(animateSpeedSlider.value)).ToString();
    }
}
