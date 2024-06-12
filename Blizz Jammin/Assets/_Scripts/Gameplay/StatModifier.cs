using _Scripts.Schemas;
using Sirenix.OdinInspector;

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
        /// TODO: Consider PercentAmount too, if we have time
        /// </summary>
        [BoxGroup("Modifier")]
        public int Amount;
        
        /// <summary>
        /// Whether or not the modifier is applied to the whole party.
        /// </summary>
        [BoxGroup("Modifier")]
        public bool Aura;
        
        /// <summary>
        /// If supplied, all loot must be present in the wielder of this loot for this modifier to take effect.
        /// "If wielder also has Scales equipped, gain +10 Endurance"
        /// </summary>
        [BoxGroup("Requirements")]
        public SchemaLoot[] RequiredLoot;

        /// <summary>
        /// If supplied, all quirks must be present on the wielder for the modifier to take effect.
        /// "If equipped by a Sneaky monster, +10 Attack"
        /// </summary>
        [BoxGroup("Requirements")]
        public SchemaQuirk[] RequiredQuirk;
        
        // TODO: These two categories are stretch goals, if we have time
        /// <summary>
        /// If supplied, all loot must be present in the party for this modifier to take effect.
        /// "If party has walkie-talkie, +10 Luck"
        /// </summary>
        [BoxGroup("Requirements")] 
        public SchemaLoot[] RequiredLootParty;
        
        // TODO: These two categories are stretch goals, if we have time
        /// <summary>
        /// If supplied, all loot must be present in the party for this modifier to take effect.
        /// "If party has a Sneaky and Slimy monster, +10 Luck"
        /// </summary>
        [BoxGroup("Requirements")] 
        public SchemaLoot[] RequiredQurikParty;
    }
}