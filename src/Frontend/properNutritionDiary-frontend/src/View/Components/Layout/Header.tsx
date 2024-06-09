// Header.tsx
import React from "react";
import { AppBar, Toolbar, Typography } from "@mui/material";

const Header: React.FC = () => {
  return (
    <AppBar elevation={0} color="transparent" position="static">
      <Toolbar>
        <Typography variant="h6">Proper Nutrition Diary</Typography>
      </Toolbar>
    </AppBar>
  );
};

export default Header;
