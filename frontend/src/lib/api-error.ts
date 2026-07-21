import axios from "axios";

export interface ApiProblemDetails {
  title?: string;
  status?: number;
  detail?: string;
  instance?: string;
  errors?: Record<string, string[]>;
}

export const getApiErrorMessage = (
  error: unknown,
  fallbackMessage = "Đã xảy ra lỗi. Vui lòng thử lại.",
): string => {
  if (!axios.isAxiosError<ApiProblemDetails>(error)) {
    return fallbackMessage;
  }

  const problem = error?.response.data;
  const validationMessage = problem?.errors
    ? Object.values(problem.errors).flat().find(Boolean)
    : undefined;

  return validationMessage ?? problem?.detail ?? fallbackMessage;
};
