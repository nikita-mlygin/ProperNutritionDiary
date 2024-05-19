import { useState } from "react";

interface NutrientCalculationState {
  calculateNutrientValue: (value: number) => number;
  toggleViewMode: (newValue: "per100g" | "total") => void;
  viewMode: "per100g" | "total";
}

const useNutrientCalculation = (
  initialWeight: number
): NutrientCalculationState => {
  const [viewMode, setViewMode] = useState<"per100g" | "total">("total");

  const calculateNutrientValue = (value: number): number => {
    return viewMode === "per100g" ? value : (value * initialWeight) / 100;
  };

  const toggleViewMode = (newValue: "per100g" | "total") => {
    setViewMode(newValue);
  };

  return {
    calculateNutrientValue,
    toggleViewMode,
    viewMode,
  };
};

export default useNutrientCalculation;
