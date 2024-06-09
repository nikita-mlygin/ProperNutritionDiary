import * as React from "react";
import { Stack } from "@mui/material";
import DiaryMenuItem from "./DiaryMenuItem";
import { DiaryItem } from "../../../Features/Diary/DiaryItem";

export type NonDraggableAvailableListProps = {
  items: DiaryItem[];
};

const DiaryMenuItemList: React.FC<NonDraggableAvailableListProps> = ({
  items,
}) => {
  return (
    <Stack spacing={2}>
      {items.map((item) => (
        <DiaryMenuItem key={item.id} item={item} />
      ))}
    </Stack>
  );
};

export default DiaryMenuItemList;
