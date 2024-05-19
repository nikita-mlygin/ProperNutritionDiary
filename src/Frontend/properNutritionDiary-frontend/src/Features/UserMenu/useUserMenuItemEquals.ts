import { useCallback } from "react";
import { MenuItemDetails } from "./Get/UserMenuDetails";

export const useUserMenuItemEquals = () => {
  return {
    isMenuItemDeepEquals: useCallback(
      (item1: MenuItemDetails, item2: MenuItemDetails): boolean => {
        if (item1.id !== item2.id) return false;

        const macro1 = item1.macronutrients;
        const macro2 = item2.macronutrients;

        if (macro1.calories !== macro2.calories) return false;
        if (macro1.proteins !== macro2.proteins) return false;
        if (macro1.fats !== macro2.fats) return false;
        if (macro1.carbohydrates !== macro2.carbohydrates) return false;

        if (item1.productName !== item2.productName) return false;
        if (item1.productIdentityType !== item2.productIdentityType)
          return false;
        if (item1.productId !== item2.productId) return false;
        if (item1.weight !== item2.weight) return false;

        return true;
      },
      []
    ),
    isMenuItemIdEqual: useCallback(
      (item1: MenuItemDetails, item2: MenuItemDetails): boolean => {
        return item1.id == item2.id;
      },
      []
    ),
  };
};
