import { useState } from "react";
import ValueableAutocomplete, { Valueable } from "./ValuableAutocomplete";

interface ValueableAutocompleteWrapperProps<T> {
  dishOptions: Valueable<T>[];
  initValues: Valueable<T>[];
  onChange: (newValues: { valueName: string; value: T | null }[]) => void;
  parseValueFn: <T extends number>(value: string) => T | null;
  label: string;
}

const ValueableAutocompleteWrapper = <T extends number>({
  dishOptions,
  onChange,
  parseValueFn,
  initValues = [],
  label,
}: ValueableAutocompleteWrapperProps<T>) => {
  const convertedDishOptions = dishOptions.map((option) => ({
    label: option.label,
    value: {
      amount: null,
      name: option.value.name,
    },
  }));

  const [convertedSelectedDishes, setConvertedSelectedDishes] =
    useState(initValues);

  const onDishChange = (newValue: Valueable<T>[]) => {
    setConvertedSelectedDishes(newValue);
    onChange(
      newValue.map((x) => ({
        valueName: x.value.name,
        value: x.value.amount,
      }))
    );
  };

  const onDishDelete = (index: number) => {
    const newValues = [...convertedSelectedDishes];
    newValues.splice(index, 1);
    setConvertedSelectedDishes(newValues);
    onChange(
      newValues.map((x) => ({ valueName: x.value.name, value: x.value.amount }))
    );
  };

  return (
    <ValueableAutocomplete
      label={label}
      dishOptions={convertedDishOptions}
      selectedDishes={convertedSelectedDishes}
      onDishChange={onDishChange}
      onDishDelete={onDishDelete}
      parseValueFn={parseValueFn}
    />
  );
};

export default ValueableAutocompleteWrapper;
