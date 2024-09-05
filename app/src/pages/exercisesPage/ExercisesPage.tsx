import "./ExercisesPage.css";
import { signOut } from "firebase/auth";
import {
  Component,
  For,
  Match,
  Show,
  Switch,
  createEffect,
  createResource,
  createSignal,
} from "solid-js";
import { auth } from "../../firebase/firebaseConfig";
import { useNavigate } from "@solidjs/router";
import { fetchExerciseDetials } from "../../api/exerciseDetialsApi";
import { getFirebaseUser } from "../../store/WorkoutBuddyState";
import { LOGIN_PATH } from "../../shared/MainRouter";
import { ViewState } from "../../shared/ViewState";
import ExerciseCard from "./ExerciseCard";

const ExercisesPage: Component = () => {
  const [viewState, setViewState] = createSignal(ViewState.LOADING);

  const [exerciseDetails, { mutate, refetch }] =
    createResource(fetchExerciseDetials);

  createEffect(() => {
    if (getFirebaseUser() && viewState() === ViewState.ERROR) {
      refetch();
    }

    if (exerciseDetails()?.data) {
      setViewState(ViewState.SUCCESS);
    } else if (exerciseDetails()?.error) {
      setViewState(ViewState.ERROR);
    }
  });

  const handlyRetryFetch = async () => {
    setViewState(ViewState.LOADING);
    refetch();
  };

  return (
    <div class="exercise-page-container">
      <Switch>
        <Match when={viewState() === ViewState.LOADING}>
          <p>Loading...</p>
        </Match>
        <Match when={viewState() === ViewState.ERROR}>
          <span>
            Error: {exerciseDetails()?.error?.errorMessage ?? "Unknown error"}
          </span>
          <button type="button" onClick={handlyRetryFetch}>
            Retry fetch
          </button>
        </Match>
        <Match when={viewState() === ViewState.SUCCESS}>
          <div class="exercises-grid">
            <For each={exerciseDetails()?.data?.items}>
              {(item, index) => <ExerciseCard exercise={item} />}
            </For>
          </div>
        </Match>
      </Switch>
    </div>
  );
};

export default ExercisesPage;
