import React, { FC, useState, ChangeEvent, useEffect } from "react";
import {
  Container,
  Typography,
  Grid,
  TextField,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
} from "@mui/material";
import { Line } from "react-chartjs-2";
import { Chart, registerables } from "chart.js";
import UserGoalView from "../UserStats/UserGoalView";
import UserIntakeView from "./Profile/UserIntakeView";
import UserCharts from "./Profile/UserCharts";
import StaticInfoForm from "./Profile/StaticInfoForm";

Chart.register(...registerables);

const UserPage: FC = () => {
  const [username, setUsername] = useState<string>("JohnDoe");
  const [goal, setGoal] = useState<string>("Maintain weight");
  const [calories, setCalories] = useState<number>(2000);
  const [protein, setProtein] = useState<number>(150);
  const [fats, setFats] = useState<number>(70);
  const [carbs, setCarbs] = useState<number>(250);
  const [height, setHeight] = useState<number>(175);
  const [lifestyle, setLifestyle] = useState<string>("Moderate");
  const [gender, setGender] = useState<string>("Male");
  const [currentWeight, setCurrentWeight] = useState<number>(75);
  const [newWeight, setNewWeight] = useState<number>(0);

  // State for weight and nutrient statistics
  const [weightData, setWeightData] = useState<number[]>([]);
  const [nutrientData, setNutrientData] = useState<{
    calories: number[];
    protein: number[];
    fats: number[];
    carbs: number[];
  }>({ calories: [], protein: [], fats: [], carbs: [] });
  const [selectedChart, setSelectedChart] = useState<string>("nutrient");

  // Dialog states
  const [openGoalDialog, setOpenGoalDialog] = useState<boolean>(false);
  const [openCaloriesDialog, setOpenCaloriesDialog] = useState<boolean>(false);

  // Handlers for opening/closing dialogs
  const handleOpenGoalDialog = () => setOpenGoalDialog(true);
  const handleCloseGoalDialog = () => setOpenGoalDialog(false);
  const handleOpenCaloriesDialog = () => setOpenCaloriesDialog(true);
  const handleCloseCaloriesDialog = () => setOpenCaloriesDialog(false);

  // Handlers for changing user data
  const handleInputChange =
    (setter: React.Dispatch<React.SetStateAction<any>>) =>
    (event: ChangeEvent<HTMLInputElement>) => {
      setter(event.target.value);
    };

  // Handler for submitting new weight
  const handleWeightSubmit = () => {
    setWeightData([...weightData, newWeight]);
    setNewWeight(0);
  };

  // Sample data for charts
  useEffect(() => {
    setWeightData([75, 74, 73, 74, 75, 74]);
    setNutrientData({
      calories: [2000, 1800, 2200, 2100, 2000, 1900],
      protein: [150, 140, 160, 155, 150, 145],
      fats: [70, 60, 80, 75, 70, 65],
      carbs: [250, 230, 270, 260, 250, 240],
    });
  }, []);

  const weightChartData = {
    labels: weightData.map((_, i) => `Day ${i + 1}`),
    datasets: [
      {
        label: "Weight",
        data: weightData,
        fill: false,
        borderColor: "blue",
      },
    ],
  };

  const nutrientChartData = {
    labels: nutrientData.calories.map((_, i) => `Day ${i + 1}`),
    datasets: [
      {
        label: "Calories",
        data: nutrientData.calories,
        fill: false,
        borderColor: "red",
      },
      {
        label: "Protein",
        data: nutrientData.protein,
        fill: false,
        borderColor: "green",
      },
      {
        label: "Fats",
        data: nutrientData.fats,
        fill: false,
        borderColor: "yellow",
      },
      {
        label: "Carbs",
        data: nutrientData.carbs,
        fill: false,
        borderColor: "purple",
      },
    ],
  };

  const deviationChartData = {
    labels: nutrientData.calories.map((_, i) => `Day ${i + 1}`),
    datasets: [
      {
        label: "Calories Deviation",
        data: nutrientData.calories.map((cal) => cal - calories),
        fill: false,
        borderColor: "red",
      },
      {
        label: "Protein Deviation",
        data: nutrientData.protein.map((prot) => prot - protein),
        fill: false,
        borderColor: "green",
      },
      {
        label: "Fats Deviation",
        data: nutrientData.fats.map((fat) => fat - fats),
        fill: false,
        borderColor: "yellow",
      },
      {
        label: "Carbs Deviation",
        data: nutrientData.carbs.map((carb) => carb - carbs),
        fill: false,
        borderColor: "purple",
      },
    ],
  };

  return (
    <Container>
      <Typography variant="h4" align="center" gutterBottom>
        User Profile
      </Typography>

      <Grid container spacing={2}>
        <Grid item xs={12}>
          <Typography variant="h6">Username: {username}</Typography>
        </Grid>
        <UserGoalView
          currentWeight={80}
          targetWeight={90}
          onGoalChange={(nv) => {
            console.log(nv);
          }}
        />
        <UserIntakeView
          calories={123}
          protein={12}
          fats={44}
          carbs={2}
          onIntakeChange={(nv) => console.log(nv)}
        />
        <Grid item xs={12}>
          <Typography variant="h6">Static Information</Typography>
          {/* <StaticInfoForm
            height={height}
            lifestyle={lifestyle}
            gender={gender}
            onChange={(field, value) => {}}
          /> */}
          <TextField
            label="Height"
            type="number"
            fullWidth
            margin="normal"
            value={height}
            onChange={handleInputChange(setHeight)}
          />
          <TextField
            label="Lifestyle"
            type="text"
            fullWidth
            margin="normal"
            value={lifestyle}
            onChange={handleInputChange(setLifestyle)}
          />
          <TextField
            label="Gender"
            type="text"
            fullWidth
            margin="normal"
            value={gender}
            onChange={handleInputChange(setGender)}
          />
        </Grid>
        <Grid item xs={12}>
          <Typography variant="h6">
            Current Weight: {currentWeight} kg
          </Typography>
          <TextField
            label="New Weight"
            type="number"
            fullWidth
            margin="normal"
            value={newWeight}
            onChange={handleInputChange(setNewWeight)}
          />
          <Button
            variant="contained"
            color="primary"
            onClick={handleWeightSubmit}
          >
            Submit New Weight
          </Button>
        </Grid>
        <Grid item xs={12}>
          <Typography variant="h6">User Statistics</Typography>
          <UserCharts
            deviationChartData={deviationChartData}
            nutrientChartData={nutrientChartData}
            weightChartData={weightChartData}
            selectedChart={selectedChart}
            onChartChange={(nv) => setSelectedChart(nv)}
          />
        </Grid>
      </Grid>
    </Container>
  );
};

export default UserPage;
