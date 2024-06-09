import React, { useEffect, useRef } from "react";
import { Typography, Box, useTheme } from "@mui/material";
import { format } from "date-fns";
import MacrosChart from "./MacrosChart";
import { Chart, ChartData } from "chart.js";
import { Square } from "@mui/icons-material";

interface MacrosChartProps {
  targetMacros: number[];
  currentMacros: number[];
  targetCalories: number;
  currentCalories: number;
  date: Date;
}

const mapMacros = (macros: number): string => macros.toFixed(2);

const MacrosChartHeader: React.FC<MacrosChartProps> = ({
  targetMacros,
  currentMacros,
  targetCalories,
  currentCalories,
  date,
}) => {
  const formattedDate = format(date, "PPPP");
  const theme = useTheme();

  const positiveColor = theme.palette.success.main; // Use your theme's success color
  const negativeColor = theme.palette.error.main; // Use your theme's error color
  const chartRef = useRef<Chart<"doughnut", number[], unknown> | null>(null);

  const dataRef = useRef<ChartData<"doughnut">>({
    labels: ["Proteins", "Fats", "Carbohydrates"],
    datasets: [
      {
        data: targetMacros,
        backgroundColor: [
          theme.palette.macros_proteins.main,
          theme.palette.macros_fats.main,
          theme.palette.macros_carbohydrates.main,
        ],
        borderWidth: 1,
        borderColor: "#fff",
        circumference: 180,
        hoverBorderColor: "#fff",
      },
      {
        data: currentMacros,
        backgroundColor: [
          theme.palette.macros_proteins.main,
          theme.palette.macros_fats.main,
          theme.palette.macros_carbohydrates.main,
        ],
        borderWidth: 1,
        borderColor: "#fff",
        hoverBorderColor: "#fff",
        circumference: Math.min((currentCalories / targetCalories) * 180, 180),
        rotation: -90,
      },
    ],
  });

  useEffect(() => {
    if (!chartRef.current) return;

    dataRef.current.datasets[1].data = currentMacros;
    dataRef.current.datasets[0].data = targetMacros;

    dataRef.current.datasets[1].circumference = Math.min(
      (currentCalories / targetCalories) * 180,
      180
    );

    dataRef.current.datasets[0].circumference = Math.min(
      (targetCalories / currentCalories) * 180,
      180
    );

    chartRef.current.data = dataRef.current;
    chartRef.current.update();
  }, [currentCalories, currentMacros, targetCalories, targetMacros]);

  const calorieDifference = currentCalories - targetCalories;
  const calorieColor =
    Math.abs(calorieDifference) < 50
      ? positiveColor
      : calorieDifference > 0
      ? negativeColor
      : "black";

  return (
    <>
      <Typography variant="h5" align="center" gutterBottom>
        {formattedDate}
      </Typography>
      <Box position="relative" width="100%" height={300}>
        <MacrosChart
          dataRef={dataRef}
          targetCalories={targetCalories}
          currentCalories={currentCalories}
          ref={chartRef}
        />
        <Box
          position="absolute"
          bottom={10}
          width="auto"
          sx={{
            transform: "translate(-50%, 0)",
            left: "50%",
          }}
          textAlign="center"
        >
          <Typography variant="h6" style={{ color: calorieColor }}>
            {mapMacros(currentCalories)} kCal
          </Typography>
          <Typography variant="h6">{mapMacros(targetCalories)} kCal</Typography>
        </Box>
      </Box>
      <Box
        display="flex"
        justifyContent="center"
        gap={2}
        alignItems="center"
        mt={4}
      >
        {[
          { name: "Proteins", color: theme.palette.macros_proteins.main },
          { name: "Fats", color: theme.palette.macros_fats.main },
          {
            name: "Carbohydrates",
            color: theme.palette.macros_carbohydrates.main,
          },
        ].map((label, index) => (
          <Box
            key={index}
            display="grid"
            gridTemplateColumns="auto 1fr"
            gridTemplateRows="auto auto"
            alignItems="center"
            mx={2}
          >
            <Square
              style={{
                color: label.color,
                marginRight: theme.spacing(1),
                lineHeight: 0.8,
              }}
            />
            <Box display="flex" alignItems="center" justifyContent="center">
              <Typography variant="body1" fontWeight="bold" lineHeight={0.8}>
                {label.name}
              </Typography>
            </Box>
            <Typography
              gridColumn={2}
              gridRow={2}
              variant="caption"
              textAlign="center"
            >
              {mapMacros(currentMacros[index])}/{mapMacros(targetMacros[index])}
            </Typography>
          </Box>
        ))}
      </Box>
    </>
  );
};

export default MacrosChartHeader;
