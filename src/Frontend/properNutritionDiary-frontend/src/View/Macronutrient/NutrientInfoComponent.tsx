import React from "react";
import Typography from "@mui/material/Typography";

interface NutrientInfoProps {
  label: string;
  value: string;
  color: string;
}

const NutrientInfoComponent: React.FC<NutrientInfoProps> = ({
  label,
  value,
  color,
}) => {
  return (
    <Typography variant="body2">
      <b>{label}:</b> <span style={{ color }}>{value}</span>
    </Typography>
  );
};

export default NutrientInfoComponent;
