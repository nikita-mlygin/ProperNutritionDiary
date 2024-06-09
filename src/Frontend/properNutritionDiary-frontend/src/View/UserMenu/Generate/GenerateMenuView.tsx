import React, { FC, useEffect, useState } from "react";
import { TextField, Button, Container, Typography, Grid } from "@mui/material";

import {
  GenerateMenuConfiguration,
  NutrientFilterType,
} from "../../../Features/UserMenu/Generate/GenerateMenuConfiguration";
import DishAutocomplete from "../../Components/DishAutocomplete";
import ValueableAutocompleteWrapper from "../../Components/ValueableAutocompleteWrapper";
import { useLazyGenerateQuery } from "../../../Features/UserMenu/Api/UserMenuApi";
import UserMenuDetailsComponent from "../Get/UserMenuDetailsComponent";
import { dishTypes } from "./Data/dishTypes";
import { dietTypes } from "./Data/dietTypes";
import { cuisineTypes } from "./Data/cuisineTypes";
import { healthTypes } from "./Data/healthTypes";
import { useNavigate } from "react-router-dom";

const initialNutrientFilter: NutrientFilterType = {
  TargetCalories: undefined,
  TargetProtein: undefined,
  TargetFats: undefined,
  TargetCarbohydrates: undefined,
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

const GenerateMenuView: FC = () => {
  const [refresh, { data, error, isLoading, isFetching }] =
    useLazyGenerateQuery();
  const navigate = useNavigate();

  const [menuConfig, setMenuConfig] = useState<GenerateMenuConfiguration>(
    initialGenerateMenuConfig
  );

  const handleInputChange = (
    field: keyof GenerateMenuConfiguration,
    value: number | string | string[]
  ) => {
    setMenuConfig((prevConfig) => ({
      ...prevConfig,
      [field]: value,
    }));
  };

  const handleNutrientChange = (field: string, value: string) => {
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

  const handleDayCountChange = (event: React.ChangeEvent<HTMLInputElement>) => {
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

  useEffect(() => {
    if (data) {
      navigate("/userMenu/create", { state: { userMenuDetails: data } });
    }
  }, [data, navigate]);

  return (
    <Container>
      <Typography variant="h4" align="center" gutterBottom>
        Generate Menu Configuration
      </Typography>
      <Grid container spacing={2}>
        <Grid item xs={12}>
          <TextField
            name="TargetCalories"
            label="Target Calories"
            type="number"
            value={menuConfig.NutrientFilter?.TargetCalories || ""}
            onChange={(e) => {
              handleNutrientChange("TargetCalories", e.target.value);
            }}
            fullWidth
          />
        </Grid>
        <Grid item xs={12}>
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
        </Grid>

        <Grid item xs={12}>
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
        </Grid>
        <Grid item xs={12}>
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
        </Grid>
        <Grid item xs={12}>
          <DishAutocomplete
            dishOptions={cuisineTypes}
            selectedDishes={menuConfig.Cuisine}
            onDishDelete={function (index: number): void {
              handleChipDelete("Cuisine", index);
            }}
            onDishChange={function (newValue: string[]): void {
              handleInputChange("Cuisine", newValue);
            }}
            label={"Cousine"}
          />
        </Grid>
        <Grid item xs={12}>
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
        </Grid>
        <Grid item xs={12}>
          <TextField
            name="DayCount"
            label="Day Count"
            type="number"
            value={menuConfig.DayCount}
            onChange={handleDayCountChange}
            fullWidth
          />
        </Grid>
      </Grid>
      <Button
        variant="contained"
        color="primary"
        style={{ marginTop: "20px" }}
        onClick={() => refresh(menuConfig)}
        disabled={isLoading || isFetching || error !== undefined}
      >
        Generate Menu
      </Button>
      {data ? <UserMenuDetailsComponent userMenuDetails={data} /> : <></>}
    </Container>
  );
};

export default GenerateMenuView;
