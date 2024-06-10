import { Macronutrients } from "../../Macronutrients/Macronutrients";

export interface ProductSummaryDto {
  name: string;
  macronutrients: Macronutrients;
  owner: string | null;
  viewCount: number;
  id: {
    type: string;
    value: string;
  };
  useCount: number;
}
