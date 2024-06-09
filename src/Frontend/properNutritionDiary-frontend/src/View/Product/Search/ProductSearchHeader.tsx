import { Box, Typography, useTheme } from "@mui/material";
import { FC } from "react";

const ProductSearchHeader: FC = () => {
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
      padding="10px"
      bgcolor="#f1f1f1"
      fontWeight="bold"
    >
      <Typography variant="body1" style={{ flex: 1 }}>
        Product name
      </Typography>
      <Box display="flex" justifyContent="space-around" gap={2}>
        <Typography variant="body1" sx={{ color: macros_calories.main }}>
          Calories
        </Typography>
        <Typography variant="body1" sx={{ color: macros_proteins.main }}>
          Proteins
        </Typography>
        <Typography variant="body1" sx={{ color: macros_fats.main }}>
          Fats
        </Typography>
        <Typography variant="body1" sx={{ color: macros_carbohydrates.main }}>
          Carbohydrates
        </Typography>
      </Box>
    </Box>
  );
};

export default ProductSearchHeader;
