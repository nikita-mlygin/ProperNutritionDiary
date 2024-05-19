import React, { FC, useState } from "react";
import { TextField, Button, Container, Typography, Grid } from "@mui/material";

import {
  GenerateMenuConfiguration,
  NutrientFilterType,
} from "../../../Features/UserMenu/Generate/GenerateMenuConfiguration";
import DishAutocomplete from "../../Components/DishAutocomplete";
import ValueableAutocompleteWrapper from "../../Components/ValueableAutocompleteWrapper";
import { useLazyGenerateQuery } from "../../../Features/UserMenu/Api/UserMenuApi";
import UserMenuDetailsComponent from "../Get/UserMenuDetailsComponent";

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

const cuisineTypes: { label: string; value: string }[] = [
  { label: "American", value: "american" },
  { label: "Asian", value: "asian" },
  { label: "British", value: "british" },
  { label: "Caribbean", value: "caribbean" },
  { label: "Central Europe", value: "central europe" },
  { label: "Chinese", value: "chinese" },
  { label: "Eastern Europe", value: "eastern europe" },
  { label: "French", value: "french" },
  { label: "Greek", value: "greek" },
  { label: "Indian", value: "indian" },
  { label: "Italian", value: "italian" },
  { label: "Japanese", value: "japanese" },
  { label: "Korean", value: "korean" },
  { label: "Kosher", value: "kosher" },
  { label: "Mediterranean", value: "mediterranean" },
  { label: "Mexican", value: "mexican" },
  { label: "Middle Eastern", value: "middle eastern" },
  { label: "Nordic", value: "nordic" },
  { label: "South American", value: "south american" },
  { label: "South East Asian", value: "south east asian" },
  { label: "World", value: "world" },
];

const dietTypes: { label: string; value: string }[] = [
  { label: "Balanced", value: "balanced" },
  { label: "High Fiber", value: "high-fiber" },
  { label: "High Protein", value: "high-protein" },
  { label: "Low Carb", value: "low-carb" },
  { label: "Low Fat", value: "low-fat" },
  { label: "Low Sodium", value: "low-sodium" },
];

