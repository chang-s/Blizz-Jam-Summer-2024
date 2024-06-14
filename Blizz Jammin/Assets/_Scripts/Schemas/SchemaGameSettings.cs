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
    }
}