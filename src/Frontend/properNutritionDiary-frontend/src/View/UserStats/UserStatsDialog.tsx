import React, { useState, ChangeEvent } from "react";
import {
  Button,
  TextField,
  Stepper,
  Step,
  StepLabel,
  Container,
  Typography,
  FormControl,
  FormLabel,
  RadioGroup,
  FormControlLabel,
  Radio,
  MenuItem,
  Select,
} from "@mui/material";
import { useLocation, useNavigate } from "react-router-dom";

// Define types for Static and Dynamic User Stats
interface StaticUserStats {
  userId: string;
  height: number;
  lifeStyle: string;
  gender: string;
}

interface DynamicUserStats {
  userId: string;
  weight: number;
  startDateTime: string;
  endDateTime: string;
  targetWeight: number;
}

const steps = [
  "Enter Height",
  "Enter Life Style",
  "Enter Gender",
  "Enter Weight",
  "Choose Weight Goal",
];

const lifeStyles = [
  { display: "Sedentary (little or no exercise)", value: "sedentary" },
  {
    display: "Lightly active (light exercise/sports 1-3 days/week)",
    value: "lightly_active",
  },
  {
    display: "Moderately active (moderate exercise/sports 3-5 days/week)",
    value: "moderately_active",
  },
  {
    display: "Very active (hard exercise/sports 6-7 days a week)",
    value: "very_active",
  },
  {
    display:
      "Super active (very hard exercise/sports & physical job or 2x training)",
    value: "super_active",
  },
];

const weightGoals = [
  { display: "Maintain Weight", value: "maintain" },
  { display: "Increase Weight", value: "increase" },
  { display: "Decrease Weight", value: "decrease" },
];

const UserStatsDialog: React.FC = () => {
  const [activeStep, setActiveStep] = useState<number>(0);
  const [staticUserStats, setStaticUserStats] = useState<StaticUserStats>({
    userId: "",
    height: 0,
    lifeStyle: "",
    gender: "",
  });
  const [dynamicUserStats, setDynamicUserStats] = useState<DynamicUserStats>({
    userId: "",
    weight: 0,
    startDateTime: "",
    endDateTime: "",
    targetWeight: 0,
  });

  const [weightGoal, setWeightGoal] = useState<string>("maintain");
  const [weightChange, setWeightChange] = useState<number>(0);
  const navigate = useNavigate();

  const location = useLocation();

  const handleNext = (): void => {
    if (activeStep < steps.length - 1) {
      setActiveStep((prevActiveStep) => prevActiveStep + 1);
    } else {
      // Calculate target weight
      let finalTargetWeight = dynamicUserStats.weight;
      if (weightGoal === "increase") {
        finalTargetWeight += weightChange;
      } else if (weightGoal === "decrease") {
        finalTargetWeight -= weightChange;
      }
      setDynamicUserStats({
        ...dynamicUserStats,
        targetWeight: finalTargetWeight,
      });
      // Submit form
      navigate("/user/profile");
    }
    // Handle form submission here
  };

  const handleLifeStyleChange = (e: ChangeEvent<{ value: unknown }>): void => {
    setStaticUserStats({
      ...staticUserStats,
      lifeStyle: e.target.value as string,
    });
  };

  const handleBack = (): void => {
    setActiveStep((prevActiveStep) => prevActiveStep - 1);
  };

  const handleChange = (
    e: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>,
    statType: "static" | "dynamic"
  ): void => {
    const { name, value } = e.target;
    const parsedValue =
      name === "height" || name === "weight" || name === "targetWeight"
        ? parseFloat(value)
        : value;
    if (statType === "static") {
      setStaticUserStats({ ...staticUserStats, [name]: parsedValue });
    } else {
      setDynamicUserStats({ ...dynamicUserStats, [name]: parsedValue });
    }
  };

  const handleGenderChange = (e: ChangeEvent<HTMLInputElement>): void => {
    setStaticUserStats({ ...staticUserStats, gender: e.target.value });
  };

  const handleWeightGoalChange = (e: ChangeEvent<{ value: unknown }>): void => {
    setWeightGoal(e.target.value as string);
  };

  const handleWeightChange = (e: ChangeEvent<HTMLInputElement>): void => {
    setWeightChange(parseFloat(e.target.value));
  };

  const isNextEnabled = (): boolean => {
    switch (activeStep) {
      case 0:
        return staticUserStats.height > 0;
      case 1:
        return staticUserStats.lifeStyle !== "";
      case 2:
        return staticUserStats.gender !== "";
      case 3:
        return dynamicUserStats.weight > 0;
      case 4:
        if (weightGoal === "maintain") return true;
        return weightChange > 0;
      default:
        return false;
    }
  };

  const renderStepContent = (step: number): JSX.Element => {
    switch (step) {
      case 0:
        return (
          <TextField
            label="Height"
            name="height"
            type="number"
            value={staticUserStats.height}
            onChange={(e) => handleChange(e, "static")}
            fullWidth
            margin="normal"
          />
        );
      case 1:
        return (
          <FormControl fullWidth margin="normal">
            <Select
              label="Life Style"
              value={staticUserStats.lifeStyle}
              onChange={handleLifeStyleChange}
            >
              {lifeStyles.map((style) => (
                <MenuItem key={style.value} value={style.value}>
                  {style.display}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
        );
      case 2:
        return (
          <FormControl component="fieldset">
            <FormLabel component="legend">Gender</FormLabel>
            <RadioGroup
              name="gender"
              value={staticUserStats.gender}
              onChange={handleGenderChange}
            >
              <FormControlLabel value="male" control={<Radio />} label="Male" />
              <FormControlLabel
                value="female"
                control={<Radio />}
                label="Female"
              />
              <FormControlLabel
                value="other"
                control={<Radio />}
                label="Other"
              />
            </RadioGroup>
          </FormControl>
        );
      case 3:
        return (
          <TextField
            label="Weight"
            name="weight"
            type="number"
            value={dynamicUserStats.weight}
            onChange={(e) => handleChange(e, "dynamic")}
            fullWidth
            margin="normal"
          />
        );
      case 4:
        return (
          <FormControl fullWidth margin="normal">
            <Select
              label="Weight Goal"
              value={weightGoal}
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
        );
      default:
        return <div>Unknown step</div>;
    }
  };

  return (
    <Container maxWidth="sm">
      <Typography variant="h4" align="center" gutterBottom>
        User Stats Form
      </Typography>
      <Stepper activeStep={activeStep} alternativeLabel>
        {steps.map((label) => (
          <Step key={label}>
            <StepLabel>{label}</StepLabel>
          </Step>
        ))}
      </Stepper>
      {renderStepContent(activeStep)}
      <div style={{ marginTop: 20 }}>
        {activeStep > 0 && (
          <Button onClick={handleBack} style={{ marginRight: 10 }}>
            Back
          </Button>
        )}
        <Button
          variant="contained"
          color="primary"
          onClick={handleNext}
          disabled={!isNextEnabled()}
        >
          {activeStep === steps.length - 1 ? "Finish" : "Next"}
        </Button>
      </div>
    </Container>
  );
};

export default UserStatsDialog;
