using TMPro;
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
    private GameObject brightnessSlider_Object, animSpeedSlider_Object, commandPanel, columnSelectionPanel;

    #endregion

    #region UI ELEMENTS

    [SerializeField]
    private Button[] productButtonsArray;
    [SerializeField]
    private Button[] productColumnButtonsArray;

    [SerializeField]
    private Button back, product_1, product_2, saveBluetoothName, openCommandPanel, singleSelection, columnSelection, ResetButton;

    [SerializeField]
    private TMP_InputField bluetoothName;

    #endregion

    private bool isPressed = true;
    public bool isAnimate = false;
    [SerializeField]
    public bool isSingle = true;
    [SerializeField]
    public string columnNumber = string.Empty;

    [SerializeField]
    private int productBtnCount = 0;

    public int selectedProductButtonIndex = 0;

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
        enter, animatOrProduct, animate, product, productselection
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
        singleSelection.onClick.AddListener(SingleSelectionProductView);
        columnSelection.onClick.AddListener(MultipleSelectionColumnView);
        ResetButton.onClick.AddListener(() => SendCommandToArduino("*reset#"));

        
        currentState = ScreenState.enter;
        back.gameObject.SetActive(false);

        //for(int i = 0; i < productColumnButtonsArray.Length; i++)
        //{
        //    productColumnButtonsArray[i].onClick.AddListener(EnableorDisableColorPickerPanel);
        //}
    }    

    private void EnableOrDisableCommandPanel()
    {
        if (commandPanel.activeSelf)
        {
            commandPanel.SetActive(false);
        }
        else
        {
            PlayerPrefs.SetString("BT_Device", bluetoothName.text);
            commandPanel.SetActive(true);
        }
    }

    public void SendCommandToArduino(string command)
    {
        BluetoothManager.Instance.SendBTMessage(command);
        Debug.Log($"the command sent : {command}");
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
                ResetButton.gameObject.SetActive(false);
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
            case 4:
                screensArray[3].SetActive(false);
                screensArray[4].SetActive(true);

                currentState = ScreenState.productselection;
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

    public void SelectOrDeselectProductButtons(Button btn)
    {
        ColorPickerPanel.SetActive(true);
        brightnessSlider_Object.SetActive(false);
        animSpeedSlider_Object.SetActive(false);

        isAnimate = false;

        if (isSingle)
        {
            Debug.Log("enabling color panel in single");
            ProductButtonIndex = int.Parse(btn.name);            
        }
        else
        {
            Debug.Log("enabling color panel in single");
            columnNumber = $"c{btn.name}";
        }
    }

    private void SingleSelectionProductView()
    {
        isSingle = true;
        EnableNextScreen(4);
        columnSelectionPanel.SetActive(false);
        Debug.Log("Issingle is : " + isSingle);
    }

    private void MultipleSelectionColumnView()
    {
        isSingle = false;
        EnableNextScreen(4);
        columnSelectionPanel.SetActive(true);

        for(int i = 0; i < productButtonsArray.Length; i++)
        {
            productButtonsArray[i].enabled = false;
            productButtonsArray[i].GetComponent<Image>().enabled = false;
            productButtonsArray[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = string.Empty;
        }

        Debug.Log("Issingle is : " + isSingle);
    }

    private void OnColumnSelected_SendCommand(int index)
    {
        columnNumber = index.ToString();
    }

    public bool isSetDefaultColor = false;

    public void ColorToggleButtonProperty(Toggle toggle)
    {
        //Enable the color panel
        if(toggle.isOn)
        {
            isSetDefaultColor = true;
            EnableorDisableColorPickerPanel();
        }
        else
        {
            isSetDefaultColor = false;
            SendCommandToArduino("*a#");
        }
    }

    public void Back()
    {
        switch (currentState)
        {
            case ScreenState.animatOrProduct:
                screensArray[1].SetActive(false);
                screensArray[0].SetActive(true);
                ResetButton.gameObject.SetActive(true);
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
            case ScreenState.productselection:
                screensArray[4].SetActive(false);
                screensArray[3].SetActive(true);
                ResetProductButtonArray();

                currentState = ScreenState.product;
                break;
            default:
                break;
        }
        
    }

    private void ResetProductButtonArray()
    {
        for (int i = 0; i < productButtonsArray.Length; i++)
        {
            productButtonsArray[i].enabled = true;
            productButtonsArray[i].GetComponent<Image>().enabled = true;
            productButtonsArray[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (i + 1).ToString();
        }
    }

    
}
