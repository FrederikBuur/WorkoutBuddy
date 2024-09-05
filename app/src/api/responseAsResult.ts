export const responseAsResult = async <T>(
  url: string,
  init?: RequestInit
): Promise<Result<T>> => {
  var response: Response;

  try {
    response = await fetch(url, init);
  } catch (error) {
    return {
      error: {
        statusCode: 500,
        errorMessage: "Something went wrong",
      },
    };
  }

  if (response.ok) {
    const responseJson = await response.json();
    const result: Result<T> = {
      data: responseJson,
    };
    return result;
  } else {
    const error: Result<T> = {
      error: {
        statusCode: response.status,
        errorMessage: response.statusText,
      },
    };
    return error;
  }
};
export interface Result<T> {
  data?: T;
  error?: Error;
}
interface Error {
  statusCode: number;
  errorMessage: string;
}
