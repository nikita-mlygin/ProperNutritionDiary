import { FetchBaseQueryError } from "@reduxjs/toolkit/query";
import { BaseQueryFn, FetchArgs } from "@reduxjs/toolkit/query";
import { userApi } from "../UserApi";
import store from "../../../Store/sotre";
import {
  refreshFailed,
  refreshJwtStart,
  refreshJwtSuccess,
  loginGuest,
} from "../UserSlice";

export const baseQueryWithReauthBuilder = (
  baseQuery: BaseQueryFn<string | FetchArgs, unknown, FetchBaseQueryError>
) => {
  const fn: BaseQueryFn<
    string | FetchArgs,
    unknown,
    FetchBaseQueryError
  > = async (args, api, extraOptions) => {
    let result = await baseQuery(args, api, extraOptions);

    if (result.error && result.error.status === 401) {
      // try to get a new token
      const currentJwt = store.getState().user.jwt;
      if (!currentJwt) return result;

      store.dispatch(refreshJwtStart());

      const refreshResult = await store.dispatch(
        userApi.endpoints.refresh.initiate(currentJwt)
      );

      if (refreshResult.data) {
        // store the new token in the store or wherever you keep it
        store.dispatch(refreshJwtSuccess(refreshResult.data.jwt));
        // retry the initial query with new token
        result = await baseQuery(args, api, extraOptions);
      } else {
        // refresh failed - try to get a guest token
        store.dispatch(refreshFailed());

        const guestResult = await store.dispatch(
          userApi.endpoints.getGuest.initiate()
        );

        if (guestResult.data) {
          // store the guest token in the store or wherever you keep it
          store.dispatch(loginGuest(guestResult.data.jwt));
          localStorage.setItem("jwt", guestResult.data.jwt); // store guest jwt in local storage

          // retry the initial query with guest token
          result = await baseQuery(args, api, extraOptions);
        }
      }
    }
    return result;
  };

  return fn;
};
