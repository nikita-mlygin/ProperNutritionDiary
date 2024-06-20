import { FC } from "react";
import { Droppable } from "react-beautiful-dnd";
import { DiaryItem } from "../../Features/Diary/DiaryItem";
import DraggableDiaryItem from "./DraggableDiaryItem";
import { Box } from "@mui/material";
import { ProductIdentityType } from "../../Features/UserMenu/Get/UserMenuDetails";

export type DraggableDiaryListProps = {
  items: DiaryItem[];
  section: string;
  editId: { type: ProductIdentityType; value: string } | null;
  newWeight: number | null;
  handleEdit: (
    id: { type: ProductIdentityType; value: string },
    weight: number
  ) => void;
  handleSave: (id: { type: ProductIdentityType; value: string }) => void;
  handleDelete: (id: { type: ProductIdentityType; value: string }) => void;
  setNewWeight: (weight: number | null) => void;
  onItemClick: (id: { type: ProductIdentityType; value: string }) => void;
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
              onClick={() => onItemClick(item.product.id)}
            />
          ))}
          {provided.placeholder}
        </Box>
      )}
    </Droppable>
  );
};

export default DraggableDiaryList;
