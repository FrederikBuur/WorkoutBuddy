import "./Header.css";
import { A, useNavigate } from "@solidjs/router";
import { Switch, Match, createEffect, createSignal, For, Show } from "solid-js";
import { isUserLoggedIn } from "../../store/WorkoutBuddyState";
import Icon, { Icons } from "./Icon";
import { EXERCISE_DETAILS_PATH, LOGIN_PATH } from "../MainRouter";
import { signOut } from "firebase/auth";
import { auth } from "../../firebase/firebaseConfig";

interface HeaderProps {}

const Header = (props: HeaderProps) => {
  const navigate = useNavigate();

  const [showProfileDropDown, setShowProdileDropDown] = createSignal(false);

  const onProfileClick = () => {
    setShowProdileDropDown(!showProfileDropDown());
    console.log(showProfileDropDown());
  };

  const handleLogout = async () => {
    await signOut(auth);
    navigate(LOGIN_PATH, { replace: true });
  };

  const testOptions: { title: string; onClick: () => void }[] = [
    { title: "", onClick: () => {} },
  ];

  return (
    <div class="header-container">
      <div class="header-container left">
        <img src="src\assets\favicon.ico" />
      </div>
      <div class="header-container center">
        <A class="header-container-item" href={EXERCISE_DETAILS_PATH}>
          Exercise Detail
        </A>
      </div>
      <div class="header-container right">
        <Switch>
          <Match when={isUserLoggedIn()}>
            <button type="button" onClick={handleLogout}>
              logout
            </button>
            <Icon src={Icons.PROFILE} onClick={onProfileClick} />
          </Match>
          <Match when={!isUserLoggedIn()}>
            <A class="app-container item" href="/login">
              Login
            </A>
          </Match>
        </Switch>
      </div>
    </div>
  );
};

export default Header;
