import { FC, useState, ChangeEvent } from "react";
import { TextField, Button, Container, Typography, Grid } from "@mui/material";
import {
  GenerateMenuConfiguration,
  NutrientFilterType,
} from "../../../Features/UserMenu/Generate/GenerateMenuConfiguration";
import DishAutocomplete from "../../../View/Components/DishAutocomplete";
import ValueableAutocompleteWrapper from "../../../View/Components/ValueableAutocompleteWrapper";
import { dishTypes } from "../../../View/UserMenu/Generate/Data/dishTypes";
import { dietTypes } from "../../../View/UserMenu/Generate/Data/dietTypes";
import { cuisineTypes } from "../../../View/UserMenu/Generate/Data/cuisineTypes";
import { healthTypes } from "../../../View/UserMenu/Generate/Data/healthTypes";

const initialNutrientFilter: NutrientFilterType = {
  TargetCalories: undefined,
  TargetProtein: undefined,
  TargetFats: undefined,
  TargetCarbohydrates: undefined,
  Other: {},
};

const initialGenerateMenuConfig: GenerateMenuConfiguration = {
  NutrientFilter: initialNutrientFilter,
  Dish: [],
  Breakfast: { MealType: [], Dish: [] },
  Lunch: { MealType: [], Dish: [] },
  Dinner: { MealType: [], Dish: [] },
  Cuisine: [],
  Diet: [],
  Health: [],
  DayCount: 1,
};

const steps = [
  "Enter Target Calories",
  "Enter Nutrients",
  "Enter Macros",
  "Select Dishes",
  "Select Diet Types",
  "Select Cuisine Types",
  "Select Health Types",
  "Enter Day Count",
];

