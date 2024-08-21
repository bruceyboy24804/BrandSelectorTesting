namespace BrandSelectorTesting.Systems
{
    using Colossal.UI.Binding;
    using BrandSelectorTesting.Extensions;
    using Game.Prefabs;
    using Game.UI.InGame;
    using Unity.Entities;
    using System.Collections.Generic;

    /// <summary>
    /// Custom dropdown system for selecting brand data.
    /// </summary>
    public partial class DropdownSystem : ExtendedInfoSectionBase
    {
        private ValueBindingHelper<string[]> m_Brands; // Binding to store brand names

        /// <inheritdoc/>
        protected override string group => "GroupName";

        /// <inheritdoc/>
        public override void OnWriteProperties(IJsonWriter writer) { }

        /// <inheritdoc/>
        protected override void OnProcess() { }

        /// <inheritdoc/>
        protected override void Reset() { }

        /// <inheritdoc/>
        protected override void OnCreate()
        {
            base.OnCreate();
            m_InfoUISystem.AddMiddleSection(this);

            // Initialize binding with an empty array to prevent null references
            m_Brands = CreateBinding("BrandDataQuery", new string[0]);

            // Dynamically update the binding with brand names
            UpdateBrandBinding();
        }

        /// <summary>
        /// Updates the binding with the brand names retrieved from BrandDataQuery.
        /// </summary>
        private void UpdateBrandBinding()
        {
            // Retrieve the brand names dynamically from BrandDataQuery
            BrandDataQuery brandDataQuery = World.GetOrCreateSystemManaged<BrandDataQuery>();
            List<string> brandNames = brandDataQuery.GetBrandNames();

            // Check if the brand names are available before updating the binding
            if (brandNames != null && brandNames.Count > 0)
            {
                // Instead of SetValue, recreate the binding with updated data
                m_Brands = CreateBinding("BrandDataQuery", brandNames.ToArray()); // Update the binding by re-creating it with new values
            }
        }

        /// <inheritdoc/>
        protected override void OnUpdate()
        {
            base.OnUpdate();
            visible = true;

            // Update the brand names in each frame to ensure it stays in sync with data
            UpdateBrandBinding();
        }
    }
}
