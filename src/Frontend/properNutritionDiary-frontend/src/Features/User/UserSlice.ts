import { createSlice, PayloadAction } from "@reduxjs/toolkit";

// Define the interface for the user state
interface UserState {
  jwt: string | null;
  userData: UserData | null;
  isJwtRefreshing: boolean;
}

// Define the interface for user data
interface UserData {
  // Define the properties for user data
}

// Initial state
const initialState: UserState = {
  jwt: null,
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
    },
    // Action to clear user data
    clearUser: (state) => {
      state.jwt = null;
      state.userData = null;
      state.isJwtRefreshing = false;
    },
  },
});

// Export actions
export const {
  setUserData,
  login,
  refreshJwtStart,
  refreshJwtSuccess,
  refreshFailed,
  clearUser,
} = userSlice.actions;

// Export reducer
export default userSlice.reducer;
