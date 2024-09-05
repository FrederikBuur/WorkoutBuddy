/* @refresh reload */
import "./index.css";
import "solid-devtools";
import { render } from "solid-js/web";
import { auth } from "./firebase/firebaseConfig";
import { clearFirebaseUser, setFirebaseUser } from "./store/WorkoutBuddyState";
import { MainRouter } from "./shared/MainRouter";
import App from "./App";

const root = document.getElementById("root");

if (import.meta.env.DEV && !(root instanceof HTMLElement)) {
  throw new Error(
    "Root element not found. Did you forget to add it to your index.html? Or maybe the id attribute got misspelled?"
  );
}

auth.onAuthStateChanged((user) => {
  if (user) {
    setFirebaseUser(user);
  } else {
    clearFirebaseUser();
  }
});

render(() => <MainRouter />, root!);
