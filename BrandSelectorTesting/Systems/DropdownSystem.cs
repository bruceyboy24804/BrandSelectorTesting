// <copyright file="ExampleSelectedInfoPanelSystem.cs" company="Yenyang's Mods. MIT License">
// Copyright (c) Yenyang's Mods. MIT License. All rights reserved.
// </copyright>

namespace BrandSelectorTesting.Systems
{
    using Colossal.UI.Binding;
    using Game.UI.InGame;
   
    using BrandSelectorTesting.Extensions;

    /// <summary>
    /// Bare bones example selected info panel system.
    /// </summary>
    public partial class DropdownSystem : ExtendedInfoSectionBase
    {
        ValueBindingHelper<string[]> m_Brands;


        /// <inheritdoc/>
        protected override string group => "GroupName";

        /// <inheritdoc/>
        public override void OnWriteProperties(IJsonWriter writer)
        {
        }

        /// <inheritdoc/>
        protected override void OnProcess()
        {
        }

        /// <inheritdoc/>
        protected override void Reset()
        {
        }

        /// <inheritdoc/>
        protected override void OnCreate()
        {
            base.OnCreate();
            m_InfoUISystem.AddMiddleSection(this);
            m_Brands = CreateBinding("AvailableBrands", new string[] { "" });
        }

        /// <inheritdoc/>
        protected override void OnUpdate()
        {
            base.OnUpdate();
            visible = true;
        }
    }
}