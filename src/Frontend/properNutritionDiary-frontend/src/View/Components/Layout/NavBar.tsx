import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  Drawer,
  List,
  ListItemText,
  ListItemIcon,
  IconButton,
  Box,
  CssBaseline,
  ListItemButton,
} from "@mui/material";
import HomeIcon from "@mui/icons-material/Home";
import MenuBookIcon from "@mui/icons-material/MenuBook";
import AccountCircleIcon from "@mui/icons-material/AccountCircle";
import BookIcon from "@mui/icons-material/Book";
import ArrowForwardIosIcon from "@mui/icons-material/ArrowForwardIos";
import ArrowBackIosIcon from "@mui/icons-material/ArrowBackIos";

const drawerWidthExpanded = 240;
const drawerWidthCollapsed = 60;

const NavBar: React.FC = () => {
  const [open, setOpen] = useState(false);
  const navigate = useNavigate();

  const handleDrawerToggle = () => {
    setOpen(!open);
  };

  const handleNavigation = (path: string) => {
    navigate(path);
  };

  return (
    <Box sx={{ display: "flex" }}>
      <CssBaseline />

      <Drawer
        variant="permanent"
        elevation={0}
        sx={{
          width: open ? drawerWidthExpanded : drawerWidthCollapsed,
          flexShrink: 0,
          whiteSpace: "nowrap",
          boxSizing: "border-box",
          border: 0,
          outline: 0,
          left: 0,
          position: "fixed",
          background: "transparent",
          height: "100vh",
          zIndex: 1,
          "& .MuiDrawer-paper": {
            width: open ? drawerWidthExpanded : drawerWidthCollapsed,
            transition: "width 0.3s",
            overflow: "visible",
            background: "transparent",
            justifyContent: "center",
            color: "#fff",
            border: 0,
            position: "relative",
            "& .MuiListItemIcon-root": {
              minWidth: open ? "auto" : "initial",
              justifyContent: "center",
            },
          },
        }}
      >
        <List sx={{ backgroundColor: "#333" }}>
          <ListItemButton
            onClick={handleDrawerToggle}
            sx={{
              transition: 2,
              justifyContent: open ? "flex-end" : "center",
              "& .MuiSvgIcon-root": {
                transform: open ? "rotate(180deg)" : "none",
              },
            }}
          ></ListItemButton>
          {[
            { text: "Home", icon: <HomeIcon fontSize="large" />, path: "/" },
            {
              text: "Diary",
              icon: <BookIcon fontSize="large" />,
              path: "/diary/main",
            },
            {
              text: "Menu",
              icon: <MenuBookIcon fontSize="large" />,
              path: "/userMenu/generate",
            },
            {
              text: "Account",
              icon: <AccountCircleIcon fontSize="large" />,
              path: "/user/profile",
            },
          ].map((item) => (
            <ListItemButton
              key={item.text}
              sx={{ justifyContent: "center" }}
              onClick={() => handleNavigation(item.path)}
            >
              <ListItemIcon sx={{ my: 2 }}>{item.icon}</ListItemIcon>
              {open && <ListItemText primary={item.text} />}
            </ListItemButton>
          ))}
        </List>
        <Box
          sx={{
            position: "absolute",
            top: "50%",
            right: "-15px",
            transform: "translateY(-50%)",
            backgroundColor: "#333",
            borderRadius: "50%",
          }}
        >
          <IconButton onClick={handleDrawerToggle} sx={{ p: 1 }}>
            {open ? <ArrowBackIosIcon /> : <ArrowForwardIosIcon />}
          </IconButton>
        </Box>
      </Drawer>
    </Box>
  );
};

export default NavBar;
