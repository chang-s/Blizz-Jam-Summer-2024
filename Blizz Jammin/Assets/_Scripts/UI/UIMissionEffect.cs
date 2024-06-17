using _Scripts.Schemas;
using System;
using _Scripts.Gameplay;
using TMPro;
using UnityEngine;
using static _Scripts.Schemas.SchemaMission;

public class UIMissionEffect : MonoBehaviour
{
    [Serializable]
    public enum State
    {
        // Used in the party 
        Buff,
        Debuff,
    }

    [SerializeField] private GameObject m_buffBackground;
    [SerializeField] private GameObject m_debuffBackground;
    [SerializeField] private TextMeshProUGUI m_effectLabel;
    [SerializeField] private Transform m_modVisualRoot;
    [SerializeField] private Transform m_buffMod;
    [SerializeField] private Transform m_debuffMod;

    public void SetState(State state)
    {
        switch (state)
        {
            case State.Buff:
                m_buffBackground.SetActive(true);
                m_debuffBackground.SetActive(false);
                break;
            case State.Debuff:
                m_buffBackground.SetActive(false);
                m_buffBackground.SetActive(true);
                break;
        }
    }

    private void ClearModVisuals()
    {
        foreach (Transform child in m_modVisualRoot)
        {
            Destroy(child);
        }
    }

    private void SetModVisuals(int modValue)
    {
        Transform modVisual = modValue > 0 ? m_buffMod : m_debuffMod;
        modValue = Mathf.Abs(modValue);
        for (int i = 0; i < modValue; ++i)
        {
            Instantiate(modVisual, m_modVisualRoot);
        }
    }

    public void SetData(Mission.Modifier mod)
    {
        string quirkNameCleanup = mod.Quirk.name;
        if (quirkNameCleanup.Contains('_'))
            quirkNameCleanup = mod.Quirk.name.Split('_')[1];
        m_effectLabel.SetText(quirkNameCleanup);

        m_buffBackground.SetActive(mod.ModValue > 0);
        m_debuffBackground.SetActive(mod.ModValue < 0);

        ClearModVisuals();
        SetModVisuals(mod.ModValue);
    }
}
