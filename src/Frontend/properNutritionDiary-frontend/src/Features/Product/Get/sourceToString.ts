import { ProductIdentityType } from "./../../UserMenu/Get/UserMenuDetails";
export function sourceToString(type: ProductIdentityType): string {
  switch (type) {
    case ProductIdentityType.Barcode:
      return "OpenFoodFacts";

    case ProductIdentityType.USDA:
      return "USDA";

    case ProductIdentityType.Edamam:
      return "Edamam";

    case ProductIdentityType.SystemItem:
      return "System";
  }
}
