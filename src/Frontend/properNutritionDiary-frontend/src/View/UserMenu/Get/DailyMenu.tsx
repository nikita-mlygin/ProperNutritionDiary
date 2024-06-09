import React, { useCallback, useEffect, useMemo } from "react";
import { Typography, Grid, Box, Fab } from "@mui/material";
import MenuItemList from "./UserMenuItemList";
import {
  DetailsDay,
  MenuItemDetails,
} from "../../../Features/UserMenu/Get/UserMenuDetails";
import { Macronutrients } from "../../../Features/Macronutrients/Macronutrients";
import MacronutrientsComponent from "../../Macronutrient/MacronutrientComponent";
import equal from "fast-deep-equal";
import AddIcon from "@mui/icons-material/Add";

export interface DailyMenuProps {
  dailyMenu: DetailsDay;
  dayNumber: number;
  onDrop: (
    item: MenuItemDetails,
    itemSource: [number, keyof DetailsDay],
    target: [number, keyof DetailsDay]
  ) => void;
  index: number;
  onDeleteItem: (
    item: MenuItemDetails,
    target: [number, keyof DetailsDay]
  ) => void;
  onDragChange: (nv: boolean) => void;
  setItems: (mutate: (prev: DetailsDay) => DetailsDay) => void;
  hasItemToAdd: boolean;
  onItemAdd: (target: [number, keyof DetailsDay]) => void;
}

const MealSection: React.FC<{
  meal: string;
  items: MenuItemDetails[];
  sectionIndex: [number, keyof DetailsDay];
  onDeleteItem: (
    item: MenuItemDetails,
    target: [number, keyof DetailsDay]
  ) => void;
  onDrop: (
    item: MenuItemDetails,
    itemSource: [number, keyof DetailsDay],
    target: [number, keyof DetailsDay]
  ) => void;
  setItems: (mutate: (prev: MenuItemDetails[]) => MenuItemDetails[]) => void;
  onDragChange: (nv: boolean) => void;
  hasItemToAdd: boolean;
  onItemAdd: (target: [number, keyof DetailsDay]) => void;
}> = React.memo(
  ({
    meal,
    items,
    sectionIndex,
    onDrop,
    setItems,
    onDeleteItem,
    onDragChange,
    hasItemToAdd,
    onItemAdd,
  }) => {
    return (
      <>
        <Grid
          item
          xs={1}
          sx={{
            position: "relative",
            "&:hover .MuiFab-root": hasItemToAdd
              ? { opacity: 1, pointerEvents: "auto" }
              : {},
            "&:hover .menuItemList": hasItemToAdd
              ? { opacity: 0.2, pointerEvents: "none" }
              : {},
          }}
        >
          <Fab
            color="primary"
            aria-label="add"
            onClick={() => onItemAdd(sectionIndex)}
            sx={{
              position: "absolute",
              top: "50%",
              left: "50%",
              transform: "translate(-50%, -50%)",
              opacity: 0,
              transition: "opacity 0.3s",
              pointerEvents: "none", // Prevent the button from capturing events when not visible
            }}
          >
            <AddIcon />
          </Fab>
          <div className="menuItemList">
            <MenuItemList
              onDeleteItem={onDeleteItem}
              meal={meal}
              items={items}
              sectionIndex={sectionIndex}
              onDrop={onDrop}
              setItems={setItems}
              onDragChange={onDragChange}
            />
          </div>
        </Grid>
      </>
    );
  }
);

const DailyMenu: React.FC<DailyMenuProps> = React.memo(
  ({
    dailyMenu,
    dayNumber,
    onDrop,
    index,
    setItems,
    onDeleteItem,
    onDragChange,
    hasItemToAdd,
    onItemAdd,
  }) => {
    console.log("Updated", dailyMenu, hasItemToAdd);

    const totalMacronutrientsForDay = useMemo((): Macronutrients => {
      const total: Macronutrients = {
        calories: 0,
        proteins: 0,
        fats: 0,
        carbohydrates: 0,
      };

      dailyMenu.breakfast
        .concat(dailyMenu.lunch, dailyMenu.dinner)
        .forEach((item) => {
          total.calories += (item.macronutrients.calories * item.weight) / 100;
          total.proteins += (item.macronutrients.proteins * item.weight) / 100;
          total.fats += (item.macronutrients.fats * item.weight) / 100;
          total.carbohydrates +=
            (item.macronutrients.carbohydrates * item.weight) / 100;
        });

      return total;
    }, [dailyMenu]);

    return (
      <div>
        <Typography variant="h6">Day {dayNumber}</Typography>
        <Box display="flex" gap={1}>
          <MacronutrientsComponent macronutrients={totalMacronutrientsForDay} />
        </Box>
        <Grid
          container
          columns={3}
          spacing={2}
          wrap="nowrap"
          alignItems="stretch"
        >
          <MealSection
            onItemAdd={onItemAdd}
            meal="Breakfast"
            items={dailyMenu.breakfast}
            sectionIndex={[index, "breakfast"]}
            onDrop={onDrop}
            onDragChange={onDragChange}
            hasItemToAdd={hasItemToAdd}
            setItems={(nv) => {
              setItems((prev) => ({
                ...prev,
                breakfast: nv(prev.breakfast),
              }));
            }}
            onDeleteItem={onDeleteItem}
          />
          <MealSection
            onItemAdd={onItemAdd}
            meal="Lunch"
            items={dailyMenu.lunch}
            sectionIndex={[index, "lunch"]}
            onDrop={onDrop}
            hasItemToAdd={hasItemToAdd}
            onDragChange={onDragChange}
            setItems={(nv) => {
              setItems((prev) => ({
                ...prev,
                lunch: nv(prev.lunch),
              }));
            }}
            onDeleteItem={onDeleteItem}
          />
          <MealSection
            onItemAdd={onItemAdd}
            meal="Dinner"
            items={dailyMenu.dinner}
            hasItemToAdd={hasItemToAdd}
            onDragChange={onDragChange}
            sectionIndex={[index, "dinner"]}
            onDrop={onDrop}
            setItems={(nv) => {
              setItems((prev) => ({
                ...prev,
                dinner: nv(prev.dinner),
              }));
            }}
            onDeleteItem={onDeleteItem}
          />
        </Grid>
      </div>
    );
  },
  (prev, next) => {
    return (
      prev.index === next.index &&
      prev.dayNumber === next.dayNumber &&
      prev.hasItemToAdd === next.hasItemToAdd &&
      equal(prev.dailyMenu, next.dailyMenu)
    );
  }
);

export default DailyMenu;
