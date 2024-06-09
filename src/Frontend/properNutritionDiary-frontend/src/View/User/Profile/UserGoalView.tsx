import React, { FC, useState, ChangeEvent } from "react";
import {
  Grid,
  Typography,
  Button,
  TextField,
  MenuItem,
  Select,
  FormControl,
  InputLabel,
} from "@mui/material";

interface UserGoalProps {
  currentWeight: number;
  targetWeight: number;
  onGoalChange: (newTargetWeight: number) => void;
}

const UserGoalView: FC<UserGoalProps> = ({
  currentWeight,
  targetWeight,
  onGoalChange,
}) => {
  const [editing, setEditing] = useState(false);
  const [newTargetWeight, setNewTargetWeight] = useState(targetWeight);

  const handleWeightChange = (event: ChangeEvent<HTMLInputElement>) => {
    setNewTargetWeight(Number(event.target.value));
  };

  const handleSave = () => {
    onGoalChange(newTargetWeight);
    setEditing(false);
  };

  const goalText =
    currentWeight === targetWeight
      ? "Maintain weight"
      : currentWeight < targetWeight
      ? `Increase weight by ${targetWeight - currentWeight} kg`
      : `Decrease weight by ${currentWeight - targetWeight} kg`;

  return (
    <Grid item xs={12}>
      {editing ? (
        <>
          <TextField
            autoFocus
            margin="dense"
            label="Target Weight (kg)"
            type="number"
            fullWidth
            value={newTargetWeight}
            onChange={handleWeightChange}
          />
          <Button onClick={handleSave} color="primary">
            Save
          </Button>
          <Button onClick={() => setEditing(false)} color="secondary">
            Cancel
          </Button>
        </>
      ) : (
        <>
          <Typography variant="h6">Goal: {goalText}</Typography>
          <Button
            variant="contained"
            color="primary"
            onClick={() => setEditing(true)}
          >
            Edit Goal
          </Button>
        </>
      )}
    </Grid>
  );
};

export default UserGoalView;
