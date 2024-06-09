using System.Collections.Generic;
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
        public MonsterManager MonsterManager { get; private set; }
        public UIPopupManager UIPopupManager { get; private set; }

        // Schema collections
        public SchemaMission[] AllMissions { get; private set; }
        public SchemaMonster[] AllMonsters { get; private set; }
        public SchemaLoot[] AllLoot { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            // Find all MonoBehaviour game systems.
            NavigationManager = FindObjectOfType<NavigationManager>();
            MonsterManager = FindObjectOfType<MonsterManager>();
            UIPopupManager = FindObjectOfType<UIPopupManager>();

            AllLoot = Resources.LoadAll<SchemaLoot>("Loot");
            AllMissions = Resources.LoadAll<SchemaMission>("Missions");
            AllMonsters = Resources.LoadAll<SchemaMonster>("Monsters");
        }
    }
}
