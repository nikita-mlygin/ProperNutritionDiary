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
  const [weightGoal, setWeightGoal] = useState<string>(
    currentWeight === targetWeight
      ? "maintain"
      : currentWeight < targetWeight
      ? "increase"
      : "decrease"
  );
  const [weightChange, setWeightChange] = useState<number>(
    Math.abs(currentWeight - targetWeight)
  );

  const weightGoals = [
    { value: "maintain", display: "Maintain weight" },
    { value: "increase", display: "Increase weight" },
    { value: "decrease", display: "Decrease weight" },
  ];

  const handleWeightGoalChange = (event: ChangeEvent<{ value: unknown }>) => {
    setWeightGoal(event.target.value as string);
  };

  const handleWeightChange = (event: ChangeEvent<HTMLInputElement>) => {
    setWeightChange(Number(event.target.value));
  };

  const handleSave = () => {
    let newTargetWeight = currentWeight;
    if (weightGoal === "increase") {
      newTargetWeight = currentWeight + weightChange;
    } else if (weightGoal === "decrease") {
      newTargetWeight = currentWeight - weightChange;
    }
    onGoalChange(newTargetWeight);
    setEditing(false);
  };

  const goalText =
    weightGoal === "maintain"
      ? "Maintain weight"
      : weightGoal === "increase"
      ? `Increase weight by ${weightChange} kg`
      : `Decrease weight by ${weightChange} kg`;

  return (
    <Grid item xs={12}>
      {editing ? (
        <>
          <FormControl fullWidth>
            <InputLabel id="demo-simple-select-label">Weight Goal</InputLabel>
            <Select
              labelId="demo-simple-select-label"
              id="demo-simple-select"
              value={weightGoal}
              label="Weight Goal"
              onChange={handleWeightGoalChange}
            >
              {weightGoals.map((goal) => (
                <MenuItem key={goal.value} value={goal.value}>
                  {goal.display}
                </MenuItem>
              ))}
            </Select>
            {(weightGoal === "increase" || weightGoal === "decrease") && (
              <TextField
                label={`Weight to ${weightGoal}`}
                type="number"
                value={weightChange}
                onChange={handleWeightChange}
                fullWidth
                margin="normal"
              />
            )}
          </FormControl>

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
