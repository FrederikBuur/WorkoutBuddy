import "./Register.css";
import { A } from "@solidjs/router";
import { Component, createSignal } from "solid-js";

const RegisterPage: Component = () => {
  const [email, setEmail] = createSignal("");
  const [password, setPassword] = createSignal("");

  return (
    <div class="register-page-container">
      <form>
        <h3>Register</h3>
        <label>Email</label>
        <input type="email" onChange={(e) => setEmail(e.target.value)}></input>
        <label>Password</label>
        <input
          type="password"
          onChange={(e) => setPassword(e.target.value)}
        ></input>
        <button type="submit">Register</button>
        <span>
          Already have and account? <A href="/login">Login here</A>
        </span>
      </form>
    </div>
  );
};

export default RegisterPage;
