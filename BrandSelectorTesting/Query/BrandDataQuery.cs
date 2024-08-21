using Colossal.Logging;
using Game.Prefabs;
using Game;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using BrandSelectorTesting;

public partial class BrandDataQuery : GameSystemBase
{
    public static ILog log = LogManager.GetLogger($"{nameof(BrandSelectorTesting)}.{nameof(Mod)}").SetShowsErrorsInUI(false);
    private PrefabSystem prefabSystem;
    private EntityQuery prefabQuery;

    // Internal list to store brand names
    private List<string> _brandNames = new List<string>();

    protected override void OnCreate()
    {
        base.OnCreate();
        prefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
        prefabQuery = GetEntityQuery(new EntityQueryDesc()
        {
            All = new ComponentType[]
            {
                ComponentType.ReadWrite<BrandData>()
            }
        });
        RequireForUpdate(prefabQuery);
    }

    protected override void OnUpdate()
    {
        try
        {
            if (prefabSystem == null)
            {
                Mod.log.Error("Prefab system is null.");
                return;
            }

            _brandNames.Clear();  // Clear existing data before fetching new ones
            var entities = prefabQuery.ToEntityArray(Allocator.Temp);
            foreach (Entity entity in entities)
            {
                if (prefabSystem.TryGetPrefab(entity, out PrefabBase prefabBase) && prefabBase != null)
                {
                    _brandNames.Add(prefabBase.name);
                }
            }
        }
        catch (Exception e)
        {
            Mod.log.Error(e);
        }
    }

    // Public method to retrieve brand names
    public List<string> GetBrandNames()
    {
        return _brandNames;
    }
}
