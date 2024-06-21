using System.Linq;
using _Scripts.Schemas;
using _Scripts.UI;
using Sirenix.Utilities;
using UnityEngine;
using Utility;

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
        public RecruitManager RecruitManager { get; private set; }

        // Schema collections
        public SchemaMission[] AllMissions { get; private set; }
        public SchemaMonster[] AllMonsters { get; private set; }
        public SchemaLoot[] AllLoot { get; private set; }
        public SchemaStat[] AllStats { get; private set; }
        public SchemaPopup[] AllPopups { get; private set; }
        public SchemaQuirk[] AllQuirks { get; private set; }
        public SchemaGameSettings GameSettings { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            // Find all MonoBehaviour game systems.
            NavigationManager = FindObjectOfType<NavigationManager>();
            MissionManager = FindObjectOfType<MissionManager>();
            MonsterManager = FindObjectOfType<MonsterManager>();
            TimeManager = FindObjectOfType<TimeManager>();
            UIPopupManager = FindObjectOfType<UIPopupManager>();
            RecruitManager = FindObjectOfType<RecruitManager>();

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
    }
}
