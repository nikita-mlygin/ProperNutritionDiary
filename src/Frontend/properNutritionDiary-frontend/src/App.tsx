import store from "./Store/sotre";
import { Provider } from "react-redux";
import Login from "./View/Authorization/Login/Login";

import "@fontsource/roboto/300.css";
import "@fontsource/roboto/400.css";
import "@fontsource/roboto/500.css";
import "@fontsource/roboto/700.css";
import { CircularProgress, CssBaseline, ThemeProvider } from "@mui/material";
import theme from "./View/theme";
import { RouterProvider, createBrowserRouter } from "react-router-dom";
import ProtectedRoute from "./View/Authorization/ProtectedRoute";
import GenerateMenuView from "./View/UserMenu/Generate/GenerateMenuView";
import { HTML5Backend } from "react-dnd-html5-backend";
import { DndProvider } from "react-dnd";
import AddSaveJwtEvent from "./Features/User/AddSaveJwtEvent";
import { initializeAuth } from "./Features/User/UserSlice";
import { useEffect, useState } from "react";
import UserMenuCreatePage from "./View/UserMenu/Get/UserMenuCreatePage";
import GenerateMenuConfiguration from "./Features/UserMenu/Configure/GenerateMenuConfiguration";
import UserPage from "./View/User/UserPage";
import UserStatsDialog from "./View/UserStats/UserStatsDialog";
import DiaryComponent from "./View/Diary/DiaryComponent";

function App() {
  const [isInitializing, setIsInitializing] = useState(true);

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
          path: "userMenu/generate",
          element: <GenerateMenuView />,
        },
        {
          path: "userMenu/create",
          element: <UserMenuCreatePage />,
        },
        {
          path: "diary/main",
          element: <DiaryComponent />,
        },
        {
          path: "userStats/dialog",
          element: <UserStatsDialog />,
        },
        {
          path: "userMenu/dialog",
          element: <GenerateMenuConfiguration />,
        },
        {
          path: "user/profile",
          element: <UserPage />,
        },
      ],
    },
  ]);

  useEffect(() => {
    async function initialize() {
      await store.dispatch(initializeAuth());
      setIsInitializing(false);
    }

    initialize();
    AddSaveJwtEvent(() => store.getState());
  }, []);

  AddSaveJwtEvent(() => store.getState());

  if (isInitializing) {
    return <CircularProgress />;
  }

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
