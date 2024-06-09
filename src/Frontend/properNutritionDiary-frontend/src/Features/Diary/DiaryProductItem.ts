import { Macronutrients } from "../Macronutrients/Macronutrients";

export interface DiaryProductItem {
  id: string;
  name: string;
  macros: Macronutrients;
  source?: {
    type: string;
    value: string;
  };
  owner: string | null;
}
