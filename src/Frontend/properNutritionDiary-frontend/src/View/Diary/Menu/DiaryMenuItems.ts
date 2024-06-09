import { DiaryItem } from "../../../Features/Diary/DiaryItem";
import { ProductSummaryDto } from "../../../Features/Product/Get/ProductSummaryDto";

export interface DiaryMenuItems {
  sections: { [section: string]: DiaryMenuSectionItem };
}

export interface DiaryMenuSectionItem {
  isConfirmed: boolean;
  list: DiaryItem[];
}
