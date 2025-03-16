using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CustomizableButton))]
public class CustomizableButtonEditor : Editor
{
    private List<int> m_existingIds = new List<int>();
    private CustomizableButton m_button;
    private ControlsConfigUtility.ControlsConfig m_settings;

    public override void OnInspectorGUI()
    {
        m_button = target as CustomizableButton;

        DrawDefaultInspector();
        EditorGUILayout.Space();

        if (m_button.ButtonId != -1)
        {
            if (GUILayout.Button("Reset button id"))
            {
                m_button.SetButtonId(-1);
                EditorUtility.SetDirty(m_button);
            }
        }

        if (m_button.ButtonId == -1)
        {
            if (GUILayout.Button("Generate new button id"))
            {
                GenerateNewButtonId();
                EditorUtility.SetDirty(m_button);
            }
        }

        var buttonExists = false;
        var modeData = m_settings.ModesData[(int)m_button.ButtonControlMode];

        for (var index = 0; index < modeData.ConfigsData.Count; ++index)
        {
            buttonExists = modeData.ConfigsData[index].ButtonsData.Exists(b => b.ButtonId == m_button.ButtonId);
        }

        if (!buttonExists && m_button.ButtonId != -1)
        {
            if (GUILayout.Button("Save button config"))
            {
                var configData = modeData.ConfigsData;
                for (var index = 0; index < configData.Count; ++index)
                {
                    if (configData.Count > 0)
                    {
                        configData[index].ButtonsData.Add(new ControlsConfigUtility.ButtonData
                        {
                            ButtonName = m_button.gameObject.name,
                            ButtonId = m_button.ButtonId,
                            ButtonPosition = m_button.ButtonRectTransform.anchoredPosition,
                            ButtonSize = m_button.ButtonRectTransform.sizeDelta
                        });
                    }
                }

                ControlsConfigUtility.SaveToJson(m_settings);
                EditorUtility.SetDirty(m_button);
            }
        }
    }

    protected void OnEnable() => m_settings = ControlsConfigUtility.LoadFromJson();

    private void GenerateNewButtonId()
    {
        m_existingIds.Clear();

        var buttons = Resources.FindObjectsOfTypeAll<CustomizableButton>();
        foreach (var button in buttons)
        {
            if (button.ButtonId != -1)
            {
                m_existingIds.Add(button.ButtonId);
            }
        }

        var newButtonId = m_existingIds.Count > 0 ? m_existingIds.Max() + 1 : 0;
        m_button.SetButtonId(newButtonId);
    }
}