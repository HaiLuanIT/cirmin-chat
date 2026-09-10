import i18n from "@/i18n";
import axios from "axios";

export type ApiErrorParams = Record<string, unknown>;

export interface ApiFieldError {
  code: string;
  params: ApiErrorParams;
}
export interface ApiProblemDetails {
  title: string;
  status: number;
  code: string;
  params?: ApiErrorParams;
  errors?: Record<string, ApiFieldError[]>;
  traceId: string;
}

export function getApiErrorMessage(error: unknown): string {
  const problem = getApiProblemDetails(error);

  if (!problem) return i18n.t("fallback", { ns: "errors" });

  if (problem.errors) {
    for (const fieldErrors of Object.values(problem.errors)) {
      const firstError = fieldErrors[0];

      if (firstError) {
        return translateApiErrorCode(firstError.code, firstError.params);
      }
    }
  }
  return translateApiErrorCode(problem.code, problem.params);
}

export function getApiProblemDetails(error: unknown): ApiProblemDetails | null {
  if (!axios.isAxiosError(error)) {
    return null;
  }

  const data: unknown = error?.response?.data;

  if (
    typeof data !== "object" ||
    data === null ||
    !("code" in data) ||
    typeof data.code !== "string"
  ) {
    return null;
  }
  return data as ApiProblemDetails;
}

function normalizeParams(params: ApiErrorParams = {}): ApiErrorParams {
  return Object.fromEntries(
    Object.entries(params).map(([key, value]) => [
      key,
      Array.isArray(value) ? value.join(", ") : value,
    ]),
  );
}

export function translateApiErrorCode(
  code?: string | null,
  params: ApiErrorParams = {},
): string {
  const fallback = i18n.t("fallback", { ns: "errors" });

  if (!code) {
    return fallback;
  }
  return i18n.t(code, {
    ns: "errors",
    ...normalizeParams(params),
    defaultValue: fallback,
  });
}
