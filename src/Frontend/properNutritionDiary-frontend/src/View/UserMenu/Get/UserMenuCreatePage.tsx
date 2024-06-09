import { FC } from "react";
import { useLocation } from "react-router-dom";
import { UserMenuDetails } from "../../../Features/UserMenu/Get/UserMenuDetails";
import UserMenuDetailsComponent from "./UserMenuDetailsComponent";

interface UserMenuCreatePageProps {}

const UserMenuCreatePage: FC<UserMenuCreatePageProps> = () => {
  const location = useLocation();
  const userMenuDetails = (location.state
    ?.userMenuDetails as UserMenuDetails) ?? {
    id: "",
    createdAt: Date(),
    dailyMenus: [],
  };

  return <UserMenuDetailsComponent userMenuDetails={userMenuDetails} />;
};

export default UserMenuCreatePage;
