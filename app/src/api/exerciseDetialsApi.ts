import { auth } from "../firebase/firebaseConfig";
import { Result, responseAsResult } from "./responseAsResult";
import { ExerciseDetail, PaginatedResponse } from "./exerciseDetailsTypes";

const BASE_URL = "https://localhost:5272/api";

export const fetchExerciseDetials = async (): Promise<
  Result<PaginatedResponse<ExerciseDetail>>
> => {
  const url = `${BASE_URL}/exercise-details`;

  const jwtToken = await auth.currentUser?.getIdToken();

  const requestHeaders = new Headers({
    Authorization: `Bearer ${jwtToken}`,
    "Content-Type": "application/json",
  });

  const result = await responseAsResult<PaginatedResponse<ExerciseDetail>>(
    url,
    {
      method: "get",
      headers: requestHeaders,
    }
  );

  return result;
};
