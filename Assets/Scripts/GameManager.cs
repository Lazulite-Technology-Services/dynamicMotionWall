using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public FlexibleColorPicker FlColorPicker;

    [SerializeField]
    private int currentIndex, lastIndex = 0;

    public int ProductButtonIndex = 0;


    #region GAMEOBJECTS
    [Header("Game Objects")]
    [Space(5)]
    [SerializeField]
    private GameObject ColorPickerPanel;

    [SerializeField]
    private GameObject[] screensArray;

    [SerializeField]
    private GameObject brightnessSlider_Object, animSpeedSlider_Object;

    #endregion

    #region UI ELEMENTS

    [SerializeField]
    private Button[] buttonsArray;

    [SerializeField]
    private Button back, product_1, product_2;

    #endregion

    private bool isPressed = true;
    public bool isAnimate = false;

    #region BUTTON COMMAND VALUES

    [Header("Command Containers")]
    [Space(5)]
    [SerializeField]
    private string CurrentButtonCommand = string.Empty;

    [SerializeField]
    public string hexValue, brightness, animationSpeed;

    [SerializeField]
    private string product_1_Command, product_2_Command;

    #endregion

    private enum ScreenState
    {
        enter, animatOrProduct, animate, product
    };

    [SerializeField] private ScreenState currentState;

    
    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        instance = this;       

        back.onClick.AddListener(Back);
        currentState = ScreenState.enter;
        back.gameObject.SetActive(false);
    }    

    private void SendHexCode()
    {
        
    }

    public void SendCommandToArduino(string command)
    {
        BluetoothManager.Instance.SendBTMessage(command);
    }

    public void EnableNextScreen(int index)
    {
        currentIndex = index;

        switch (index)
        {
            case 1:
                screensArray[1].SetActive(true);
                screensArray[0].SetActive(false);
                back.gameObject.SetActive(true);
                currentState = ScreenState.animatOrProduct;
                break;
            case 2:
                screensArray[1].SetActive(false);
                screensArray[2].SetActive(true);

                currentState = ScreenState.animate;
                break;
            case 3:
                screensArray[1].SetActive(false);
                screensArray[3].SetActive(true);

                currentState = ScreenState.product;
                break;
            default:
                Debug.Log($"no such screen : {index}");
                break;
        }
    }

    public void DisablePreviousScreen(GameObject screen)
    {
        screen.SetActive(false);
    }

    public void EnableorDisableColorPickerPanel()
    {
        if(ColorPickerPanel.activeSelf)
        {
            ColorPickerPanel.SetActive(false);
        }
        else
        {
            ColorPickerPanel.SetActive(true);
        }
    }

    public void SetButtonCommand(string command)
    {
        isAnimate = true;
        CurrentButtonCommand = command;

        brightnessSlider_Object.SetActive(true);
        animSpeedSlider_Object.SetActive(true);

        SendCommandToArduino(command);
    }

    public void  SelectOrDeselectProductButtons(Button btn)
    {
        if(btn.name == "Product_1")
        {
            isAnimate = false;
            ProductButtonIndex = 1;

            if (isPressed)
            {
                //isPressed = true;
                product_1_Command = "*1-S#";
                //product_2.interactable = false;
                ColorPickerPanel.SetActive(true);

                SendCommandToArduino("*1-S#");

                brightnessSlider_Object.SetActive(false);
                animSpeedSlider_Object.SetActive(false);
            }
            //else
            //{
            //    isPressed = false;
            //    product_1_Command = "*1-D#";
            //    product_2.interactable = true;

            //    SendCommandToArduino("*1-D#");

            //    brightnessSlider_Object.SetActive(true);
            //    animSpeedSlider_Object.SetActive(true);
            //}
        }
        else if (btn.name == "Product_2")
        {
            isAnimate = false;
            ProductButtonIndex = 2;

            if (isPressed)
            {
                //isPressed = true;
                product_1_Command = "*2-S#";
                //product_1.interactable = false;
                ColorPickerPanel.SetActive(true);

                SendCommandToArduino("*2-S#");

                brightnessSlider_Object.SetActive(false);
                animSpeedSlider_Object.SetActive(false);
            }
            //else
            //{
            //    isPressed = false;
            //    product_1_Command = "*2-D#";
            //    product_1.interactable = true;

            //    SendCommandToArduino("*2-D#");

            //    brightnessSlider_Object.SetActive(true);
            //    animSpeedSlider_Object.SetActive(true);
            //}
        }
    }

    public void Back()
    {
        switch (currentState)
        {
            case ScreenState.animatOrProduct:
                screensArray[1].SetActive(false);
                screensArray[0].SetActive(true);
                currentState = ScreenState.enter;
                back.gameObject.SetActive(false);
                break;
            case ScreenState.animate:
                isAnimate = true;
                
                screensArray[2].SetActive(false);
                screensArray[1].SetActive(true);
                currentState = ScreenState.animatOrProduct;
                break;
            case ScreenState.product:
                screensArray[3].SetActive(false);
                screensArray[1].SetActive(true);
                currentState = ScreenState.animatOrProduct;
                break;
            default:
                break;
        }
        
    }

    
}
