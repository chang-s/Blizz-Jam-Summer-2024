using UnityEngine;
using Utility.Observable;

namespace _Scripts.Gameplay
{
    // TODO: Should this just live in MonsterManager too? perhaps
    public class RecruitManager : MonoBehaviour
    {
        public Observable<int> Infamy { get; private set; } = new Observable<int>(0);

        public void DeltaInfamy(int amount)
        {
            Infamy.Value += amount;
        }
    }
}
