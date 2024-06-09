import { createAsyncThunk, createSlice, PayloadAction } from "@reduxjs/toolkit";
import { userApi } from "./UserApi";

// Define the interface for the user state
interface UserState {
  jwt: string | null;
  userData: UserData | null;
  isGuest: boolean;
  isJwtRefreshing: boolean;
}

// Define the interface for user data
interface UserData {
  // Define the properties for user data
}

// Initial state
const initialState: UserState = {
  jwt: null,
  isGuest: true,
  userData: null,
  isJwtRefreshing: false,
};

// Create a slice
const userSlice = createSlice({
  name: "user",
  initialState,
  reducers: {
    // Action to set user data
    setUserData: (state, action: PayloadAction<UserData | null>) => {
      state.userData = action.payload;
    },
    // Action to set jwt and clear user data
    login: (state, action: PayloadAction<string>) => {
      state.jwt = action.payload;
      state.userData = null; // Clear user data
    },
    // Action to start JWT refresh
    refreshJwtStart: (state) => {
      state.isJwtRefreshing = true;
    },
    // Action to handle successful JWT refresh
    refreshJwtSuccess: (state, action: PayloadAction<string>) => {
      state.jwt = action.payload;
      state.isJwtRefreshing = false;
    },
    // Action to handle failure to refresh JWT
    refreshFailed: (state) => {
      state.jwt = null;
      state.userData = null;
      state.isJwtRefreshing = false;
      state.isGuest = true;
    },
    loginGuest(state, action: PayloadAction<string>) {
      state.isGuest = true;
      state.jwt = action.payload;
    },
    // Action to clear user data
    clearUser: (state) => {
      state.jwt = null;
      state.userData = null;
      state.isJwtRefreshing = false;
      state.isGuest = true;
    },
  },
});

export const initializeAuth = createAsyncThunk(
  "user/initializeAuth",
  async (_, { dispatch }) => {
    const storedJwt = localStorage.getItem("jwt");

    async function getJwt(exJwt: string | null): Promise<string | null> {
      if (!exJwt) return null;

      try {
        const refreshResult = await dispatch(
          userApi.endpoints.refresh.initiate(exJwt)
        );

        if (!("data" in refreshResult)) return null;

        if (!refreshResult.data) return null;

        return refreshResult.data.jwt;
      } catch {
        return null;
      }
    }

    async function getGuestJwt(): Promise<string | null> {
      try {
        const guestResult = await dispatch(
          userApi.endpoints.getGuest.initiate()
        );

        if (!("data" in guestResult)) return null;

        if (!guestResult.data) return null;

        return guestResult.data.jwt;
      } catch {
        return null;
      }
    }

    dispatch(refreshJwtStart());

    const newJwt = await getJwt(storedJwt);

    if (newJwt) {
      dispatch(refreshJwtSuccess(newJwt));
      return;
    }

    dispatch(refreshFailed());

    const newGuestJwt = await getGuestJwt();

    if (newGuestJwt) {
      dispatch(loginGuest(newGuestJwt));
    }
  }
);

// Export actions
export const {
  setUserData,
  login,
  refreshJwtStart,
  refreshJwtSuccess,
  refreshFailed,
  loginGuest,
  clearUser,
} = userSlice.actions;

// Export reducer
export default userSlice.reducer;
