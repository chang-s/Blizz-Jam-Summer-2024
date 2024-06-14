using Sirenix.OdinInspector;
using UnityEngine;

namespace _Scripts.Schemas
{
    [CreateAssetMenu(menuName = "Schema/GameSettings")]
    public class SchemaGameSettings : Schema
    {
        [BoxGroup("Timer")]
        [Tooltip("How much real time (in seconds) equals 1 day in the game")]
        [SerializeField] public float SecondsPerDay;
        
        [BoxGroup("Combat")]
        [Tooltip("The exponent applied to every stat (before items), where the power is equal to the monster's level.")]
        [SerializeField] public float StatLevelExponent;
        
        [BoxGroup("Combat")]
        [Tooltip("The base scalar to multiply damage calculations when an attack crits.")]
        [SerializeField] public float DefaultCritScalar;
        
        [BoxGroup("Combat")]
        [Tooltip("How much crit chance you get for 1 Luck. This means there is a cap!")]
        [SerializeField] public float CritChancePerLuck;
        
        [BoxGroup("Combat")]
        [Tooltip("Once capped, any remaining luck becomes crit scalar at this rate.")]
        [SerializeField] public float AdditionalCritScalarPerLuck;
        
        [BoxGroup("Combat")]
        [Tooltip("Terror directly modifies the crit scalar, this much per Terror.")]
        [SerializeField] public float CritScalarPerTerror;
        
        [BoxGroup("Combat")]
        [Tooltip("Symbiosis directly modifies the flat attack value of a monster per step. " +
                 "This value is how much attack damage a monster gets for each point of terror. This value is ALWAYS" +
                 "multiplied by the amount of non-exhausted party members!")]
        [SerializeField] public float DamageBonusPerSymbiosis;
    }
}