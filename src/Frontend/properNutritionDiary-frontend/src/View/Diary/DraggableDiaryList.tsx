import React, { FC } from "react";
import { Droppable, OnDragEndResponder } from "react-beautiful-dnd";
import { DiaryItem } from "../../Features/Diary/DiaryItem";
import DraggableDiaryItem from "./DraggableDiaryItem";
import { Box } from "@mui/material";

export type DraggableDiaryListProps = {
  items: DiaryItem[];
  section: string;
  editId: string | null;
  newWeight: number | null;
  handleEdit: (id: string, weight: number) => void;
  handleSave: (id: string) => void;
  handleDelete: (id: string) => void;
  setNewWeight: (weight: number | null) => void;
  onItemClick: (id: string) => void;
};

const DraggableDiaryList: FC<DraggableDiaryListProps> = ({
  items,
  section,
  editId,
  newWeight,
  handleEdit,
  handleSave,
  handleDelete,
  setNewWeight,
  onItemClick,
}) => {
  return (
    <Droppable droppableId={section}>
      {(provided) => (
        <Box
          sx={{ minHeight: 30 }}
          ref={provided.innerRef}
          {...provided.droppableProps}
        >
          {items.map((item, index) => (
            <DraggableDiaryItem
              key={item.product.id.type + "_" + item.product.id.value}
              item={item}
              index={index}
              editId={editId}
              newWeight={newWeight}
              handleEdit={handleEdit}
              handleSave={handleSave}
              handleDelete={handleDelete}
              setNewWeight={setNewWeight}
              onClick={() =>
                onItemClick(item.product.id.type + "_" + item.product.id.value)
              }
            />
          ))}
          {provided.placeholder}
        </Box>
      )}
    </Droppable>
  );
};

export default DraggableDiaryList;
