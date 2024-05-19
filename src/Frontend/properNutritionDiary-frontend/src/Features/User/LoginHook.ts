import { useState } from "react";
import { useDispatch } from "react-redux";
import { useLazyLoginQuery } from "./UserApi";
import { login as loginAction } from "../User/UserSlice";

const useLoginHook = () => {
  const dispatch = useDispatch();
  const [login, setLogin] = useState("");
  const [password, setPassword] = useState("");
  const [execLogin, { isLoading, isFetching, error }] = useLazyLoginQuery();

  const handleLogin = () => {
    execLogin({ login, password }).then((response) => {
      if (!response.error) {
        dispatch(loginAction(response.data!.jwt));
      }
    });
  };

  return {
    login,
    setLogin,
    password,
    setPassword,
    isLoading,
    isFetching,
    error,
    handleLogin,
  };
};

export default useLoginHook;
