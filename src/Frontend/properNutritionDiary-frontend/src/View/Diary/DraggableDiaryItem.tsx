import * as React from "react";
import { Draggable } from "react-beautiful-dnd";
import {
  ListItem,
  ListItemAvatar,
  ListItemText,
  Avatar,
  IconButton,
  TextField,
  Box,
  Typography,
  Stack,
  useTheme,
} from "@mui/material";
import EditIcon from "@mui/icons-material/Edit";
import DeleteIcon from "@mui/icons-material/Delete";
import CheckIcon from "@mui/icons-material/Check";
import { DiaryItem } from "../../Features/Diary/DiaryItem";

export type DraggableDiaryItemProps = {
  item: DiaryItem;
  index: number;
  editId: string | null;
  newWeight: number | null;
  handleEdit: (id: string, weight: number) => void;
  handleSave: (id: string) => void;
  handleDelete: (id: string) => void;
  setNewWeight: (weight: number | null) => void;
};

const DraggableDiaryItem: React.FC<DraggableDiaryItemProps> = ({
  item,
  index,
  editId,
  newWeight,
  handleEdit,
  handleSave,
  handleDelete,
  setNewWeight,
}) => {
  const theme = useTheme();

  return (
    <Draggable draggableId={item.product.id} index={index}>
      {(provided, snapshot) => (
        <Stack
          key={item.product.id}
          direction="row"
          spacing={2}
          alignItems="center"
          ref={provided.innerRef}
          {...provided.draggableProps}
          {...provided.dragHandleProps}
        >
          <Box flex={1}>
            <Typography variant="h6">{item.product.name}</Typography>
            <Stack direction="row" spacing={1}>
              <Typography
                variant="body2"
                color={theme.palette.macros_calories.main}
              >
                {item.product.macros.calories.toFixed(2)} kCal
              </Typography>
              <Typography
                variant="body2"
                color={theme.palette.macros_proteins.main}
              >
                {item.product.macros.proteins.toFixed(2)}g
              </Typography>
              <Typography
                variant="body2"
                color={theme.palette.macros_fats.main}
              >
                {item.product.macros.fats.toFixed(2)}g
              </Typography>
              <Typography
                variant="body2"
                color={theme.palette.macros_carbohydrates.main}
              >
                {item.product.macros.carbohydrates.toFixed(2)}g
              </Typography>
            </Stack>
          </Box>
          <Box>
            {editId === item.product.id ? (
              <TextField
                type="number"
                value={newWeight}
                onChange={(e) => setNewWeight(parseFloat(e.target.value))}
                onBlur={() => handleSave(item.product.id)}
                fullWidth
              />
            ) : (
              <Typography>{item.weight} g</Typography>
            )}
          </Box>
          <Box>
            {editId === item.product.id ? (
              <IconButton onClick={() => handleSave(item.product.id)}>
                <CheckIcon />
              </IconButton>
            ) : (
              <IconButton
                onClick={() => handleEdit(item.product.id, item.weight)}
              >
                <EditIcon />
              </IconButton>
            )}
            <IconButton onClick={() => handleDelete(item.product.id)}>
              <DeleteIcon />
            </IconButton>
          </Box>
        </Stack>
      )}
    </Draggable>
  );
};

export default DraggableDiaryItem;
