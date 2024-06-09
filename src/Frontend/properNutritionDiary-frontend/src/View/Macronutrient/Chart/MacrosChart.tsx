import React, { useRef, useEffect, forwardRef, memo } from "react";
import { Doughnut } from "react-chartjs-2";
import {
  Chart as ChartJS,
  ArcElement,
  Tooltip,
  Legend,
  ChartOptions,
  ChartData,
  Chart,
} from "chart.js";

ChartJS.register(ArcElement, Tooltip, Legend);

interface ChartComponentProps {
  dataRef: React.MutableRefObject<ChartData<"doughnut">>;
  targetCalories: number;
  currentCalories: number;
}

const MacrosChart = forwardRef<
  Chart<"doughnut", number[], unknown> | undefined,
  ChartComponentProps
>(({ dataRef, targetCalories, currentCalories }, ref) => {
  const options: ChartOptions<"doughnut"> = {
    rotation: -90,
    plugins: {
      legend: { display: false },
      tooltip: {
        callbacks: {
          label: (tooltipItem) => `${tooltipItem.label}: ${tooltipItem.raw}g`,
        },
      },
    },
    maintainAspectRatio: false,
  };

  useEffect(() => {}, [currentCalories, targetCalories, dataRef]);

  return <Doughnut ref={ref} data={dataRef.current} options={options} />;
});

const MacrosChartMemo = memo(MacrosChart);

export default MacrosChartMemo;
