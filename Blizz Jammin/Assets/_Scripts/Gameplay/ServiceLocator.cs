using System.Linq;
using _Scripts.Schemas;
using _Scripts.UI;
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

        // TODO: Do some basic sorting for our intended purposes
        // Schema collections
        public SchemaMission[] AllMissions { get; private set; }
        public SchemaMonster[] AllMonsters { get; private set; }
        public SchemaLoot[] AllLoot { get; private set; }
        public SchemaStat[] AllStats { get; private set; }
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

            AllLoot = Resources.LoadAll<SchemaLoot>("Loot");
            AllMissions = Resources.LoadAll<SchemaMission>("Missions");
            AllMonsters = Resources.LoadAll<SchemaMonster>("Monsters");
            AllStats = Resources.LoadAll<SchemaStat>("Stats");
            AllQuirks = Resources.LoadAll<SchemaQuirk>("Quirks");
            GameSettings = Resources.LoadAll<SchemaGameSettings>("GameSettings")[0];
        }

        // TODO: Better lookups, might have to introduce an enum
        public SchemaStat GetStat(string statName)
        {
            return AllStats.First(s => s.Name == statName);
        }
    }
}
