using _Scripts.Schemas;
using _Scripts.UI;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using Utility;
using Utility.Observable;

namespace _Scripts.Gameplay
{
    /// <summary>
    /// This is the service locator pattern. All shared game systems will register to this singleton,
    /// and is the main way systems communicate.
    /// </summary>
    public class ServiceLocator : SingletonMonoBehaviour<ServiceLocator>
    {
        // MonoBehavior systems 
        public NavigationManager NavigationManager { get; private set; }
        public MissionManager MissionManager { get; private set; }
        public MonsterManager MonsterManager { get; private set; }
        public TimeManager TimeManager { get; private set; }
        public UIPopupManager UIPopupManager { get; private set; }
        public LootManager LootManager { get; private set; }
        public SoundManager SoundManager { get; private set; }

        // Schema collections
        public SchemaMission[] AllMissions { get; private set; }
        public SchemaMonster[] AllMonsters { get; private set; }
        public SchemaLoot[] AllLoot { get; private set; }
        public SchemaStat[] AllStats { get; private set; }
        public SchemaPopup[] AllPopups { get; private set; }
        public SchemaQuirk[] AllQuirks { get; private set; }
        public SchemaGameSettings GameSettings { get; private set; }
        
        // Other global variables
        public Observable<int> Infamy { get; private set; } = new Observable<int>(0);
        
        protected override void Awake()
        {
            base.Awake();

            // Find all MonoBehaviour game systems.
            NavigationManager = FindObjectOfType<NavigationManager>();
            MissionManager = FindObjectOfType<MissionManager>();
            MonsterManager = FindObjectOfType<MonsterManager>();
            TimeManager = FindObjectOfType<TimeManager>();
            UIPopupManager = FindObjectOfType<UIPopupManager>();
            LootManager = FindObjectOfType<LootManager>();
            SoundManager = FindObjectOfType<SoundManager>();

            AllLoot = Resources.LoadAll<SchemaLoot>("Loot");
            
            // Missions are important to sort because they need to appear in a specific order
            AllMissions = Resources.LoadAll<SchemaMission>("Missions");
            AllMissions.Sort();
            
            AllMonsters = Resources.LoadAll<SchemaMonster>("Monsters");
            AllStats = Resources.LoadAll<SchemaStat>("Stats");
            AllPopups = Resources.LoadAll<SchemaPopup>("Popups");
            AllQuirks = Resources.LoadAll<SchemaQuirk>("Quirks");
            GameSettings = Resources.LoadAll<SchemaGameSettings>("GameSettings")[0];
        }
        
        public void DeltaInfamy(int amount)
        {
            Infamy.Value += amount;
        }


        #region Cheats
        
        [Button("Grant Ramdom Loot")]
        private void Cheat_GrantLoot()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            LootManager.GrantLoot(AllLoot[Random.Range(0, AllLoot.Length)]);
        }
        
        [Button("Add 100 Infamy")]
        private void Cheat_AddInfamy100()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            DeltaInfamy(100);
        }
        
        [Button("Add 1000 Infamy")]
        private void Cheat_AddInfamy1000()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            DeltaInfamy(1000);
        }
        
        [Button("Add 2XP Owned Monsters")]
        private void Cheat_AddXPOwnedMonsters2()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            foreach (var monster in MonsterManager.GetOwnedMonsters())
            {
                monster.AddXp(2);
            }
        }
        
        [Button("Add 10XP Owned Monsters")]
        private void Cheat_AddXPOwnedMonsters10()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            foreach (var monster in MonsterManager.GetOwnedMonsters())
            {
                monster.AddXp(10);
            }
        }
        
        [Button("Add 100XP Owned Monsters")]
        private void Cheat_AddXPOwnedMonsters100()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            foreach (var monster in MonsterManager.GetOwnedMonsters())
            {
                monster.AddXp(100);
            }
        }

        [Button("Unlock All Missions")]
        private void Cheat_UnlockAllMissions()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            foreach (var schemaMission in AllMissions)
            {
                MissionManager.UnlockMission(MissionManager.GetMissionInfo(schemaMission));
            }
        }
        
        [Button("Unlock Next Mission")]
        private void Cheat_UnlockNextMission()
        {
            if (!Application.isPlaying)
            {
                return;
            }
            
            for (var i = 0; i < AllMissions.Length; i++)
            {
                var missionInfo = MissionManager.GetMissionInfo(AllMissions[i]);
                if (missionInfo.m_status != MissionManager.MissionStatus.Locked)
                {
                    continue;
                }

                MissionManager.UnlockMission(missionInfo);
                return;
            }
        }
        
        [Button("Unlock All Monsters")]
        private void Cheat_UnlockAllMonsters()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            foreach (var monster in MonsterManager.GetMonsters(Monster.MonsterStatus.Locked))
            {
                MonsterManager.Unlock(monster);
            }
        }
        
        [Button("Recruit All Monsters")]
        private void Cheat_RecruitAllMonsters()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            foreach (var monster in MonsterManager.GetMonsters(Monster.MonsterStatus.Locked))
            {
                MonsterManager.Recruit(monster);
            }
            foreach (var monster in MonsterManager.GetMonsters(Monster.MonsterStatus.Purchasable))
            {
                MonsterManager.Recruit(monster);
            }
        }
        
        #endregion
    }
}
