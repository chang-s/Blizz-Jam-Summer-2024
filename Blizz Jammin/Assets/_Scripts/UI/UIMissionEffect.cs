using System;
using _Scripts.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private Image m_buffAmountImage;

    [SerializeField] private Sprite[] m_buffIcons;
    [SerializeField] private Sprite[] m_debuffIcons;
    
    private void SetModVisuals(int modValue)
    {
        // You can assume the buff range is 1-3
        bool isBuff = modValue > 0;
        modValue = Math.Abs(modValue);
        
        Sprite[] sprites = isBuff ? m_buffIcons : m_debuffIcons;
        m_buffAmountImage.sprite = sprites[modValue - 1];
        
        m_buffBackground.SetActive(isBuff);
        m_debuffBackground.SetActive(!isBuff);
    }

    public void SetData(Mission.Modifier mod)
    {
        m_effectLabel.SetText(mod.Quirk.Name);
        m_buffBackground.SetActive(mod.ModValue > 0);
        m_debuffBackground.SetActive(mod.ModValue < 0);
        SetModVisuals(mod.ModValue);
    }
}
