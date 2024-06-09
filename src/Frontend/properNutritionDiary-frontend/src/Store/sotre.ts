import { configureStore } from "@reduxjs/toolkit";
import { userApi } from "../Features/User/UserApi";
import { userMenuApi } from "../Features/UserMenu/Api/UserMenuApi";
import UserSlice from "../Features/User/UserSlice";
import { productApi } from "../Features/Product/ProductApi";

const store = configureStore({
  reducer: {
    user: UserSlice,
    [userApi.reducerPath]: userApi.reducer,
    [productApi.reducerPath]: productApi.reducer,
    [userMenuApi.reducerPath]: userMenuApi.reducer,
  },
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware().concat(
      userApi.middleware,
      userMenuApi.middleware,
      productApi.middleware
    ),
});

export default store;
export type RootState = ReturnType<typeof store.getState>;
