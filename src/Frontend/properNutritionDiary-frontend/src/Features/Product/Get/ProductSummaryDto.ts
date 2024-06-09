import { Macronutrients } from "../../Macronutrients/Macronutrients";

export interface ProductSummaryDto {
  id: string;
  name: string;
  macronutrients: Macronutrients;
  owner: string | null;
  viewCount: number;
  externalSource?: {
    type: string;
    value: string;
  };
  useCount: number;
}
