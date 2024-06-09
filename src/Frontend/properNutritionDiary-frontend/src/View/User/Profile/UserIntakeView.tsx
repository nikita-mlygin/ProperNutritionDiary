import React, { FC, useState, ChangeEvent } from "react";
import { Grid, Typography, Button, TextField } from "@mui/material";

interface UserIntakeProps {
  calories: number;
  protein: number;
  fats: number;
  carbs: number;
  onIntakeChange: (
    newCalories: number,
    newProtein: number,
    newFats: number,
    newCarbs: number
  ) => void;
}

const UserIntakeView: FC<UserIntakeProps> = ({
  calories,
  protein,
  fats,
  carbs,
  onIntakeChange,
}) => {
  const [editing, setEditing] = useState(false);
  const [newCalories, setNewCalories] = useState<number>(calories);
  const [newProtein, setNewProtein] = useState<number>(protein);
  const [newFats, setNewFats] = useState<number>(fats);
  const [newCarbs, setNewCarbs] = useState<number>(carbs);

  const handleIntakeChange =
    (setter: React.Dispatch<React.SetStateAction<number>>) =>
    (event: ChangeEvent<HTMLInputElement>) => {
      setter(Number(event.target.value));
    };

  const handleSave = () => {
    onIntakeChange(newCalories, newProtein, newFats, newCarbs);
    setEditing(false);
  };

  return (
    <Grid item xs={12}>
      {editing ? (
        <>
          <TextField
            margin="dense"
            label="Calories"
            type="number"
            fullWidth
            value={newCalories}
            onChange={handleIntakeChange(setNewCalories)}
          />
          <TextField
            margin="dense"
            label="Protein"
            type="number"
            fullWidth
            value={newProtein}
            onChange={handleIntakeChange(setNewProtein)}
          />
          <TextField
            margin="dense"
            label="Fats"
            type="number"
            fullWidth
            value={newFats}
            onChange={handleIntakeChange(setNewFats)}
          />
          <TextField
            margin="dense"
            label="Carbs"
            type="number"
            fullWidth
            value={newCarbs}
            onChange={handleIntakeChange(setNewCarbs)}
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
          <Typography variant="h6">
            Daily Intake: {calories} Calories, {protein}g Protein, {fats}g Fats,{" "}
            {carbs}g Carbs
          </Typography>
          <Button
            variant="contained"
            color="primary"
            onClick={() => setEditing(true)}
          >
            Edit Intake
          </Button>
        </>
      )}
    </Grid>
  );
};

export default UserIntakeView;
