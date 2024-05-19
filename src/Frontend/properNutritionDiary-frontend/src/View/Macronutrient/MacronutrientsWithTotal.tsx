import React from "react";
import { Macronutrients } from "../../Features/Macronutrients/Macronutrients";
import { default as MacronutrientsComponent } from "./MacronutrientComponent";

interface MacronutrientsWithTotalProps {
  weight: number;
  viewMode: "per100g" | "total";
  macronutrients: Macronutrients;
}

const MacronutrientsWithTotal: React.FC<MacronutrientsWithTotalProps> = ({
  weight,
  viewMode,
  macronutrients,
}) => {
  const calculateMacronutrientsValeu = (
    value: Macronutrients
  ): Macronutrients => {
    return viewMode === "per100g"
      ? value
      : {
          calories: calculateNutrientValue(value.calories),
          proteins: calculateNutrientValue(value.proteins),
          fats: calculateNutrientValue(value.fats),
          carbohydrates: calculateNutrientValue(value.carbohydrates),
        };
  };

  const calculateNutrientValue = (value: number): number => {
    return viewMode === "per100g" ? value : (value * weight) / 100;
  };

  return (
    <MacronutrientsComponent
      macronutrients={calculateMacronutrientsValeu(macronutrients)}
    />
  );
};

export default MacronutrientsWithTotal;
