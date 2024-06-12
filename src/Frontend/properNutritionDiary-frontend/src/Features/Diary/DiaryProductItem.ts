import { Macronutrients } from "../Macronutrients/Macronutrients";

export interface DiaryProductItem {
  id: {
    value: string;
    type: string;
  };
  name: string;
  macros: Macronutrients;
  owner: string | null;
}
