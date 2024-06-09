import React, { FC, ChangeEvent } from "react";
import { TextField } from "@mui/material";

interface StaticInfoFormProps {
  height: number;
  lifestyle: string;
  gender: string;
  onChange: (field: string, value: any) => void;
}

const StaticInfoForm: FC<StaticInfoFormProps> = ({
  height,
  lifestyle,
  gender,
  onChange,
}) => {
  const handleInputChange =
    (field: string) => (event: ChangeEvent<HTMLInputElement>) => {
      onChange(field, event.target.value);
    };

  return (
    <>
      <TextField
        label="Height"
        type="number"
        fullWidth
        margin="normal"
        value={height}
        onChange={handleInputChange("height")}
      />
      <TextField
        label="Lifestyle"
        type="text"
        fullWidth
        margin="normal"
        value={lifestyle}
        onChange={handleInputChange("lifestyle")}
      />
      <TextField
        label="Gender"
        type="text"
        fullWidth
        margin="normal"
        value={gender}
        onChange={handleInputChange("gender")}
      />
    </>
  );
};

export default StaticInfoForm;
