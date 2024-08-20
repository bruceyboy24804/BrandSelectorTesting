using System.Collections.Generic;
using Game.Prefabs;
using Unity.Entities;
using Unity.Collections;
using Colossal.Logging;

namespace BrandSelectorTesting.Systems
{
    public partial class BrandDataQuerySystem : SystemBase
    {
        private EntityQuery brandQuery;
        private PrefabSystem prefabSystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            prefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();

            // Ensure the system is required for update
            brandQuery = GetEntityQuery(new EntityQueryDesc()
            {
                All = new ComponentType[]
                {
            ComponentType.ReadWrite<BrandPrefab>()
                }
            });

            RequireForUpdate(brandQuery); // Ensure this system will run if there's relevant data
        }


        protected override void OnUpdate()
        {
            // Log or process the brand data
            List<string> brands = GetBrands();
            Mod.log.Info($"Fetched {brands.Count} brands.");
        }

        public List<string> GetBrands()
        {
            List<string> brandNames = new List<string>();

            // Query all entities that match the BrandPrefab component
            var brandEntities = brandQuery.ToEntityArray(Allocator.Temp);
            foreach (var entity in brandEntities)
            {
                if (prefabSystem.TryGetPrefab(entity, out BrandPrefab brandPrefab))
                {
                    brandNames.Add(brandPrefab.name); // Add the brand name
                }
            }

            brandEntities.Dispose();
            return brandNames;
        }

    }
}
