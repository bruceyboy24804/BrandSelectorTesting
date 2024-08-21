import { getModule } from "cs2/modding";
import { Theme, FocusKey, UniqueFocusKey } from "cs2/bindings";
import { bindValue, trigger, useValue } from "cs2/api";
import { VanillaComponentResolver } from "mods/VanillaComponentResolver/VanillaComponentResolver";
import styles from './DropdownComponent.module.scss';
import { useCallback, useState } from "react";


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
);

const InfoSection: any = getModule(
    "game-ui/game/components/selected-info-panel/shared-components/info-section/info-section.tsx",
    "InfoSection"
);

const InfoRow: any = getModule(
    "game-ui/game/components/selected-info-panel/shared-components/info-row/info-row.tsx",
    "InfoRow"
);

// Bind the list of brand names from BrandDataQuery
const items$ = bindValue<string[]>("BrandSelectorTesting", "BrandDataQuery");

// Trigger the backend system to change the current prefab base
const itemSelected = (item: string) => {
    trigger("BrandSelectorTesting", item); // Trigger the backend method to set the current prefab base
}

export const DropdownComponent = (componentList: any): any => {
    componentList["BrandSelectorTesting.Systems.DropdownSystem"] = (e: InfoSectionComponent) => {
        const [dropdownVisible, setDropdownVisible] = useState(false); // Control dropdown visibility
        const [searchTerm, setSearchTerm] = useState<string>(''); // Track the search term
        const items = useValue(items$) || []; // Bind the items from the "BrandSelectorTesting" data source
        const [selectedItem, setSelectedItem] = useState<string>(''); // Track the selected item

        // Memoize the dropdown toggle
        const toggleDropdown = useCallback(() => {
            setDropdownVisible(prev => !prev);
        }, []);

        // Handle item click (select an item)
        const handleItemClick = (item: string) => {
            setSelectedItem(item); // Set selected item to input
            setSearchTerm(""); // Clear the search term
            itemSelected(item); // Trigger the backend to switch the prefab base
            setDropdownVisible(false); // Optionally close the dropdown after item selection
        };

        // Handle search term update
        const handleSearch = (event: React.ChangeEvent<HTMLInputElement>) => {
            const value = event.target.value;
            setSearchTerm(value); // Update search term on typing

            if (value === "") {
                setSelectedItem(""); // Clear the selected item if input is cleared
                setDropdownVisible(true); // Show the dropdown again when input is cleared
            }
        };

        // Filter items based on search term
        const filteredItems = items.filter(item =>
            item.toLowerCase().includes(searchTerm.toLowerCase())
        );

        return (
            <InfoSection focusKey={VanillaComponentResolver.instance.FOCUS_DISABLED} disableFocus={true} className={styles.infoSection}>
                <InfoRow
                    left={"Brand Selector"} // Label for the row
                    right={
                        <div className={styles.dropdownContainer}>
                            {/* Input field that acts as the dropdown */}
                            <input
                                type="text"
                                value={searchTerm || selectedItem} // Show the search term or the selected item
                                onClick={toggleDropdown} // Show the dropdown on input click
                                onChange={handleSearch} // Handle search input and allow deletion
                                placeholder="Select a brand..."
                                className={styles.searchInput} // Style like a dropdown
                            />

                            {/* Render dropdown content only when dropdownVisible is true */}
                            {dropdownVisible && (
                                <div className={styles.dropdownContent}>
                                    {/* Map through filtered items to display the dropdown list */}
                                    {filteredItems.length > 0 ? (
                                        filteredItems.map((item, index) => (
                                            <div
                                                key={index}
                                                className={styles.dropdownItem}
                                                onClick={() => handleItemClick(item)} // Trigger item selection when clicked
                                            >
                                                {item} {/* Display the item */}
                                            </div>
                                        ))
                                    ) : (
                                        <div className={styles.noResults}>No brands available</div> // Show this if no items are available
                                    )}
                                </div>
                            )}
                        </div>
                    }
                    tooltip={"Select a brand"} // Tooltip for additional info
                    uppercase={true} // Optional styling
                    disableFocus={true} // Disable focus handling
                    subRow={false} // No sub-row
                    className={styles.infoRow} // Use custom styles
                />
            </InfoSection>
        );
    }

    return componentList as any;
};
