import { FC } from "react";
import { RootState } from "../../Store/sotre";
import { useSelector } from "react-redux";
import selectIsAuth from "./isAuth";
import { Navigate, Outlet } from "react-router-dom";
import Layout from "../Components/Layout/Layout";

const loginPath = "/login";

const ProtectedRoute: FC = () => {
  const isAuth = useSelector<RootState>(selectIsAuth);

  return !isAuth ? (
    <Navigate to={loginPath} />
  ) : (
    <Layout>
      <Outlet />
    </Layout>
  );
};

export default ProtectedRoute;
