import { User } from "firebase/auth";
import { createStore, produce, reconcile } from "solid-js/store";
import { ExerciseDetail } from "../api/exerciseDetailsTypes";
import { fetchExerciseDetials } from "../api/exerciseDetialsApi";

interface FirebaseUserStore {
  firebaseUser?: User;
  exerciseDetails: ExerciseDetail[];
}

const INITIAL_FIREBASE_USER_STORE: FirebaseUserStore = {
  exerciseDetails: [],
};

// stores
const [userStore, setUserStore] = createStore(INITIAL_FIREBASE_USER_STORE);

// store setters
export const setFirebaseUser = (user: User) => {
  setUserStore("firebaseUser", user);
};

export const clearFirebaseUser = () => {
  setUserStore("firebaseUser", undefined);
};

export const setExerciseDetails = (exerciseDetails: ExerciseDetail[]) => {
  setUserStore("exerciseDetails", reconcile(exerciseDetails));
};

// store getters
export const getFirebaseUser = (): User | undefined => userStore.firebaseUser;

export const isUserLoggedIn = (): boolean =>
  userStore.firebaseUser ? true : false;
