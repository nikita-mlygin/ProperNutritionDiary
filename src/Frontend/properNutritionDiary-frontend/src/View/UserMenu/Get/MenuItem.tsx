import React, { FC, useEffect } from "react";
import {
  DetailsDay,
  MenuItemDetails,
} from "../../../Features/UserMenu/Get/UserMenuDetails";
import {
  Divider,
  IconButton,
  ListItem,
  ListItemText,
  Typography,
  Box,
  Paper,
  Grid,
} from "@mui/material";
import { useDrag } from "react-dnd";
import EditIcon from "@mui/icons-material/Edit";
import DeleteIcon from "@mui/icons-material/Delete"; // Импортируем иконку для удаления
import useWeightEditing from "../../../Features/Product/useWeightEditing";
import WeightEditComponent from "../../Product/Weight/WeightEditComponent";
import useNutrientCalculation from "../../../Features/Macronutrients/useNutrientCalculationHook";
import NutrientToggleButton from "../../Macronutrient/NutrientToggleButton";
import MacronutrientsWithTotal from "../../Macronutrient/MacronutrientsWithTotal";

interface MenuItemProps {
  srcIndex: [number, keyof DetailsDay];
  item: MenuItemDetails;
  setItem: (
    prevMutateFunction: (prev: MenuItemDetails) => MenuItemDetails
  ) => void;
  onDelete: () => void; // Обработчик для удаления продукта
  onDragChange: (nv: boolean) => void;
}

const MenuItem: FC<MenuItemProps> = React.memo(
  ({ item, srcIndex, setItem, onDelete, onDragChange }) => {
    console.log("Item updated", srcIndex);

    const {
      weight,
      tempWeight,
      isEditing,
      startEditing,
      cancelEditing,
      handleWeightChange,
      handleWeightBlur,
    } = useWeightEditing(item.weight, (nv) => {
      setItem((prev: MenuItemDetails) => ({
        ...prev,
        weight: nv,
      }));
    });

    const { toggleViewMode, viewMode } = useNutrientCalculation(item.weight);

    const [{ isDragging }, drag] = useDrag(
      () => ({
        type: "UserItem",
        item: {
          item: () => item,
          source: () => srcIndex,
        },
        collect: (monitor) => ({ isDragging: monitor.isDragging() }),
      }),
      [item, srcIndex]
    );

    useEffect(() => {
      onDragChange(isDragging);
    }, [isDragging, onDragChange]);

    return (
      <Paper>
        <ListItem
          ref={drag}
          draggable
          sx={{
            cursor: "grab",
            display: "flex",
            flexDirection: "column",
            alignItems: "flex-start",
          }}
        >
          <Grid
            container
            sx={{ display: "flex", justifyContent: "space-between", width: 1 }}
          >
            <Grid item xs>
              <ListItemText primary={item.productName} secondary={<></>} />
            </Grid>
            <Grid item xs="auto">
              <IconButton size="small" onClick={onDelete}>
                <DeleteIcon />
              </IconButton>
            </Grid>
          </Grid>
          <Box sx={{ display: "flex", alignItems: "center", width: 1 }}>
            {isEditing ? (
              <>
                <WeightEditComponent
                  tempWeight={tempWeight}
                  handleWeightChange={handleWeightChange}
                  handleWeightBlur={handleWeightBlur}
                  onSave={cancelEditing}
                />
              </>
            ) : (
              <Box
                sx={{
                  display: "flex",
                  justifyContent: "space-between",
                  width: 1,
                  alignItems: "center",
                }}
              >
                <Box display="flex" alignItems="center">
                  <Typography variant="body2" sx={{ marginRight: 1 }}>
                    {weight} g
                  </Typography>
                  <IconButton onClick={startEditing}>
                    <EditIcon />
                  </IconButton>
                </Box>

                <NutrientToggleButton
                  value={viewMode}
                  onClick={toggleViewMode}
                />
              </Box>
            )}
          </Box>
          <Divider sx={{ width: "100%", margin: "8px 0" }} />
          <Box sx={{ display: "flex", gap: 2 }}>
            <MacronutrientsWithTotal
              weight={item.weight}
              viewMode={viewMode}
              macronutrients={item.macronutrients}
            />
          </Box>
        </ListItem>
      </Paper>
    );
  },
  (prev, next) => {
    return prev.item.id == next.item.id && prev.item.weight == next.item.weight;
  }
);

export default MenuItem;
