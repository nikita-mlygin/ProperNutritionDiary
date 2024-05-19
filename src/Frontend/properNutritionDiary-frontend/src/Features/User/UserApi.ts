import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import { TokenResponse } from "./Token/TokenResponse";
import { LoginRequest } from "./Login/LoginRequest";
import { RegRequest } from "./Reg/RegRequest";

const baseUrl = "https://localhost:8081/user-api/";

export const userApi = createApi({
  reducerPath: "userApi",
  baseQuery: fetchBaseQuery({ baseUrl }),
  endpoints: (builder) => ({
    getGuest: builder.query<TokenResponse, void>({
      query: () => "guest",
    }),
    login: builder.query<TokenResponse, LoginRequest>({
      query: (loginRequest) => ({
        url: "login",
        method: "POST",
        body: loginRequest,
      }),
    }),
    reg: builder.mutation<void, RegRequest>({
      query: (regRequest) => ({
        url: "reg",
        method: "POST",
        body: regRequest,
      }),
    }),
    refresh: builder.query<TokenResponse, string>({
      query: (eJwt) => ({
        url: "r",
        method: "POST",
        body: eJwt,
      }),
    }),
  }),
});

export const {
  useGetGuestQuery,
  useLoginQuery,
  useLazyLoginQuery,
  useRegMutation,
  useRefreshQuery,
} = userApi;
