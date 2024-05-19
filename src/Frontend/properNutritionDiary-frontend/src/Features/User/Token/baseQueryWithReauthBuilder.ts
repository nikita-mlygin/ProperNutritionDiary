import { FetchBaseQueryError } from "@reduxjs/toolkit/query";
import { BaseQueryFn, FetchArgs } from "@reduxjs/toolkit/query";
import { userApi } from "../UserApi";
import store from "../../../Store/sotre";
import {
  refreshFailed,
  refreshJwtStart,
  refreshJwtSuccess,
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
      if (!store.getState().user.jwt) return result;

      const refreshResult = await store.dispatch(
        userApi.endpoints.refresh.initiate(store.getState().user.jwt!)
      );

      store.dispatch(refreshJwtStart());

      if (refreshResult.data) {
        // store the new token in the store or wherever you keep it
        store.dispatch(refreshJwtSuccess(refreshResult.data.jwt));
        // retry the initial query
        result = await baseQuery(args, api, extraOptions);
      } else {
        // refresh failed - do something like redirect to login or show a "retry" button
        api.dispatch(refreshFailed());
      }
    }
    return result;
  };

  return fn;
};
