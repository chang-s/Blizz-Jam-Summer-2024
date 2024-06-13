using _Scripts.Schemas;
using System;
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

    public void SetData(Modifier mod)
    {
        string quirkNameCleanup = mod.Quirk.name;
        if (quirkNameCleanup.Contains('_'))
            quirkNameCleanup = mod.Quirk.name.Split('_')[1];
        m_effectLabel.SetText(quirkNameCleanup);

        m_buffBackground.SetActive(mod.ModValue > 0);
        m_debuffBackground.SetActive(mod.ModValue < 0);
    }
}
