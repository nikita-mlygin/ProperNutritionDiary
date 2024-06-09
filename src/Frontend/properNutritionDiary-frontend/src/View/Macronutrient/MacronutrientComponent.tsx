import React from "react";
import NutrientInfoComponent from "./NutrientInfoComponent";
import { Macronutrients as MacronutrientsType } from "../../Features/Macronutrients/Macronutrients";
import { useTheme } from "@mui/material";

interface MacronutrientsProps {
  macronutrients: MacronutrientsType;
}

const Macronutrients: React.FC<MacronutrientsProps> = ({ macronutrients }) => {
  const {
    macros_calories,
    macros_proteins,
    macros_fats,
    macros_carbohydrates,
  } = useTheme().palette;

  const formatNumber = (value: number): string => {
    return value.toFixed(2);
  };

  return (
    <>
      <NutrientInfoComponent
        label="Cal"
        value={formatNumber(macronutrients.calories)}
        color={macros_calories.main}
      />
      <NutrientInfoComponent
        label="Pro"
        value={formatNumber(macronutrients.proteins)}
        color={macros_proteins.main}
      />
      <NutrientInfoComponent
        label="Fat"
        value={formatNumber(macronutrients.fats)}
        color={macros_fats.main}
      />
      <NutrientInfoComponent
        label="Carb"
        value={formatNumber(macronutrients.carbohydrates)}
        color={macros_carbohydrates.main}
      />
    </>
  );
};

export default Macronutrients;
