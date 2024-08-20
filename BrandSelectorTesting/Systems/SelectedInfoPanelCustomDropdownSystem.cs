using Game.Prefabs;
using Unity.Entities;
using Unity.Collections;
using Colossal.Logging;
using BrandSelectorTesting.Extensions;
using Colossal.UI.Binding;
using System.Collections.Generic;

namespace BrandSelectorTesting.Systems
{
    public partial class SelectedInfoPanelCustomDropdownSystem : ExtendedInfoSectionBase
    {
        private PrefabSystem prefabSystem;
        private Entity currentEntity;
        private EntityQuery brandQuery;
        private ValueBindingHelper<string[]> m_Brands;

        protected override string group => "GroupName"; // Define the group name for UI sections

        protected override void Reset() { }

        protected override void OnProcess() { }

        public override void OnWriteProperties(IJsonWriter writer) { }

        protected override void OnCreate()
        {
            base.OnCreate();
            prefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();

            // Define the query to get all BrandPrefabs
            brandQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    ComponentType.ReadWrite<BrandPrefab>() // Adjust based on the correct component if needed
                }
            });

            // Initialize the brand names binding
            m_Brands = CreateBinding("BrandDataQuery", new string[0]);

            // Add this system's UI section to the middle section
            m_InfoUISystem.AddMiddleSection(this);

            // Populate the brand list
            UpdateBrandList();
        }

        /// <summary>
        /// Updates the list of available brands by querying all BrandPrefab entities.
        /// </summary>
        private void UpdateBrandList()
        {
            List<string> brandNames = new List<string>();

            // Query all entities matching the BrandPrefab component
            var entities = brandQuery.ToEntityArray(Allocator.Temp);
            foreach (var entity in entities)
            {
                if (prefabSystem.TryGetPrefab(entity, out BrandPrefab brandPrefab))
                {
                    // Add logic to filter by brand type, if necessary
                    brandNames.Add(brandPrefab.name); // Add the brand name to the list
                }
            }

            entities.Dispose(); // Always dispose of the allocated array

            // Update the brand binding
            m_Brands = CreateBinding("BrandDataQuery", brandNames.ToArray());
            Mod.log.Info($"Updated brand list with {brandNames.Count} brands.");
        }

        /// <summary>
        /// Sets the current brand prefab based on the user's selection from the dropdown.
        /// </summary>
        /// <param name="brandName">The name of the selected brand.</param>
        public void SetCurrentPrefabBase(string brandName)
        {
            // Remove the current prefab if it exists
            if (currentEntity != Entity.Null)
            {
                RemoveCurrentPrefab();
            }

            // Query for the entity with the matching brand name
            var entities = brandQuery.ToEntityArray(Allocator.Temp);
            foreach (var entity in entities)
            {
                if (prefabSystem.TryGetPrefab(entity, out BrandPrefab brandPrefab))
                {
                    if (brandPrefab.name == brandName)
                    {
                        SetNewPrefab(entity);
                        break;
                    }
                }
            }

            entities.Dispose(); // Always dispose of the allocated array
        }

        /// <summary>
        /// Removes the currently active brand prefab from the game world.
        /// </summary>
        private void RemoveCurrentPrefab()
        {
            if (currentEntity != Entity.Null)
            {
                EntityManager.DestroyEntity(currentEntity);
                currentEntity = Entity.Null;
                Mod.log.Info("Removed existing prefab.");
            }
        }

        /// <summary>
        /// Sets the new brand prefab as the current active prefab.
        /// </summary>
        /// <param name="entity">The entity representing the new brand prefab.</param>
        private void SetNewPrefab(Entity entity)
        {
            currentEntity = entity;
            Mod.log.Info($"Set new brand prefab: {EntityManager.GetName(currentEntity)}");

            // Add logic to activate or instantiate the prefab in the game world
        }

        /// <summary>
        /// Called every frame to handle any necessary updates.
        /// </summary>
        protected override void OnUpdate()
        {
            base.OnUpdate();
            visible = true; // Ensure the system remains visible in the UI
        }
    }
}
