import { getModule } from "cs2/modding";
import { Theme, FocusKey, UniqueFocusKey } from "cs2/bindings";
import { bindValue, trigger, useValue } from "cs2/api";
import { useLocalization } from "cs2/l10n";
import { VanillaComponentResolver } from "mods/VanillaComponentResolver/VanillaComponentResolver";
import styles from './DropdownComponent.module.scss';
import { useState } from "react";

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

// New Dropdown Style Theme
const DropdownStyle: Theme | any = getModule("game-ui/menu/themes/dropdown.module.scss", "classes");

function handleClick(eventName: string) {
    // This triggers an event on C# side and C# designates the method to implement.
}

// Bind the list of items for the dropdown
const items$ = bindValue<string[]>("BrandSelectorTesting", "AvailableBrands");

// Trigger item selection
export function itemSelected(item: string) {
    trigger("BrandSelectorTesting", "ItemSelected", item); // Correct mod name
}

// Translation function using localization hook
export function translate(key: string) {
    const { translate } = useLocalization();
    return translate(key);
}

export const SelectedInfoPanelDropdownComponent = (componentList: any): any => {
    componentList["BrandSelectorTesting.Systems.DropdownSystem"] = (e: InfoSectionComponent) => {
        const [dropdownVisible, setDropdownVisible] = useState(false); // Control dropdown visibility
        const [searchTerm, setSearchTerm] = useState<string | undefined>(""); // Search term state
        const items = useValue(items$) || []; // Bind the items from the "BrandSelectorTesting" data source

        // Toggle the visibility of the dropdown
        const toggleDropdown = () => {
            setDropdownVisible(!dropdownVisible);
        };

        // Handle search input and update searchTerm state
        const handleSearch = (event: React.ChangeEvent<HTMLInputElement>) => {
            setSearchTerm(event.target.value || undefined); // Ensure undefined is passed if input is cleared
        };

        // Handle item click (select an item)
        const handleItemClick = (item: string) => {
            itemSelected(item); // Trigger the event for the selected item
            setDropdownVisible(false); // Optionally close the dropdown after item selection
        };

        // Filter items based on search term (case-insensitive)
        const filteredItems = items.filter(item => (searchTerm || '').toLowerCase().includes(item.toLowerCase()));

        // Applying the DropdownStyle theme classes to elements
        return (
            <InfoSection focusKey={VanillaComponentResolver.instance.FOCUS_DISABLED} disableFocus={true} className={InfoSectionTheme.infoSection}>
                <InfoRow
                    left={"Brand Selector Testing"}
                    right={
                        <div className={DropdownStyle.dropdownContainer || styles.dropdownContainer}>
                            {/* Input field to trigger dropdown visibility and handle search */}
                            <input
                                type="text"
                                placeholder={translate("BRANDSELECTORTESTING.SearchPlaceholder") || undefined}
                                className={DropdownStyle.searchInput || styles.searchInput}
                                value={searchTerm || ''}
                                onClick={toggleDropdown}
                                onChange={handleSearch}
                            />

                            {/* Render dropdown content only when dropdownVisible is true */}
                            {dropdownVisible && (
                                <div className={DropdownStyle.dropdownContent || styles.dropdownContent}>
                                    {/* Map through filteredItems to display the dropdown list */}
                                    {filteredItems.map((item, index) => (
                                        <div
                                            key={index}
                                            className={DropdownStyle.dropdownItem || styles.dropdownItem}
                                            onClick={() => handleItemClick(item)} // Trigger item selection when clicked
                                        >
                                            {item} {/* Display the item */}
                                        </div>
                                    ))}
                                </div>
                            )}
                        </div>
                    }
                    tooltip={"Select a brand"}
                    uppercase={true}
                    disableFocus={true}
                    subRow={false}
                    className={InfoRowTheme.infoRow}
                />
            </InfoSection>
        );
    }

    return componentList as any;
}
