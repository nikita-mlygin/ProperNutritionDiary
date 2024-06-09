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
  onDeleteItem: (
    item: MenuItemDetails,
    target: [number, keyof DetailsDay]
  ) => void;
  onDragChange: (nv: boolean) => void;
  items: MenuItemDetails[];
}

const MainMenuItemList: FC<
  MenuItemListProps & {
    isEquals: (item1: MenuItemDetails, item2: MenuItemDetails) => boolean;
  }
> = React.memo(
  ({
    sectionIndex,
    meal,
    items,
    onDrop,
    setItems,
    onDeleteItem,
    onDragChange,
  }) => {
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
      <Stack
        spacing={2}
        ref={drop}
        sx={{
          height: "100%",
        }}
      >
        <Typography variant="subtitle2">{meal}</Typography>
        {items.map((item, index) => (
          <MenuItem
            onDragChange={onDragChange}
            srcIndex={sectionIndex}
            key={item.id}
            item={item}
            setItem={(newItem) => updateItem(index, newItem(item))}
            onDelete={() => {
              onDeleteItem(item, sectionIndex);
            }}
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
  console.log("List updated", props.sectionIndex);

  const { isMenuItemDeepEquals } = useUserMenuItemEquals();

  return <MainMenuItemList {...props} isEquals={isMenuItemDeepEquals} />;
};

export default MenuItemList;
