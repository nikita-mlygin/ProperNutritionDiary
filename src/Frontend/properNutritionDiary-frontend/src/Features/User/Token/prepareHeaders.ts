import { getAccessToken } from "./getAccessToken";

export function prepareHeaders(headers: Headers) {
  const token = getAccessToken();

  if (token) {
    headers.set("authorization", `Bearer ${token}`);
  }

  return headers;
}
