import store from "../../../Store/sotre";
import { userApi } from "../UserApi";

const refreshToken = async (jwt: string | null): Promise<string | null> => {
  if (!jwt) return null;

  const refreshResult = await store.dispatch(
    userApi.endpoints.refresh.initiate(jwt)
  );

  if (refreshResult.data) {
    return refreshResult.data.jwt;
  }
  return null;
};

export default refreshToken;
