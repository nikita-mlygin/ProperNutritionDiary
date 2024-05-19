import store from "./Store/sotre";
import { Provider } from "react-redux";
import Login from "./View/Authorization/Login/Login";

import "@fontsource/roboto/300.css";
import "@fontsource/roboto/400.css";
import "@fontsource/roboto/500.css";
import "@fontsource/roboto/700.css";
import { CssBaseline, ThemeProvider } from "@mui/material";
import theme from "./View/theme";
import { RouterProvider, createBrowserRouter } from "react-router-dom";
import ProtectedRoute from "./View/Authorization/ProtectedRoute";
import GenerateMenuView from "./View/UserMenu/Generate/GenerateMenuView";
import { HTML5Backend } from "react-dnd-html5-backend";
import { DndProvider } from "react-dnd";

function App() {
  const router = createBrowserRouter([
    {
      path: "/login",
      element: <Login />,
    },
    {
      path: "/",
      element: <ProtectedRoute />,
      children: [
        {
          path: "userMenu/create",
          element: <GenerateMenuView />,
        },
      ],
    },
  ]);

  return (
    <>
      <Provider store={store}>
        <DndProvider backend={HTML5Backend}>
          <ThemeProvider theme={theme}>
            <CssBaseline />
            <RouterProvider router={router} />
          </ThemeProvider>
        </DndProvider>
      </Provider>
    </>
  );
}

export default App;
