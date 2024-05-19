import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import { GenerateMenuConfiguration } from "../Generate/GenerateMenuConfiguration";
import { UserMenuDetails } from "../Get/UserMenuDetails";
import { prepareHeaders } from "../../User/Token/prepareHeaders";
import { baseQueryWithReauthBuilder } from "../../User/Token/baseQueryWithReauthBuilder";

const baseUserMenuApiUrl = "https://localhost:8081/user-menu-api/";

export const userMenuApi = createApi({
  reducerPath: "userMenuApi",
  baseQuery: baseQueryWithReauthBuilder(
    fetchBaseQuery({
      baseUrl: baseUserMenuApiUrl,
      prepareHeaders: prepareHeaders,
    })
  ),
  endpoints: (builder) => ({
    generate: builder.query<UserMenuDetails, GenerateMenuConfiguration>({
      query: (cfg) => ({ url: "gen", method: "POST", body: cfg }),
    }),
  }),
});

export const { useGenerateQuery, useLazyGenerateQuery } = userMenuApi;

export default userMenuApi.reducer;
