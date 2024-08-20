import { bindValue, trigger, useValue } from "cs2/api";
import { useState, useCallback } from "react";
import styles from './custom-dropdown.module.scss';
import { Theme } from "cs2/bindings";
import { getModule } from "cs2/modding";

interface InfoSectionComponent {
    group: string;
    tooltipKeys: Array<string>;
    tooltipTags: Array<string>;
}

const InfoSectionTheme: Theme | any = getModule(
    "game-ui/game/components/selected-info-panel/shared-components/info-section/info-section.module.scss",
    "classes"
);

const InfoRowTheme: Theme | any = getModule(
    "game-ui/game/components/selected-info-panel/shared-components/info-row/info-row.module.scss",
    "classes"
)

const InfoSection: any = getModule(
    "game-ui/game/components/selected-info-panel/shared-components/info-section/info-section.tsx",
    "InfoSection"
)

const InfoRow: any = getModule(
    "game-ui/game/components/selected-info-panel/shared-components/info-row/info-row.tsx",
    "InfoRow"
)

function handleClick(eventName: string) {
    // This triggers an event on C# side and C# designates the method to implement.
}


// Bind the list of brand names from the BrandDataQuery system
const brandNames$ = bindValue<string[]>("BrandSelectorTesting", "BrandDataQuerySystem");
const brandNames = useValue(brandNames$) || []; // Use an empty array if not yet populated

export const SelectedInfoPanelCustomDropdownComponent = (componentList: any): any => {
    componentList["BrandSelectorTesting.Systems.SelectedInfoPanelCustomDropdownSystem"] = (e: InfoSectionComponent) => {
        const [dropdownVisible, setDropdownVisible] = useState(false);
        const [selectedBrand, setSelectedBrand] = useState<string>(''); // Keep track of selected brand
        const brandNames = useValue(brandNames$) || []; // Get brand names from the query

        // Handle item selection
        const handleBrandSelect = (brand: string) => {
            setSelectedBrand(brand); // Set selected brand
            trigger("BrandSelectorTesting", "SetCurrentPrefabBase", brand); // Trigger backend to change brand
        };

        return (
            <div className={styles.dropdownContainer}>
                {/* Input field that displays selected brand */}
                <input
                    type="text"
                    value={selectedBrand}
                    onClick={() => setDropdownVisible(!dropdownVisible)}
                    placeholder="Select a brand..."
                    className={styles.searchInput}
                />

                {/* Dropdown with list of brands */}
                {dropdownVisible && (
                    <div className={styles.dropdownContent}>
                        {brandNames.length > 0 ? (
                            brandNames.map((brand, index) => (
                                <div
                                    key={index}
                                    className={styles.dropdownItem}
                                    onClick={() => handleBrandSelect(brand)}
                                >
                                    {brand}
                                </div>
                            ))
                        ) : (
                            <div>No brands available</div>
                        )}
                    </div>
                )}
            </div>
        );
    };

    return componentList as any;
};
