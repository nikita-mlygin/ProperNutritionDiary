import React from "react";
import { Box, Chip, Autocomplete, TextField } from "@mui/material";

interface DishOption {
  value: string;
  label: string;
}

interface DishAutocompleteProps {
  label: string;
  dishOptions: DishOption[];
  selectedDishes: string[];
  onDishDelete: (index: number) => void;
  onDishChange: (newValue: string[]) => void;
}

const DishAutocomplete: React.FC<DishAutocompleteProps> = ({
  dishOptions,
  selectedDishes,
  onDishDelete,
  onDishChange,
  label,
}) => {
  const selectedDishOptions: DishOption[] = selectedDishes.map(
    (selectedDish) => {
      const existingOption = dishOptions.find(
        (dishOption) => dishOption.value === selectedDish
      );
      if (existingOption) {
        return existingOption;
      } else {
        return { value: selectedDish, label: selectedDish };
      }
    }
  );

  return (
    <>
      <Box mb={2}>
        {selectedDishOptions.map((option, index) => (
          <Chip
            key={option.value}
            label={option.label}
            onDelete={() => {
              onDishDelete(index);
            }}
          />
        ))}
      </Box>
      <Autocomplete
        multiple
        options={dishOptions}
        value={selectedDishOptions}
        getOptionLabel={(option) => option.label}
        onChange={(_, newValue) => {
          onDishChange(newValue.map((option) => option.value));
        }}
        renderTags={() => <></>}
        renderInput={(params) => (
          <TextField {...params} label={label} fullWidth />
        )}
      />
    </>
  );
};

export default DishAutocomplete;
