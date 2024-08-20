using Game.Prefabs;
using Game;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Colossal.Logging;

namespace BrandSelectorTesting.Systems
{
    /// <summary>
    /// System to query and categorize BrandPrefabs based on their companies and other data.
    /// </summary>
    public partial class BrandPrefabQuerySystem : SystemBase
    {
        private PrefabSystem prefabSystem;
        private EntityQuery brandQuery;

        protected override void OnCreate()
        {
            base.OnCreate();
            prefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();

            // Query to retrieve all entities with BrandPrefab
            brandQuery = GetEntityQuery(new EntityQueryDesc()
            {
                All = new ComponentType[]
                {
                    ComponentType.ReadWrite<BrandPrefab>() // Query for BrandPrefab specifically
                }
            });
        }

        /// <summary>
        /// Method to retrieve BrandPrefabs and categorize them based on company or other data.
        /// </summary>
        public List<BrandInfo> GetBrandsWithTypes()
        {
            List<BrandInfo> brandInfoList = new List<BrandInfo>();

            // Query all entities that match the BrandPrefab component
            var brandEntities = brandQuery.ToEntityArray(Allocator.Temp);
            foreach (var entity in brandEntities)
            {
                // Try to get the BrandPrefab associated with this entity
                if (prefabSystem.TryGetPrefab(entity, out BrandPrefab brandPrefab) && brandPrefab != null)
                {
                    // Get the brand name from the prefab (assuming the name property is in the prefab base)
                    string brandName = brandPrefab.name;

                    // Determine the type of the brand based on the companies or other logic
                    string brandType = DetermineBrandType(brandPrefab);

                    // Add the brand information to the list
                    brandInfoList.Add(new BrandInfo(brandName, brandType));

                    Mod.log.Info($"Brand: {brandName}, Type: {brandType}");
                }
            }

            brandEntities.Dispose(); // Dispose of the temporary array
            return brandInfoList;
        }

        /// <summary>
        /// Helper method to determine the type (Commercial, Industrial, Office) based on the prefab's companies or data.
        /// </summary>
        private string DetermineBrandType(BrandPrefab brandPrefab)
        {
            // Check the companies or brand data to determine the type
            if (brandPrefab.m_Companies != null)
            {
                foreach (var company in brandPrefab.m_Companies)
                {
                    string companyName = company.name.ToLower();

                    // Example logic to categorize the brand based on the associated companies
                    if (companyName.Contains("commercial"))
                    {
                        return "Commercial";
                    }
                    else if (companyName.Contains("industrial"))
                    {
                        return "Industrial";
                    }
                    else if (companyName.Contains("office"))
                    {
                        return "Office";
                    }
                }
            }

            return "Unknown"; // Return unknown if no match is found
        }

        protected override void OnUpdate()
        {
            // Ensure this is being called so the data is updated properly
            List<BrandInfo> brandInfoList = GetBrandsWithTypes();
            // ...any additional logic
        }

    }

    /// <summary>
    /// Simple class to hold brand name and its type.
    /// </summary>
    public class BrandInfo
    {
        public string BrandName { get; }
        public string BrandType { get; }

        public BrandInfo(string brandName, string brandType)
        {
            BrandName = brandName;
            BrandType = brandType;
        }
    }
}
