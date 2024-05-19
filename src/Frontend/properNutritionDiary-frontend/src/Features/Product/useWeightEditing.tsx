import { useState } from "react";

const useWeightEditing = (
  initialWeight: number,
  onWeightChange: (nV: number) => void
) => {
  const [weight, setWeight] = useState(initialWeight.toFixed(2));
  const [tempWeight, setTempWeight] = useState(initialWeight.toFixed(2));
  const [isEditing, setIsEditing] = useState(false);

  const startEditing = () => {
    setIsEditing(true);
    setTempWeight(weight);
  };

  const cancelEditing = () => {
    setIsEditing(false);
    setTempWeight(weight);
  };

  const handleWeightChange = (value: string) => {
    if (/^\d*\.?\d{0,2}$/.test(value)) {
      setTempWeight(value);
    }
  };

  const handleWeightBlur = () => {
    if (!tempWeight) {
      setTempWeight("0.00");
    }

    const res = parseFloat(tempWeight);

    setWeight(res.toFixed(2));
    onWeightChange(res);
    setIsEditing(false);
  };

  return {
    weight,
    tempWeight,
    isEditing,
    startEditing,
    cancelEditing,
    handleWeightChange,
    handleWeightBlur,
  };
};

export default useWeightEditing;
