import { FirebaseOptions, initializeApp } from "firebase/app";
import { getAuth } from "firebase/auth";
import { getAnalytics } from "firebase/analytics";

const firebaseOptions: FirebaseOptions = {
  apiKey: import.meta.env.VITE_FIREBASE_API_KEY,
  authDomain: `${import.meta.env.VITE_FIREBASE_PROJECT_ID}.firebaseapp.com`,
  projectId: import.meta.env.VITE_FIREBASE_PROJECT_ID,
  storageBucket: `${import.meta.env.VITE_FIREBASE_PROJECT_ID}.appspot.com`,
  messagingSenderId: "217070469792",
  appId: import.meta.env.VITE_FIREBASE_APP_ID,
  measurementId: "G-GKWWJVLMK8",
};

const app = initializeApp(firebaseOptions);

export const auth = getAuth(app);
export const analytics = getAnalytics(app);
