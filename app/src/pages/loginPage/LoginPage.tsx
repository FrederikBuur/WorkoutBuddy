import "./LoginPage.css";
import { A, Navigate, useNavigate } from "@solidjs/router";
import { signInWithEmailAndPassword } from "firebase/auth";
import { Component, Show, createSignal, onMount } from "solid-js";
import { auth } from "../../firebase/firebaseConfig";
import { FirebaseError } from "firebase/app";
import { HOME_PATH } from "../../shared/MainRouter";

const LoginPage: Component = () => {
  const [email, setEmail] = createSignal("test@test.com");
  const [password, setPassword] = createSignal("password");
  const [loginError, setLoginError] = createSignal<string | null>();
  const navigate = useNavigate();

  const loginUser = async (e: Event) => {
    e.preventDefault();
    try {
      const test = await signInWithEmailAndPassword(auth, email(), password());
      test.operationType;
      navigate(HOME_PATH, { replace: true });
    } catch (err) {
      if (err instanceof FirebaseError) {
        setLoginError(err.code);
      } else {
        console.error("unknown login error: " + err);
      }
    }
  };

  return (
    <div class="login-page-container">
      <form onSubmit={(e) => loginUser(e)}>
        <h3>Login</h3>
        <label>Email</label>
        <input
          type="email"
          onChange={(e) => setEmail(e.target.value)}
          value={email()}
        ></input>
        <label>Password</label>
        <input
          type="password"
          onChange={(e) => setPassword(e.target.value)}
          value={password()}
        ></input>
        <Show when={loginError()}>
          <span class="error-message">{loginError()}</span>
        </Show>
        <button type="submit">Login</button>
        <span>
          <A href="/register">Register</A>
        </span>
      </form>
    </div>
  );
};

export default LoginPage;
