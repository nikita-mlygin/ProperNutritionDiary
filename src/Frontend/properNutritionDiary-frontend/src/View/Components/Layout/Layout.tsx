// Layout.tsx
import React, { useState } from "react";
import { Box, CssBaseline, Container } from "@mui/material";
import Header from "./Header";
import Footer from "./Footer";
import NavBar from "./NavBar";

const Layout: React.FC = ({ children }) => {
  return (
    <Box sx={{ display: "flex", flexDirection: "column", minHeight: "100vh" }}>
      <CssBaseline />
      <Header />
      <Box sx={{ display: "flex", flexGrow: 1, pl: "60px" }}>
        <NavBar />
        <Container component="main" sx={{ mt: 8, mb: 2, flexGrow: 1 }}>
          {children}
        </Container>
      </Box>
      <Footer />
    </Box>
  );
};

export default Layout;
