using Game.Prefabs;
using Unity.Entities;
using Unity.Collections;
using Colossal.Logging;
using System.Collections.Generic;
using Game.Companies;

namespace BrandSelectorTesting.Systems
{
    public partial class BrandBuildingMatcherSystem : SystemBase
    {

        public List<string> CachedBrandNames { get; private set; } = new List<string>(); // Exposing cached brand names
        private PrefabSystem prefabSystem;
        private EntityQuery buildingQuery;
        private EntityQuery companyQuery;

        protected override void OnCreate()
        {
            base.OnCreate();
            prefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();

            // Query to retrieve buildings with company types allowed to rent
            buildingQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    
                    ComponentType.ReadWrite<CompanyData>() // CompanyData contains allowed renting companies
                }
            });

            // Query to retrieve CompanyData entities
            companyQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    ComponentType.ReadWrite<CompanyData>() // CompanyData holds company-related information
                }
            });
        }

        /// <summary>
        /// Get matching brands that can rent space in the provided building.
        /// </summary>
        public List<string> GetMatchingBrandsForBuilding(Entity buildingEntity)
        {
            List<string> matchingBrands = new List<string>();

            if (EntityManager.HasComponent<CompanyData>(buildingEntity))
            {
                var buildingCompanyData = EntityManager.GetComponentData<CompanyData>(buildingEntity);

                // Query all entities that have CompanyData (instead of BrandPrefab)
                var companyEntities = companyQuery.ToEntityArray(Allocator.Temp);
                foreach (var companyEntity in companyEntities)
                {
                    if (EntityManager.HasComponent<CompanyData>(companyEntity))
                    {
                        var companyData = EntityManager.GetComponentData<CompanyData>(companyEntity);

                        // Check if the company's data matches the building's allowed company types
                        if (MatchesBuildingType(companyData, buildingCompanyData))
                        {
                            matchingBrands.Add(companyData.m_Brand.ToString()); // Assuming m_Brand holds the brand name or ID
                        }
                    }
                }
                companyEntities.Dispose();
            }
            return matchingBrands;
        }

        /// <summary>
        /// Checks if the company matches any of the allowed company types for the building.
        /// </summary>
        private bool MatchesBuildingType(CompanyData companyData, CompanyData buildingCompanyData)
        {
            // Match the company type with the allowed types for the building
            return companyData.m_Brand != null && companyData.m_Brand == buildingCompanyData.m_Brand;
        }

        protected override void OnUpdate()
        {
            // Assume logic to get brands for specific building
            if (buildingQuery.CalculateEntityCount() > 0)
            {
                var entities = buildingQuery.ToEntityArray(Allocator.Temp);
                foreach (var entity in entities)
                {
                    CachedBrandNames = GetMatchingBrandsForBuilding(entity);
                }
                entities.Dispose();
            }
        }
    }
}
