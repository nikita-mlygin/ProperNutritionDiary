import store from "../../../Store/sotre";

export function getAccessToken(): string | null {
  return store.getState().user.jwt;
}
