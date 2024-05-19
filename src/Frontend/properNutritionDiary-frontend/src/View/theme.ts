import { createTheme } from "@mui/material";

declare module "@mui/material/styles" {
  interface Palette {
    macros_calories: Palette["primary"];
    macros_proteins: Palette["primary"];
    macros_fats: Palette["primary"];
    macros_carbohydrates: Palette["primary"];
  }

  interface PaletteOptions {
    macros_calories?: PaletteOptions["primary"];
    macros_proteins?: PaletteOptions["primary"];
    macros_fats?: PaletteOptions["primary"];
    macros_carbohydrates?: PaletteOptions["primary"];
  }
}

const caloriesMain = "#FF8A65"; // более насыщенный пастельный цвет для калорий
const proteinsMain = "#4FC3F7"; // более насыщенный пастельный цвет для протеинов
const fatsMain = "#FFD54F"; // более насыщенный пастельный цвет для жиров
const carbohydratesMain = "#81C784"; // более насыщенный пастельный цвет для углеводов

let theme = createTheme();

// Создание темы Material-UI с палитрой макронутриентов
theme = createTheme({
  palette: {
    macros_calories: theme.palette.augmentColor({
      color: {
        main: caloriesMain,
      },
    }),
    macros_proteins: theme.palette.augmentColor({
      color: {
        main: proteinsMain,
      },
    }),
    macros_fats: theme.palette.augmentColor({
      color: {
        main: fatsMain,
      },
    }),
    macros_carbohydrates: theme.palette.augmentColor({
      color: {
        main: carbohydratesMain,
      },
    }),
  },
});

export default theme;
