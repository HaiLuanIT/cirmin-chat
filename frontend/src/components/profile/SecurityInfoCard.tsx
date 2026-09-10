import { useForm } from "react-hook-form";
import { Button } from "../ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "../ui/card";
import { Field, FieldLabel } from "../ui/field";
import { Input } from "../ui/input";
import { Loader2, Save } from "lucide-react";
import { useUserStore } from "@/stores/useUserStore";
import { useTranslation } from "react-i18next";
import { getApiProblemDetails, translateApiErrorCode } from "@/lib/api-error";

interface IFormValue {
  password: string;
  newPassword: string;
  confirmNewPassword: string;
}
const SecurityInfoCard = () => {
  const { changePassword } = useUserStore();
  const { t } = useTranslation(["common", "profile"]);
  const {
    register,
    handleSubmit,
    reset,
    setError,
    clearErrors,
    formState: { errors, isSubmitting },
  } = useForm<IFormValue>({
    defaultValues: {
      password: "",
      newPassword: "",
      confirmNewPassword: "",
    },
  });

  const backendFieldMap: Record<string, keyof IFormValue> = {
    oldPassword: "password",
    newPassword: "newPassword",
  };

  const businessFieldMap: Partial<Record<string, keyof IFormValue>> = {
    "AUTH.OLD_PASSWORD_INCORRECT": "password",
    "AUTH.NEW_PASSWORD_SAME_AS_OLD": "newPassword",
  };

  const handleChangePassword = handleSubmit(async (data) => {
    if (data.newPassword != data.confirmNewPassword) {
      setError("confirmNewPassword", {
        type: "validate",
        message: t("AUTH.PASSWORD_MISMATCH"),
      });
      return;
    }
    clearErrors();
    try {
      await changePassword(data.password, data.newPassword);
    } catch (error) {
      const problem = getApiProblemDetails(error);

      if (!problem) {
        setError("root.server", {
          message: translateApiErrorCode("SYSTEM.INTERNAL_ERROR"),
        });
        return;
      }

      if (problem.errors) {
        for (const [backendField, fieldErrors] of Object.entries(
          problem.errors,
        )) {
          const firstError = fieldErrors[0];

          if (!firstError) {
            continue;
          }
          const frontendField = backendFieldMap[backendField];

          if (!frontendField) {
            continue;
          }
          setError(frontendField, {
            type: "server",
            message: translateApiErrorCode(firstError.code, firstError.params),
          });
        }
        return;
      }
      const businessField = businessFieldMap[problem.code];
      if (businessField) {
        setError(businessField, {
          type: "server",
          message: translateApiErrorCode(problem.code, problem.params),
        });
        return;
      }

      setError("root.server", {
        type: "server",
        message: translateApiErrorCode(problem.code, problem.params),
      });
    } finally {
      reset();
    }
  });
  return (
    <>
      <form onSubmit={handleChangePassword}>
        <Card className="bg-card/80 mb-5">
          <CardHeader>
            <CardTitle>{t("profile:security.title")}</CardTitle>
            <CardDescription>
              {t("profile:security.description")}
            </CardDescription>
          </CardHeader>
          <CardContent>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-5">
              <Field className="md:col-span-2">
                <FieldLabel htmlFor="password">
                  {t("profile:security.fields.currentPassword")}
                </FieldLabel>
                <Input
                  id="password"
                  type="password"
                  autoComplete="off"
                  {...register("password")}
                />
              </Field>
              <Field>
                <FieldLabel htmlFor="newPassword">
                  {t("profile:security.fields.newPassword")}
                </FieldLabel>
                <Input
                  id="newPassword"
                  type="password"
                  autoComplete="off"
                  {...register("newPassword")}
                />
              </Field>
              <Field>
                <FieldLabel htmlFor="confirmNewPassword">
                  {t("profile:security.fields.confirmPassword")}
                </FieldLabel>
                <Input
                  id="confirmNewPassword"
                  type="password"
                  autoComplete="off"
                  {...register("confirmNewPassword")}
                />
              </Field>
            </div>
          </CardContent>
          <CardFooter className="justify-end border-0 bg-card/80">
            {errors.root?.server && (
              <p className="error-message text-center">
                {errors.root.server.message}
              </p>
            )}
            <Button
              type="submit"
              disabled={isSubmitting}
              className="w-full sm:w-auto cursor-pointer"
            >
              {isSubmitting ? (
                <>
                  <Loader2 className="animate-spin" />
                </>
              ) : (
                <>
                  <Save /> {t("common:actions.save")}
                </>
              )}
            </Button>
          </CardFooter>
        </Card>
      </form>
      {/* <Card></Card> */}
    </>
  );
};

export default SecurityInfoCard;