const healthTypes: { label: string; value: string; description: string }[] = [
  {
    label: "Alcohol Cocktail",
    value: "alcohol-cocktail",
    description: "Describes an alcoholic cocktail",
  },
  {
    label: "Alcohol Free",
    value: "alcohol-free",
    description: "No alcohol used or contained",
  },
  {
    label: "Celery Free",
    value: "celery-free",
    description: "Does not contain celery or derivatives",
  },
  {
    label: "Crustacean Free",
    value: "crustacean-free",
    description:
      "Does not contain crustaceans (shrimp, lobster etc.) or derivatives",
  },
  {
    label: "Dairy Free",
    value: "dairy-free",
    description: "No dairy; no lactose",
  },
  {
    label: "DASH",
    value: "DASH",
    description: "Dietary Approaches to Stop Hypertension diet",
  },
  {
    label: "Egg Free",
    value: "egg-free",
    description: "No eggs or products containing eggs",
  },
  {
    label: "Fish Free",
    value: "fish-free",
    description: "No fish or fish derivatives",
  },
  {
    label: "FODMAP Free",
    value: "fodmap-free",
    description: "Does not contain FODMAP foods",
  },
  {
    label: "Gluten Free",
    value: "gluten-free",
    description: "No ingredients containing gluten",
  },
  {
    label: "Immuno Supportive",
    value: "immuno-supportive",
    description:
      "Recipes which fit a science-based approach to eating to strengthen the immune system",
  },
  {
    label: "Keto Friendly",
    value: "keto-friendly",
    description: "Maximum 7 grams of net carbs per serving",
  },
  {
    label: "Kidney Friendly",
    value: "kidney-friendly",
    description:
      "Per serving – phosphorus less than 250 mg AND potassium less than 500 mg AND sodium less than 500 mg",
  },
  {
    label: "Kosher",
    value: "kosher",
    description:
      "Contains only ingredients allowed by the kosher diet. However it does not guarantee kosher preparation of the ingredients themselves",
  },
  {
    label: "Low Potassium",
    value: "low-potassium",
    description: "Less than 150mg per serving",
  },
  {
    label: "Low Sugar",
    value: "low-sugar",
    description:
      "No simple sugars – glucose, dextrose, galactose, fructose, sucrose, lactose, maltose",
  },
  {
    label: "Lupine Free",
    value: "lupine-free",
    description: "Does not contain lupine or derivatives",
  },
  {
    label: "Mediterranean",
    value: "Mediterranean",
    description: "Mediterranean diet",
  },
  { label: "Mollusk Free", value: "mollusk-free", description: "No mollusks" },
  {
    label: "Mustard Free",
    value: "mustard-free",
    description: "Does not contain mustard or derivatives",
  },
  {
    label: "No Oil Added",
    value: "No-oil-added",
    description:
      "No oil added except to what is contained in the basic ingredients",
  },
  {
    label: "Paleo",
    value: "paleo",
    description:
      "Excludes what are perceived to be agricultural products; grains, legumes, dairy products, potatoes, refined salt, refined sugar, and processed oils",
  },
  {
    label: "Peanut Free",
    value: "peanut-free",
    description: "No peanuts or products containing peanuts",
  },
  {
    label: "Pescatarian",
    value: "pecatarian",
    description:
      "Does not contain meat or meat based products, can contain dairy and fish",
  },
  {
    label: "Pork Free",
    value: "pork-free",
    description: "Does not contain pork or derivatives",
  },
  {
    label: "Red Meat Free",
    value: "red-meat-free",
    description:
      "Does not contain beef, lamb, pork, duck, goose, game, horse, and other types of red meat or products containing red meat.",
  },
  {
    label: "Sesame Free",
    value: "sesame-free",
    description: "Does not contain sesame seed or derivatives",
  },
  {
    label: "Shellfish Free",
    value: "shellfish-free",
    description: "No shellfish or shellfish derivatives",
  },
  {
    label: "Soy Free",
    value: "soy-free",
    description: "No soy or products containing soy",
  },
  {
    label: "Sugar Conscious",
    value: "sugar-conscious",
    description: "Less than 4g of sugar per serving",
  },
  { label: "Sulfite Free", value: "sulfite-free", description: "No Sulfites" },
  {
    label: "Tree Nut Free",
    value: "tree-nut-free",
    description: "No tree nuts or products containing tree nuts",
  },
  {
    label: "Vegan",
    value: "vegan",
    description: "No meat, poultry, fish, dairy, eggs or honey",
  },
  {
    label: "Vegetarian",
    value: "vegetarian",
    description: "No meat, poultry, or fish",
  },
  {
    label: "Wheat Free",
    value: "wheat-free",
    description: "No wheat, can have gluten though",
  },
];

const dishTypes: { label: string; value: string }[] = [
  { label: "Alcohol Cocktail", value: "alcohol cocktail" },
  { label: "Biscuits and Cookies", value: "biscuits and cookies" },
  { label: "Bread", value: "bread" },
  { label: "Cereals", value: "cereals" },
  { label: "Condiments and Sauces", value: "condiments and sauces" },
  { label: "Desserts", value: "desserts" },
  { label: "Drinks", value: "drinks" },
  { label: "Egg", value: "egg" },
  { label: "Ice Cream and Custard", value: "ice cream and custard" },
  { label: "Main Course", value: "main course" },
  { label: "Pancake", value: "pancake" },
  { label: "Pasta", value: "pasta" },
  { label: "Pastry", value: "pastry" },
  { label: "Pies and Tarts", value: "pies and tarts" },
  { label: "Pizza", value: "pizza" },
  { label: "Preps", value: "preps" },
  { label: "Preserve", value: "preserve" },
  { label: "Salad", value: "salad" },
  { label: "Sandwiches", value: "sandwiches" },
  { label: "Seafood", value: "seafood" },
  { label: "Side Dish", value: "side dish" },
  { label: "Soup", value: "soup" },
  { label: "Special Occasions", value: "special occasions" },
  { label: "Starter", value: "starter" },
  { label: "Sweets", value: "sweets" },
];

const GenerateMenuView: FC = () => {
  const [refresh, { data, error, isLoading, isFetching }] =
    useLazyGenerateQuery();

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
            dishOptions={[
              {
                label: "Aboba",
                value: {
                  name: "Aboba",
                  amount: null,
                },
              },
              {
                label: "Aboba2",
                value: {
                  name: "Aboba2",
                  amount: null,
                },
              },
              {
                label: "Aboba3",
                value: {
                  name: "Aboba3",
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
      >
        Generate Menu
      </Button>
      {data ? <UserMenuDetailsComponent userMenuDetails={data} /> : <></>}
    </Container>
  );
};

export default GenerateMenuView;
