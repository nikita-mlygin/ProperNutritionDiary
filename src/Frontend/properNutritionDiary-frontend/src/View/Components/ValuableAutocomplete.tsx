import React, { useState } from "react";
import { Box, Chip, Autocomplete, TextField } from "@mui/material";

// Обобщённый тип для опции
export interface Valueable<T> {
  label: string;
  value: { amount: T | null; name: string };
}

// Обобщённый тип для компонента
interface ValueableAutocompleteProps<T> {
  dishOptions: Valueable<T>[];
  selectedDishes: Valueable<T>[];
  onDishDelete: (index: number) => void;
  onDishChange: (newValue: Valueable<T>[]) => void;
  // Функция-преобразователь для преобразования строки в значение типа T
  parseValueFn: (value: string) => T | null;
  label: string;
}

const ValueableAutocomplete = <T extends string | number>({
  dishOptions,
  selectedDishes,
  onDishDelete,
  onDishChange,
  parseValueFn,
  label,
}: ValueableAutocompleteProps<T>) => {
  const [editedIndex, setEditedIndex] = useState(-1);

  const handleChipDelete = (index: number) => {
    onDishDelete(index);
  };

  const handleChipChange = (index: number, newValue: string) => {
    const newSelectedDishes = [...selectedDishes];
    const value = parseValueFn(newValue);
    if (value !== null) {
      newSelectedDishes[index].value.amount = value;
      onDishChange(newSelectedDishes);
    }
  };

  return (
    <>
      <Box mb={2} sx={{ display: "flex" }}>
        {selectedDishes.map((option, index) => (
          <Chip
            key={index}
            label={
              index === editedIndex
                ? `${option.value.amount ?? "<not set>"}`
                : `${option.label} - ${option.value.amount}`
            }
            onDelete={() => handleChipDelete(index)}
            style={{ marginRight: 8, marginBottom: 8 }}
            contentEditable
            onBlur={(e) => {
              handleChipChange(index, e.currentTarget.textContent || "");
              setEditedIndex(-1);
            }}
            onClick={(e) => {
              setEditedIndex(index);

              const selection = window.getSelection();
              if (!selection) return;
              const range = document.createRange();
              const elm = e.currentTarget.querySelector("span");
              if (!elm) return;
              range.selectNodeContents(elm);
              selection.removeAllRanges();
              selection.addRange(range);
            }}
            suppressContentEditableWarning
            onKeyUpCapture={(e) => {
              if (e.key === "Backspace") {
                const selection = window.getSelection();
                if (!selection) return;
                // Удаляем выделенный текст или символ перед курсором
                if (!selection.isCollapsed) {
                  selection.deleteFromDocument();
                } else if (
                  e.key === "Backspace" &&
                  selection.anchorOffset > 0
                ) {
                  selection.modify("extend", "backward", "character");
                  selection.deleteFromDocument();
                  e.preventDefault();
                }

                e.stopPropagation(); // Предотвращаем всплытие события, чтобы не удалить Chip
              }
            }}
          />
        ))}
      </Box>
      <Autocomplete
        multiple
        options={dishOptions}
        value={selectedDishes}
        isOptionEqualToValue={(opt, val) => opt.value.name == val.value.name}
        getOptionLabel={(option) => option.label}
        onChange={(_, newValue) => {
          onDishChange(newValue);
        }}
        renderTags={() => <></>}
        renderInput={(params) => (
          <TextField
            {...params}
            onKeyDown={(e) => {
              if (e.key === "Backspace") {
                e.stopPropagation();
              }
            }}
            label={label}
            fullWidth
          />
        )}
      />
    </>
  );
};

export default ValueableAutocomplete;
