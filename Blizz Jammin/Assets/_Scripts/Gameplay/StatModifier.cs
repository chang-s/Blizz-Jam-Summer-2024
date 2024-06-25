using System.Linq;
using _Scripts.Gameplay.Instances;
using _Scripts.Schemas;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Scripts.Gameplay
{
    public class StatModifier
    {
        /// <summary>
        /// The stat to modify.
        /// </summary>
        [BoxGroup("Modifier")]
        public SchemaStat Stat;
        
        /// <summary>
        /// The flat amount to delta on that stat.
        /// </summary>
        [BoxGroup("Modifier")]
        public int FlatAmount;
        
        /// <summary>
        /// The flat amount to delta on that stat.
        /// </summary>
        [BoxGroup("Modifier")]
        [Tooltip("FlatAmount takes precedence. Please use a new stat modifier to add second if needed")]
        public float MultAmount;
        
        /// <summary>
        /// Whether or not the modifier is applied to the whole party.
        /// </summary>
        [BoxGroup("Modifier")]
        public bool Aura;

        /// <summary>
        /// If supplied, at least one quirks must be present on the wielder for the modifier to take effect.
        /// "If equipped by a Sneaky monster, +10 Attack"
        /// </summary>
        [BoxGroup("Requirements")]
        public SchemaQuirk[] RequiredQuirk;
        
        /// <summary>
        /// If supplied, at least one loot must be present in the wielder of this loot for this modifier to take effect.
        /// "If wielder also has Scales equipped, gain +10 Endurance"
        /// </summary>
        [BoxGroup("Requirements")]
        public SchemaLoot[] RequiredLoot;
        
        public bool Passes(InstanceLoot loot)
        {
            bool hasLootRequirements = RequiredLoot != null && RequiredLoot.Length > 0;
            bool hasQuirkRequirements = RequiredQuirk != null && RequiredQuirk.Length > 0;
            if (!hasLootRequirements && !hasQuirkRequirements)
            {
                return true;
            }

            if (loot.EquippedMonster == null)
            {
                return false;
            }

            bool passes = true;
            if (hasLootRequirements)
            {
                passes = false;
                foreach (var schemaLoot in RequiredLoot)
                {
                    if (loot.EquippedMonster.EquippedLoot.Contains(loot))
                    {
                        passes = true;
                        break;
                    }
                }
            }

            if (!passes)
            {
                return false;
            }
            
            if (hasQuirkRequirements)
            {
                foreach (var schemaQuirk in RequiredQuirk)
                {
                    if (loot.EquippedMonster.Quirks.Contains(schemaQuirk))
                    {
                        return true;
                    }

                    if (loot.EquippedMonster.Class == schemaQuirk)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}