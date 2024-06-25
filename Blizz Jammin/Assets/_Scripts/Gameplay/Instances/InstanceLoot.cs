using _Scripts.Schemas;

namespace _Scripts.Gameplay.Instances
{
    public class InstanceLoot : Instance
    {
        public enum LootState
        {
            // It's owned and it's never been seen by the user
            OwnedNew,
            
            // We currently own this instance and it is NOT equipped
            Owned,
            
            // This instance is currently equipped to a monster
            Equipped
        }
        
        public SchemaLoot Data { get; private set; }
        public LootState State { get; private set; } = LootState.OwnedNew;
        public Monster EquippedMonster { get; private set; }        // TODO: InstanceMonster
        
        public InstanceLoot(SchemaLoot data)
        {
            Data = data;
            State = LootState.OwnedNew;
            EquippedMonster = null;
        }

        // TODO: Make IBadgeable
        public void MarkSeen()
        {
            if (State != LootState.OwnedNew)
            {
                return;
            }

            State = LootState.Owned;
        }

        public bool Equip(Monster monster)
        {
            if (State == LootState.Equipped)
            {
                return false;
            }

            if (monster.EquippedLoot.Count >= 3)
            {
                return false;
            }

            State = LootState.Equipped;
            EquippedMonster = monster;
            monster.EquippedLoot.Add(this);
            return true;
        }

        public bool UnEquip(Monster monster)
        {
            if (State != LootState.Equipped)
            {
                return false;
            }

            if (monster != EquippedMonster)
            {
                return false;
            }

            if (!monster.EquippedLoot.Contains(this))
            {
                return false;
            }

            State = LootState.Owned;
            EquippedMonster = null;
            monster.EquippedLoot.Remove(this);
            return true;
        }
    }
}