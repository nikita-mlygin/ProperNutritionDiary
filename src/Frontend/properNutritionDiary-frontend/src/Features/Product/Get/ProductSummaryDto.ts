import { Macronutrients } from "../../Macronutrients/Macronutrients";

export interface ProductSummaryDto {
  name: string;
  macronutrients: Macronutrients;
  otherNutrients: { [key: string]: number };
  owner: string | null;
  id: {
    type: string;
    value: string;
  };
  allergens: string[];
  ingredients: string[];
}
