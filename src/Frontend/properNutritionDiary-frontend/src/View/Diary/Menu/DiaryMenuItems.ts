import { DiaryItem } from "../../../Features/Diary/DiaryItem";

export interface DiaryMenuItems {
  sections: { [section: string]: DiaryMenuSectionItem };
}

export interface DiaryMenuSectionItem {
  isConfirmed: boolean;
  list: DiaryItem[];
}
