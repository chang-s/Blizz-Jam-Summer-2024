using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Scripts.Schemas
{
    [CreateAssetMenu(menuName = "Schema/GameSettings")]
    public class SchemaGameSettings : Schema
    {
        [BoxGroup("Timer")]
        [Tooltip("How much real time (in seconds) equals 1 day in the game")]
        [MinValue(1)]
        [SerializeField] public float SecondsPerDay;
        
        [BoxGroup("Timer")]
        [Tooltip("The amount of terror required to reduce the mission length by 1 day")]
        [MinValue(1)]
        [SerializeField] public int MissionSpeedTerrorPerDay;
        
        [BoxGroup("Monster Levels")]
        [Tooltip("How much XP you need to have to be at the level (-1 based)")]
        [SerializeField] public int[] XpForLevel;
        
        [BoxGroup("Combat")]
        [Tooltip("The exponent applied to every stat (before items), where the power is equal to the monster's level.")]
        [MinValue(1)]
        [SerializeField] public float StatLevelExponent;
        
        [BoxGroup("Combat")]
        [Tooltip("The base scalar to multiply damage calculations when an attack crits.")]
        [MinValue(0)]
        [SerializeField] public float DefaultCritScalar;
        
        [BoxGroup("Combat")]
        [Tooltip("How much crit you need for a guaranteed crit")]
        [MinValue(0)]
        [SerializeField] public int CritChanceCap;
        
        [BoxGroup("Combat")]
        [Tooltip("Once capped, any remaining luck becomes crit scalar at this rate.")]
        [MinValue(0)]
        [SerializeField] public float CritScalarPerSurplusLuck;
        
        [BoxGroup("Combat")]
        [Tooltip("Terror directly modifies the crit scalar, this much per Terror.")]
        [MinValue(0)]
        [SerializeField] public float CritScalarPerTerror;
        
        [BoxGroup("Combat")]
        [Tooltip("Symbiosis directly modifies the flat attack value of a monster per step. " +
                 "This value is how much attack damage a monster gets for each point of terror. This value is ALWAYS" +
                 "multiplied by the amount of non-exhausted party members!")]
        [MinValue(0)]
        [SerializeField] public float DamageBonusPerSymbiosis;
        
        [BoxGroup("Rewards")]
        [Tooltip("The amount of luck that is needed to get an extra loot roll")]
        [MinValue(0)]
        [SerializeField] public float ExtraLootPerLuck;

        [BoxGroup("Rewards")]
        [Tooltip("The amount of Infamy scalar to get per Terror in the party")]
        [MinValue(0)]
        [SerializeField] public float InfamyScalarPerTerror { get; set; }
        
        [BoxGroup("Rewards")]
        [Tooltip("The amount of Infamy scalar to get per Terror in the party")]
        [MinValue(0)]
        [SerializeField] public float XpScalarPerSymbiosis { get; set; }

        /// <summary>
        /// This is mostly a hack. We can easily determine the highest value of a stat by level if we know
        /// how much the maximum amount of level 1 is.
        /// </summary>

        private const int c_maxStatAtLevelOne = 10;
        public int GetMaxStatValueForLevel(int level)
        {
            return (int) (c_maxStatAtLevelOne * Math.Pow(StatLevelExponent, level - 1));
        }
    }
}