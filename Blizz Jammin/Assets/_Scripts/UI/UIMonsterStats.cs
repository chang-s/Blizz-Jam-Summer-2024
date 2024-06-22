using System.Collections.Generic;
using _Scripts.Gameplay;
using _Scripts.Schemas;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class UIMonsterStats : SerializedMonoBehaviour, IWorldInstanceController<Monster>
    {
        private const string c_statFormat = "{0}/{1}";

        // TODO: Consider making a struct container for both
        [SerializeField]
        private Dictionary<SchemaStat.Stat, Slider> m_statSliders = new Dictionary<SchemaStat.Stat, Slider>();
        [SerializeField]
        private Dictionary<SchemaStat.Stat, TMP_Text> m_statLabels = new Dictionary<SchemaStat.Stat, TMP_Text>();

        private SchemaGameSettings m_gameSettings;
        
        public void SetInstance(Monster instance)
        {
            int maxStatValue = ServiceLocator.Instance.GameSettings.GetMaxStatValueForLevel(instance.Level);
            var stats = ServiceLocator.Instance.AllStats;
            foreach (var schemaStat in stats)
            {
                var slider = m_statSliders[schemaStat.Type];
                var label = m_statLabels[schemaStat.Type];

                var statValue = instance.GetStatValue(schemaStat.Type);
                label.SetText(string.Format(c_statFormat, statValue, maxStatValue));
                slider.minValue = 0;
                slider.maxValue = maxStatValue;
                slider.normalizedValue = statValue / (float)maxStatValue;
            }
        }
    }
}
