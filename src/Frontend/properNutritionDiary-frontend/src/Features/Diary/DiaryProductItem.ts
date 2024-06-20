import { Macronutrients } from "../Macronutrients/Macronutrients";
import { ProductIdentityType } from "../UserMenu/Get/UserMenuDetails";

export interface DiaryProductItem {
  id: {
    value: string;
    type: ProductIdentityType;
  };
  name: string;
  macros: Macronutrients;
}
