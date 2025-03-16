using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class ButtonsSettingsView : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown m_modeDropdown;
    [SerializeField] private TMP_Dropdown m_configDropdown;
    [SerializeField] private TMP_Text m_sliderValueText;

    [SerializeField] private GameObject m_miniGameModeControls;
    [SerializeField] private GameObject m_combatModeControls;
    [SerializeField] private GameObject m_buttonSettings;
    [SerializeField] private GameObject m_openIcon;
    [SerializeField] private GameObject m_closeIcon;

    [SerializeField] private Slider m_sizeSlider;

    private ControlsConfigUtility.ControlsConfig m_settings;
    private CustomizableButton[] m_buttons;
    private CustomizableButton m_selectedButton = null;
    private int m_currentModeId;
    private int m_currentConfigId;

    //Dropdown element have reference for this method and it's called when value is changed
    public void ChangeMode()
    {
        m_currentModeId = m_modeDropdown.value;

        m_combatModeControls.SetActive(m_currentModeId == (int)ControlMode.CombatMode);
        m_miniGameModeControls.SetActive(m_currentModeId == (int)ControlMode.MinigameMode);

        if(m_selectedButton)
        {
            m_selectedButton.SetSelected(false);
        }

        m_buttons = FindObjectsOfType<CustomizableButton>();
        LoadConfigs();
    }

    //Dropdown element have reference for this method and it's called when value is changed
    public void ChangeConfig()
    {
        m_currentConfigId = m_configDropdown.value;
        LoadCurrentConfig();
    }

    //Button have reference for this method and it's called when button is clicked
    public void SaveCurrentConfig()
    {

        var currentModeData = ControlsConfigUtility.GetCurrentModeData(m_settings.ModesData, m_currentModeId);
        if (currentModeData == null)
        {
            return;
        }

        var currentConfigData = ControlsConfigUtility.GetCurrentConfigData(currentModeData.ConfigsData, m_currentConfigId);
        if (currentConfigData == null)
        {
            return;
        }

        if (m_currentModeId != currentModeData.ModeId || m_currentConfigId != currentConfigData.ConfigId)
        {
            return;
        }

        currentConfigData.ButtonsData.Clear();
        foreach (var button in m_buttons)
        {
            if (IsButtonValid(button))
            {
                var buttonRectTransform = button.ButtonRectTransform;
                currentConfigData.ButtonsData.Add(new ControlsConfigUtility.ButtonData
                {
                    ButtonId = button.ButtonId,
                    ButtonName = button.gameObject.name,
                    ButtonPosition = buttonRectTransform.anchoredPosition,
                    ButtonSize = buttonRectTransform.sizeDelta,
                });
            }
        }

        ControlsConfigUtility.SaveToJson(m_settings);
    }

    public void SelectButton(CustomizableButton button)
    {
        m_selectedButton = button;
        m_sizeSlider.onValueChanged.RemoveAllListeners();

        m_sizeSlider.maxValue = m_selectedButton.ButtonDefaultSize.x * 2 - m_sizeSlider.minValue;
        m_sizeSlider.value = m_selectedButton.ButtonRectTransform.sizeDelta.x;

        UpdateSliderText();

        m_sizeSlider.onValueChanged.AddListener(UpdateButtonSize);

        foreach (var buttonElement in m_buttons)
        {
            buttonElement.SetSelected(m_selectedButton.ButtonId == buttonElement.ButtonId);
        }
    }

    //Button have reference for this method and it's called when button is clicked
    public void OpenSettings()
    {
        var isActive = m_buttonSettings.activeInHierarchy;

        m_buttonSettings.SetActive(!isActive);
        m_openIcon.SetActive(isActive);
        m_closeIcon.SetActive(!isActive);
    }

    //Button have reference for this method and it's called when button is clicked
    public void ResetToDefault()
    {
        foreach (var button in m_buttons)
        {
            button.ResetToDefault();
        }
    }

    protected void Start()
    {
        m_settings = ControlsConfigUtility.LoadFromJson();
        m_buttons = FindObjectsOfType<CustomizableButton>();

        m_sizeSlider.onValueChanged.AddListener(UpdateButtonSize);

        foreach (var button in m_buttons)
        {
            if (!IsButtonValid(button))
            {
                button.gameObject.SetActive(false);
            }
        }

        LoadModes();
    }

    private void UpdateSliderText()
    {
        var sliderPercentage = (m_sizeSlider.value - m_sizeSlider.minValue) / (m_sizeSlider.maxValue - m_sizeSlider.minValue) * 200;
        m_sliderValueText.text = $"{Mathf.RoundToInt(sliderPercentage)} %";
    }

    private void UpdateButtonSize(float value)
    {
        if (m_selectedButton != null)
        {
            m_selectedButton.ChangeSize(value);
            UpdateSliderText();
        }
    }

    private bool IsButtonValid(CustomizableButton button) => button.ButtonId != -1;

    private void LoadModes()
    {
        m_modeDropdown.ClearOptions();

        var modeNames = new List<string>();
        var modeIds = new List<int>();

        foreach (var mode in m_settings.ModesData)
        {
            modeNames.Add(mode.ModeName);
            modeIds.Add(mode.ModeId);
        }

        m_modeDropdown.AddOptions(modeNames);

        if (m_settings.ModesData.Count > 0)
        {
            m_currentModeId = modeIds[0];
            LoadConfigs();
        }
    }

    private void LoadConfigs()
    {
        m_configDropdown.ClearOptions();

        var currentModeData = ControlsConfigUtility.GetCurrentModeData(m_settings.ModesData, m_currentModeId);

        if (currentModeData != null)
        {
            var configNames = new List<string>();
            var configIds = new List<int>();

            foreach (var config in currentModeData.ConfigsData)
            {
                configNames.Add(config.ConfigName);
                configIds.Add(config.ConfigId);
            }

            m_configDropdown.AddOptions(configNames);

            if (currentModeData.ConfigsData.Count > 0)
            {
                m_currentConfigId = configIds[0];
                LoadCurrentConfig();
            }
        }
    }

    private void LoadCurrentConfig()
    {
        var currentModeData = ControlsConfigUtility.GetCurrentModeData(m_settings.ModesData, m_currentModeId);
        if (currentModeData == null)
        {
            return;
        }

        var currentConfigData = ControlsConfigUtility.GetCurrentConfigData(currentModeData.ConfigsData, m_currentConfigId);
        if (currentConfigData == null)
        {
            return;
        }

        foreach (var buttonData in currentConfigData.ButtonsData)
        {
            foreach (var button in m_buttons)
            {
                if (button.ButtonId == buttonData.ButtonId)
                {
                    var rectTransform = button.ButtonRectTransform;
                    rectTransform.anchoredPosition = new Vector2(buttonData.ButtonPosition.x, buttonData.ButtonPosition.y);
                    rectTransform.sizeDelta = new Vector2(buttonData.ButtonSize.x, buttonData.ButtonSize.y);
                }
            }
        }
    }
}