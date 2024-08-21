using Game;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Colossal.Logging;
using Colossal.UI.Binding;
using BrandSelectorTesting.Extensions;
using BrandSelectorTesting;
using Game.Prefabs;

public partial class BrandDataQuery : GameSystemBase
{
    public static ILog log = LogManager.GetLogger($"{nameof(BrandSelectorTesting)}.{nameof(Mod)}").SetShowsErrorsInUI(false);
    private PrefabSystem prefabSystem;
    private EntityQuery prefabQuery;

    // Binding helper to expose the brand names to the UI
    private ValueBindingHelper<string[]> m_Brands;

    private List<string> brandNames = new List<string>();

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

        // Create binding for brand names
        m_Brands = CreateBinding("AvailableBrands", new string[] { });
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

            brandNames.Clear();  // Clear previous data
            var entities = prefabQuery.ToEntityArray(Allocator.Temp);

            foreach (Entity entity in entities)
            {
                if (prefabSystem.TryGetPrefab(entity, out PrefabBase prefabBase) && prefabBase != null)
                {
                    brandNames.Add(prefabBase.name);  // Add brand name to the list
                }
            }

            entities.Dispose();

            // Update the binding with the current brand names
            m_Brands.Value = brandNames.ToArray();
        }
        catch (Exception e)
        {
            Mod.log.Error(e);
        }
    }

    // Expose the brand names so the UI can fetch them if necessary
    public List<string> GetBrandNames()
    {
        return brandNames;
    }
}
