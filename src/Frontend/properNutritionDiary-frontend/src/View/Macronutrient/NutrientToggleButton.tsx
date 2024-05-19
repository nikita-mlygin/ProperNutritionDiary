import React from "react";
import { ToggleButton, ToggleButtonGroup } from "@mui/material";
import { useTheme } from "@mui/material";

interface NutrientToggleButtonProps {
  value: "per100g" | "total";
  onClick: (value: "per100g" | "total") => void;
}

const NutrientToggleButton: React.FC<NutrientToggleButtonProps> = ({
  value,
  onClick,
}) => {
  const theme = useTheme();

  return (
    <ToggleButtonGroup
      value={value}
      exclusive
      onChange={(_, newMode: "per100g" | "total" | null) => {
        if (newMode) onClick(newMode);
      }}
      sx={{
        marginTop: 1,
        height: 24,
        ".MuiToggleButton-root": {
          padding: "4px 8px",
          fontSize: "0.75rem",
          "&.Mui-selected": {
            backgroundColor: theme.palette.primary.main,
            color: theme.palette.primary.contrastText,
          },
        },
      }}
      size="small"
      color="primary"
    >
      <ToggleButton value="total">Total</ToggleButton>
      <ToggleButton value="per100g">Per 100g</ToggleButton>
    </ToggleButtonGroup>
  );
};

export default NutrientToggleButton;
