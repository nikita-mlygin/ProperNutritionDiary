import React, { FC } from "react";
import { Button } from "@mui/material";
import { Line } from "react-chartjs-2";

interface UserChartsProps {
  selectedChart: string;
  weightChartData: any;
  nutrientChartData: any;
  deviationChartData: any;
  onChartChange: (chart: string) => void;
}

const UserCharts: FC<UserChartsProps> = ({
  selectedChart,
  weightChartData,
  nutrientChartData,
  deviationChartData,
  onChartChange,
}) => {
  return (
    <>
      <Button
        variant="contained"
        color="primary"
        onClick={() => onChartChange("nutrient")}
        disabled={selectedChart === "nutrient"}
      >
        Nutrient Intake
      </Button>
      <Button
        variant="contained"
        color="primary"
        onClick={() => onChartChange("deviation")}
        disabled={selectedChart === "deviation"}
      >
        Deviation from Norm
      </Button>
      <Button
        variant="contained"
        color="primary"
        onClick={() => onChartChange("weight")}
        disabled={selectedChart === "weight"}
      >
        Weight Changes
      </Button>
      {selectedChart === "nutrient" && <Line data={nutrientChartData} />}
      {selectedChart === "deviation" && <Line data={deviationChartData} />}
      {selectedChart === "weight" && <Line data={weightChartData} />}
    </>
  );
};

export default UserCharts;
