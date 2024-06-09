// ProductItem.js
import { FC } from "react";
import { Typography, Box, useTheme, PaletteColor } from "@mui/material";

const formatNumber = (value: number): string => {
  return value.toFixed(2);
};

const NutrientDisplay: FC<{ value: number; color: PaletteColor }> = ({
  value,
  color,
}) => {
  return (
    <Typography sx={{ color: color.main, mx: 1 }}>
      {formatNumber(value)}
    </Typography>
  );
};

interface ProductSearchItemProps {
  name: string;
  source: string;
  calories: number;
  proteins: number;
  fats: number;
  carbohydrates: number;
  onSelect: () => void;
}

const ProductSearchItem: FC<ProductSearchItemProps> = ({
  name,
  source,
  calories,
  proteins,
  fats,
  carbohydrates,
  onSelect,
}) => {
  const {
    macros_calories,
    macros_proteins,
    macros_fats,
    macros_carbohydrates,
  } = useTheme().palette;

  return (
    <Box
      display="flex"
      justifyContent="space-between"
      alignItems="center"
      padding="10px"
      borderRadius="5px"
      bgcolor="#f9f9f9"
      onClick={onSelect}
      sx={{ cursor: "pointer" }}
      mb={1}
    >
      <Box>
        <Typography variant="h6">{name}</Typography>
        <Typography variant="body2" color="textSecondary">
          Source: {source}
        </Typography>
      </Box>
      <Box display="flex" alignItems="center">
        <NutrientDisplay value={calories} color={macros_calories} />
        <NutrientDisplay value={proteins} color={macros_proteins} />
        <NutrientDisplay value={fats} color={macros_fats} />
        <NutrientDisplay value={carbohydrates} color={macros_carbohydrates} />
      </Box>
    </Box>
  );
};

export default ProductSearchItem;
