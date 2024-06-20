import { Macronutrients } from "../../Macronutrients/Macronutrients";

export interface UserMenuDetails {
  id: string;
  createdAt: Date;
  dailyMenus: DetailsDay[];
}

export interface DetailsDay {
  dayNumber: number;
  breakfast: MenuItemDetails[];
  lunch: MenuItemDetails[];
  dinner: MenuItemDetails[];
}

export interface MenuItemDetails {
  id: string;
  productName: string;
  productIdentityType: ProductIdentityType;
  productId: string;
  macronutrients: Macronutrients;
  weight: number;
}

export enum ProductIdentityType {
  USDA,
  Barcode,
  Edamam,
  SystemItem,
}
