import React, { useState } from "react";
import { Paper, Typography, List } from "@mui/material";
import {
  DetailsDay,
  MenuItemDetails,
  UserMenuDetails,
} from "../../../Features/UserMenu/Get/UserMenuDetails";
import DailyMenu from "./DailyMenu";

interface UserMenuDetailsProps {
  userMenuDetails: UserMenuDetails;
}

const UserMenuDetailsComponent: React.FC<UserMenuDetailsProps> = ({
  userMenuDetails,
}) => {
  const [dailyMenus, setDailyMenus] = useState(userMenuDetails.dailyMenus);

  const handleDrop = (
    item: MenuItemDetails,
    itemSource: [number, keyof DetailsDay],
    target: [number, keyof DetailsDay]
  ) => {
    const [sourceIndex, sourceSection] = itemSource;
    const [targetIndex, targetSection] = target;

    setDailyMenus((prev) =>
      prev.map((x, i) => {
        let fn1 = (x: DetailsDay) => x;
        let fn2 = (x: DetailsDay) => x;

        if (i == sourceIndex) {
          fn1 = (x: DetailsDay) => ({
            ...x,
            [sourceSection]: [
              ...(x[sourceSection] as MenuItemDetails[]).filter(
                (x) => x.id !== item.id
              ),
            ],
          });
        }

        if (i == targetIndex) {
          fn2 = (x: DetailsDay) => ({
            ...x,
            [targetSection]: [
              ...(x[targetSection] as MenuItemDetails[]),
              { ...item },
            ],
          });
        }

        return fn2(fn1(x));
      })
    );
  };

  return (
    <div>
      <Typography variant="h4">User Menu Details</Typography>
      <Paper elevation={3} style={{ padding: 20, marginTop: 20 }}>
        <Typography variant="h6">ID: {userMenuDetails.id}</Typography>
        <Typography variant="subtitle1">
          Created At: {userMenuDetails.createdAt.toString()}
        </Typography>
        <List>
          {dailyMenus.map((dailyMenu, index) => (
            <DailyMenu
              key={index}
              dailyMenu={dailyMenu}
              dayNumber={dailyMenu.dayNumber}
              onDrop={handleDrop}
              index={index}
              setItems={(nv) => {
                setDailyMenus((prev) => {
                  const res = [...prev];
                  res[index] = nv(prev[index]);
                  return res;
                });
              }}
            />
          ))}
        </List>
      </Paper>
    </div>
  );
};

export default UserMenuDetailsComponent;
