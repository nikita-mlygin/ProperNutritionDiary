import { configureStore } from "@reduxjs/toolkit";
import UserSlice from "../Features/User/UserSlice";
import { userApi } from "../Features/User/UserApi";
import { userMenuApi } from "../Features/UserMenu/Api/UserMenuApi";

const store = configureStore({
  reducer: {
    user: UserSlice,
    [userApi.reducerPath]: userApi.reducer,
    [userMenuApi.reducerPath]: userMenuApi.reducer,
  },
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware().concat(userApi.middleware, userMenuApi.middleware),
});

export default store;
export type RootState = ReturnType<typeof store.getState>;
