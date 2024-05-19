import { RootState } from "../../Store/sotre";

const selectIsAuth = (state: RootState): boolean =>
  state.user.jwt != null && !state.user.isJwtRefreshing;

export default selectIsAuth;
