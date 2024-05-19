import React from "react";
import TextField from "@mui/material/TextField";
import IconButton from "@mui/material/IconButton";
import SaveIcon from "@mui/icons-material/Save";

interface WeightEditProps {
  tempWeight: string;
  handleWeightChange: (value: string) => void;
  handleWeightBlur: () => void;
  onSave: () => void;
}

const WeightEditComponent: React.FC<WeightEditProps> = ({
  tempWeight,
  handleWeightChange,
  handleWeightBlur,
  onSave,
}) => {
  return (
    <>
      <TextField
        type="text"
        value={tempWeight}
        variant="standard"
        onChange={(e) => handleWeightChange(e.target.value)}
        onBlur={handleWeightBlur}
        InputProps={{ inputProps: { min: 0, step: 0.01 } }}
        sx={{ marginRight: 1 }}
      />
      <IconButton onClick={onSave}>
        <SaveIcon />
      </IconButton>
    </>
  );
};

export default WeightEditComponent;
