import { RootState } from "../../Store/sotre";

export default function AddSaveJwtEvent(getState: () => RootState) {
  window.addEventListener("beforeunload", () => {
    const state = getState();

    if (state.user.jwt) {
      localStorage.setItem("jwt", state.user.jwt);
    }
  });
}
