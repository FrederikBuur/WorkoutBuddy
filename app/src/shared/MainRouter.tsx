import { Route, Router } from "@solidjs/router";
import { Component } from "solid-js";
import LoginPage from "../pages/loginPage/LoginPage";
import App from "../App";
import ExercisesPage from "../pages/exercisesPage/ExercisesPage";
import RegisterPage from "../pages/registerPage/RegisterPage";

export const ROOT_PATH = "/";
export const LOGIN_PATH = "/login";
export const REGISTER_PATH = "/register";
export const HOME_PATH = "/home";
export const EXERCISE_DETAILS_PATH = "/exercise-details";

export const MainRouter: Component = () => {
  return (
    <Router root={App}>
      <Route path={LOGIN_PATH} component={LoginPage} />
      <Route path={REGISTER_PATH} component={RegisterPage} />
      <Route path={EXERCISE_DETAILS_PATH} component={ExercisesPage} />
      {/* <Route path={HOME_PATH} component={HomePage} /> */}
    </Router>
  );
};