const GenerateMenuForm: FC = () => {
  const [activeStep, setActiveStep] = useState<number>(0);
  const [menuConfig, setMenuConfig] = useState<GenerateMenuConfiguration>(
    initialGenerateMenuConfig
  );

  const handleNext = (): void => {
    if (activeStep < steps.length - 1) {
      setActiveStep((prevActiveStep) => prevActiveStep + 1);
    }
  };

  const handleBack = (): void => {
    setActiveStep((prevActiveStep) => prevActiveStep - 1);
  };

  const handleInputChange = (
    field: keyof GenerateMenuConfiguration,
    value: number | string | string[]
  ) => {
    setMenuConfig((prevConfig) => ({
      ...prevConfig,
      [field]: value,
    }));
  };

  const handleNutrientChange = (
    field: keyof NutrientFilterType,
    value: string
  ) => {
    setMenuConfig((prevConfig) => ({
      ...prevConfig,
      NutrientFilter: {
        ...prevConfig.NutrientFilter,
        [field]: Number.parseInt(value),
      },
    }));
  };

  const handleNutrientOtherChange = (value: { [key: string]: number }) => {
    setMenuConfig((prevConfig) => ({
      ...prevConfig,
      NutrientFilter: {
        ...prevConfig.NutrientFilter,
        Other: value,
      },
    }));
  };

  const handleDayCountChange = (event: ChangeEvent<HTMLInputElement>) => {
    const { value } = event.target;
    setMenuConfig((prevConfig) => ({
      ...prevConfig,
      DayCount: parseInt(value, 10),
    }));
  };

  const handleChipDelete = (
    field: keyof GenerateMenuConfiguration,
    index: number
  ) => {
    const fieldValue = menuConfig[field];

    if (!Array.isArray(fieldValue)) return;

    const updatedValues = [...fieldValue];

    updatedValues.splice(index, 1);
    setMenuConfig((prevConfig) => ({
      ...prevConfig,
      [field]: updatedValues,
    }));
  };

  const renderStepContent = (step: number): JSX.Element => {
    switch (step) {
      case 0:
        return (
          <TextField
            name="TargetCalories"
            label="Target Calories"
            type="number"
            value={menuConfig.NutrientFilter?.TargetCalories || ""}
            onChange={(e) =>
              handleNutrientChange("TargetCalories", e.target.value)
            }
            fullWidth
            margin="normal"
          />
        );
      case 1:
        return (
          <>
            <TextField
              name="TargetProtein"
              label="Target Protein"
              type="number"
              value={menuConfig.NutrientFilter?.TargetProtein || ""}
              onChange={(e) =>
                handleNutrientChange("TargetProtein", e.target.value)
              }
              fullWidth
              margin="normal"
            />
            <TextField
              name="TargetFats"
              label="Target Fats"
              type="number"
              value={menuConfig.NutrientFilter?.TargetFats || ""}
              onChange={(e) =>
                handleNutrientChange("TargetFats", e.target.value)
              }
              fullWidth
              margin="normal"
            />
            <TextField
              name="TargetCarbohydrates"
              label="Target Carbohydrates"
              type="number"
              value={menuConfig.NutrientFilter?.TargetCarbohydrates || ""}
              onChange={(e) =>
                handleNutrientChange("TargetCarbohydrates", e.target.value)
              }
              fullWidth
              margin="normal"
            />
          </>
        );
      case 2:
        return (
          <ValueableAutocompleteWrapper
            label="Macros"
            dishOptions={[
              {
                label: "Mg",
                value: {
                  name: "Magni",
                  amount: null,
                },
              },
            ]}
            initValues={[]}
            onChange={(nv) => {
              handleNutrientOtherChange(
                nv.reduce((acc, curr) => {
                  if (curr.value) acc[curr.valueName] = curr.value;
                  return acc;
                }, {} as { [key: string]: number })
              );

              console.log(nv);
            }}
            parseValueFn={<T extends number>(value: string) => {
              const parsedValue = Number.parseInt(value);
              return isNaN(parsedValue) ? null : (parsedValue as T);
            }}
          />
        );
      case 3:
        return (
          <DishAutocomplete
            dishOptions={dishTypes}
            selectedDishes={menuConfig.Dish}
            onDishDelete={function (index: number): void {
              handleChipDelete("Dish", index);
            }}
            onDishChange={function (newValue: string[]): void {
              handleInputChange("Dish", newValue);
            }}
            label={"Dish"}
          />
        );
      case 4:
        return (
          <DishAutocomplete
            dishOptions={dietTypes}
            selectedDishes={menuConfig.Diet}
            onDishDelete={function (index: number): void {
              handleChipDelete("Diet", index);
            }}
            onDishChange={function (newValue: string[]): void {
              handleInputChange("Diet", newValue);
            }}
            label={"Diet"}
          />
        );
      case 5:
        return (
          <DishAutocomplete
            dishOptions={cuisineTypes}
            selectedDishes={menuConfig.Cuisine}
            onDishDelete={function (index: number): void {
              handleChipDelete("Cuisine", index);
            }}
            onDishChange={function (newValue: string[]): void {
              handleInputChange("Cuisine", newValue);
            }}
            label={"Cuisine"}
          />
        );
      case 6:
        return (
          <DishAutocomplete
            dishOptions={healthTypes}
            selectedDishes={menuConfig.Health}
            onDishDelete={function (index: number): void {
              handleChipDelete("Health", index);
            }}
            onDishChange={function (newValue: string[]): void {
              handleInputChange("Health", newValue);
            }}
            label={"Health"}
          />
        );
      case 7:
        return (
          <TextField
            name="DayCount"
            label="Day Count"
            type="number"
            value={menuConfig.DayCount}
            onChange={handleDayCountChange}
            fullWidth
            margin="normal"
          />
        );
      default:
        return <div>Unknown step</div>;
    }
  };

  return (
    <Container>
      <Typography variant="h4" align="center" gutterBottom>
        Generate Menu Configuration
      </Typography>
      <Typography variant="h6" align="center" gutterBottom>
        Step {activeStep + 1}: {steps[activeStep]}
      </Typography>
      <Grid container spacing={2}>
        <Grid item xs={12}>
          {renderStepContent(activeStep)}
        </Grid>
      </Grid>
      <Grid container spacing={2} justifyContent="space-between" marginTop={2}>
        <Grid item>
          <Button
            variant="contained"
            color="primary"
            disabled={activeStep === 0}
            onClick={handleBack}
          >
            Back
          </Button>
        </Grid>
        <Grid item>
          <Button variant="contained" color="primary" onClick={handleNext}>
            {activeStep === steps.length - 1 ? "Submit" : "Next"}
          </Button>
        </Grid>
      </Grid>
    </Container>
  );
};

export default GenerateMenuForm;
