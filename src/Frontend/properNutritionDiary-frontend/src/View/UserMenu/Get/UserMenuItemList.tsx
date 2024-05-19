import React, { FC } from "react";
import {
  DetailsDay,
  MenuItemDetails,
} from "../../../Features/UserMenu/Get/UserMenuDetails";
import { Stack, Typography } from "@mui/material";
import { useDrop } from "react-dnd";
import MenuItem from "./MenuItem";
import { useUserMenuItemEquals } from "../../../Features/UserMenu/useUserMenuItemEquals";

interface MenuItemListProps {
  sectionIndex: [number, keyof DetailsDay];
  setItems: (
    mutateFunction: (prev: MenuItemDetails[]) => MenuItemDetails[]
  ) => void;
  onDrop: (
    item: MenuItemDetails,
    itemSource: [number, keyof DetailsDay],
    target: [number, keyof DetailsDay]
  ) => void;
  meal: string;
  items: MenuItemDetails[];
}

const MainMenuItemList: FC<
  MenuItemListProps & {
    isEquals: (item1: MenuItemDetails, item2: MenuItemDetails) => boolean;
  }
> = React.memo(
  ({ sectionIndex, meal, items, onDrop, setItems }) => {
    console.log("itemListUpdated");

    const [, drop] = useDrop(
      () => ({
        accept: "UserItem",
        drop: (item: {
          item: () => MenuItemDetails;
          source: () => [number, keyof DetailsDay];
        }) => {
          onDrop(item.item(), item.source(), sectionIndex);
        },
      }),
      [items]
    );

    const updateItem = (index: number, newItem: MenuItemDetails) => {
      setItems((prevItems) => {
        const updatedItems = [...prevItems];
        updatedItems[index] = newItem;
        return updatedItems;
      });
    };

    return (
      <Stack spacing={2} ref={drop} style={{ height: "100%" }}>
        <Typography variant="subtitle2">{meal}</Typography>
        {items.map((item, index) => (
          <MenuItem
            srcIndex={sectionIndex}
            key={item.id}
            item={item}
            setItem={(newItem) => updateItem(index, newItem(item))}
            onDelete={() => {}}
          />
        ))}
      </Stack>
    );
  },
  (prev, next) => {
    if (prev.items.length != next.items.length) return false;

    for (let i = 0; i < prev.items.length; i++) {
      if (!prev.isEquals(prev.items[i], next.items[i])) return false;
    }

    return false;
  }
);

const MenuItemList: FC<MenuItemListProps> = (props) => {
  const { isMenuItemDeepEquals } = useUserMenuItemEquals();

  return <MainMenuItemList {...props} isEquals={isMenuItemDeepEquals} />;
};

export default MenuItemList;
