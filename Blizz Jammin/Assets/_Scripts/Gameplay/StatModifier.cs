using _Scripts.Schemas;
using _Scripts.UI;
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
        /// If supplied, all quirks must be present on the wielder for the modifier to take effect.
        /// "If equipped by a Sneaky monster, +10 Attack"
        /// </summary>
        [BoxGroup("Requirements")]
        public SchemaQuirk[] RequiredQuirk;
        
        /// <summary>
        /// If supplied, all loot must be present in the wielder of this loot for this modifier to take effect.
        /// "If wielder also has Scales equipped, gain +10 Endurance"
        /// </summary>
        [BoxGroup("Requirements")]
        public SchemaLoot[] RequiredLoot;
        
        // TODO: Stretch goals
        /*
        /// <summary>
        /// If supplied, all loot must be present in the party for this modifier to take effect.
        /// "If party has walkie-talkie, +10 Luck"
        /// </summary>
        [BoxGroup("Requirements")] 
        public SchemaLoot[] RequiredLootParty;

        /// <summary>
        /// If supplied, all loot must be present in the party for this modifier to take effect.
        /// "If party has a Sneaky and Slimy monster, +10 Luck"
        /// </summary>
        [BoxGroup("Requirements")] 
        public SchemaLoot[] RequiredQurikParty;
        */
        public bool Passes(Loot loot)
        {
            if (RequiredLoot == null || RequiredLoot.Length == 0)
            {
                return true;
            }

            if (loot.EquippedMonster == null)
            {
                return false;
            }
            
            foreach (var schemaLoot in RequiredLoot)
            {
                Loot toFind = loot.EquippedMonster.EquippedLoot.Find(l => l.Data == schemaLoot);
                if (toFind == null)
                {
                    return false;
                }
            }

            return true;
        }
    }
}