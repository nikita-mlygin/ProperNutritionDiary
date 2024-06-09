import * as React from "react";
import { Box, Typography, Stack, useTheme, TextField } from "@mui/material";
import { ProductSummaryDto } from "../../../Features/Product/Get/ProductSummaryDto";
import { DiaryItem } from "../../../Features/Diary/DiaryItem";

export type NonDraggableAvailableItemProps = {
  item: DiaryItem;
};

const DiaryMenuItem: React.FC<NonDraggableAvailableItemProps> = ({ item }) => {
  const theme = useTheme();

  return (
    <Stack
      key={item.product.id}
      direction="row"
      spacing={2}
      alignItems="center"
      sx={{ opacity: 0.5 }}
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
          <Typography variant="body2" color={theme.palette.macros_fats.main}>
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
        <Typography>{item.weight} g</Typography>
      </Box>
    </Stack>
  );
};

export default DiaryMenuItem;
