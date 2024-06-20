import { Macronutrients } from "../../Macronutrients/Macronutrients";
import { ProductIdentityType } from "../../UserMenu/Get/UserMenuDetails";

export interface ProductSummaryDto {
  name: string;
  macronutrients: Macronutrients;
  otherNutrients: { [key: string]: number };
  owner: string | null;
  id: {
    type: ProductIdentityType;
    value: string;
  };
  allergens: string[];
  ingredients: string[];
}
