// DailyMenu.jsx
import React, { useMemo } from "react";
import { Typography, Grid, Box } from "@mui/material";
import MenuItemList from "./UserMenuItemList";
import {
  DetailsDay,
  MenuItemDetails,
} from "../../../Features/UserMenu/Get/UserMenuDetails";
import { Macronutrients } from "../../../Features/Macronutrients/Macronutrients";
import MacronutrientsComponent from "../../Macronutrient/MacronutrientComponent";

interface DailyMenuProps {
  dailyMenu: DetailsDay;
  dayNumber: number;
  onDrop: (
    item: MenuItemDetails,
    itemSource: [number, keyof DetailsDay],
    target: [number, keyof DetailsDay]
  ) => void;
  index: number;
  setItems: (mutate: (prev: DetailsDay) => DetailsDay) => void;
}

const DailyMenu: React.FC<DailyMenuProps> = React.memo(
  ({ dailyMenu, dayNumber, onDrop, index, setItems }) => {
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
          <Grid item xs={1}>
            <MenuItemList
              meal="Breakfast"
              items={dailyMenu.breakfast}
              sectionIndex={[index, "breakfast"]}
              onDrop={onDrop}
              setItems={(nv) => {
                setItems((prev) => ({
                  ...prev,
                  breakfast: nv(prev.breakfast),
                }));
              }}
            />
          </Grid>
          <Grid item xs={1}>
            <MenuItemList
              meal="Lunch"
              items={dailyMenu.lunch}
              sectionIndex={[index, "lunch"]}
              onDrop={onDrop}
              setItems={(nv) => {
                setItems((prev) => ({ ...prev, lunch: nv(prev.lunch) }));
              }}
            />
          </Grid>
          <Grid item xs={1}>
            <MenuItemList
              meal="Dinner"
              items={dailyMenu.dinner}
              sectionIndex={[index, "dinner"]}
              onDrop={onDrop}
              setItems={(nv) => {
                setItems((prev) => ({ ...prev, dinner: nv(prev.dinner) }));
              }}
            />
          </Grid>
        </Grid>
      </div>
    );
  },
  (prev, next) => {
    if (prev.index != next.index) return false;
    if (prev.dayNumber != next.dayNumber) return false;

    if (JSON.stringify(prev.dailyMenu) !== JSON.stringify(next.dailyMenu))
      return false;

    return true;
  }
);

export default DailyMenu;
