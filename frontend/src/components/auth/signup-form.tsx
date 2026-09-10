import { Label } from "../ui/label";
import { Input } from "../ui/input";
import { Card, CardContent } from "../ui/card";
import { cn } from "../../lib/utils";
import { Button } from "../ui/button";
import { z } from "zod";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useAuthStore } from "../../stores/useAuthStore";
import { useNavigate } from "react-router";
import { useTranslation } from "react-i18next";
import { getApiProblemDetails, translateApiErrorCode } from "@/lib/api-error";
import type { TFunction } from "i18next";
import { useMemo } from "react";

function createSignUpSchema(t: TFunction) {
  return z.object({
    firstName: z.string().min(1, t("VALIDATION.REQUIRED", { ns: "errors" })),
    lastName: z.string().min(1, t("VALIDATION.REQUIRED", { ns: "errors" })),
    username: z
      .string()
      .min(3, t("VALIDATION.MIN_LENGTH", { ns: "errors", min: 3 })),
    email: z.email(t("VALIDATION.INVALID_EMAIL", { ns: "errors" })),
    password: z
      .string()
      .min(8, t("VALIDATION.MIN_LENGTH", { ns: "errors", min: 8 })),
  });
}

export type SignUpFormValues = z.infer<ReturnType<typeof createSignUpSchema>>;
export function SignupForm({
  className,
  ...props
}: React.ComponentProps<"div">) {
  const { signUp } = useAuthStore();
  const navigate = useNavigate();
  const { t } = useTranslation(["auth", "errors"]);

  const signUpSchema = useMemo(() => createSignUpSchema(t), [t]);

  const {
    register,
    handleSubmit,
    setError,
    clearErrors,
    formState: { errors, isSubmitting },
  } = useForm<SignUpFormValues>({
    resolver: zodResolver(signUpSchema),
  });

  const businessErrorFields: Partial<Record<string, keyof SignUpFormValues>> = {
    "USER.USERNAME_ALREADY_EXISTS": "username",
    "USER.EMAIL_ALREADY_EXISTS": "email",
  };
  const onSubmit = async (data: SignUpFormValues) => {
    clearErrors();
    try {
      const { firstName, lastName, username, password, email } = data;
      await signUp(username, password, email, firstName, lastName);
      navigate("/signin");
    } catch (error) {
      const problem = getApiProblemDetails(error);
      if (!problem) {
        setError("root.server", {
          type: "server",
          message: translateApiErrorCode("SYSTEM.INTERNAL_ERROR"),
        });
        return;
      }

      if (problem.errors) {
        for (const [field, fieldErrors] of Object.entries(problem.errors)) {
          const firstError = fieldErrors[0];
          if (!firstError) {
            continue;
          }
          if (
            field === "username" ||
            field === "password" ||
            field === "email" ||
            field === "firstName" ||
            field === "lastName"
          ) {
            setError(field, {
              type: "server",
              message: translateApiErrorCode(
                firstError.code,
                firstError.params,
              ),
            });
          }
        }
        return;
      }
      const businessField = businessErrorFields[problem.code];

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
    }
  };

  return (
    <div className={cn("flex flex-col gap-6", className)} {...props}>
      <Card className="overflow-hidden p-0 border-border">
        <CardContent className="grid p-0 md:grid-cols-2">
          <form className="p-4 sm:p-6 md:p-8" onSubmit={handleSubmit(onSubmit)}>
            <div className="flex flex-col gap-5 sm:gap-6">
              {/* header - logo */}
              <div className="flex flex-col items-center text-center gap-2">
                <a href="/" className="mx-auto block w-fit text-center">
                  <img src="/logo.svg" alt="logo" />
                </a>
                <h1 className="text-2xl font-bold">{t("signUp.title")}</h1>
                <p className="text-muted-foreground text-balance">
                  {t("signUp.description")}
                </p>
              </div>

              {/* họ và tên */}
              <div className="grid grid-cols-1 gap-3 min-[380px]:grid-cols-2">
                <div className="space-y-2">
                  <Label htmlFor="firstName" className="block text-sm">
                    {t("fields.firstName")}
                  </Label>
                  <Input
                    type="text"
                    id="firstName"
                    {...register("firstName")}
                  />
                  {errors.firstName && (
                    <p className="error-message">{errors.firstName.message}</p>
                  )}
                </div>
                <div className="space-y-2">
                  <Label htmlFor="lastName" className="block text-sm">
                    {t("fields.lastName")}
                  </Label>
                  <Input type="text" id="lastName" {...register("lastName")} />
                  {errors.lastName && (
                    <p className="error-message">{errors.lastName.message}</p>
                  )}
                </div>
              </div>
              {/* username */}
              <div className="flex flex-col gap-3">
                <Label htmlFor="username" className="block text-sm">
                  {t("fields.username")}
                </Label>
                <Input
                  type="text"
                  id="username"
                  placeholder="cirmin"
                  {...register("username")}
                />
                {errors.username && (
                  <p className="error-message">{errors.username.message}</p>
                )}
              </div>
              {/* email */}
              <div className="flex flex-col gap-3">
                <Label htmlFor="email" className="block text-sm">
                  {t("fields.email")}
                </Label>
                <Input
                  type="email"
                  id="email"
                  placeholder="cirmin@gmail.com"
                  {...register("email")}
                />
                {errors.email && (
                  <p className="error-message">{errors.email.message}</p>
                )}
              </div>
              {/* password */}
              <div className="flex flex-col gap-3">
                <Label htmlFor="password" className="block text-sm">
                  {t("fields.password")}
                </Label>
                <Input
                  type="password"
                  id="password"
                  {...register("password")}
                />
                {errors.password && (
                  <p className="error-message">{errors.password.message}</p>
                )}
              </div>
              {/* nút đăng ký */}
              {errors.root?.server && (
                <p className="error-message">{errors.root.server.message}</p>
              )}
              <Button type="submit" className="w-full" disabled={isSubmitting}>
                {t("signUp.submit")}
              </Button>

              <div className="text-center text-sm">
                {t("signUp.switchPrompt")}{" "}
                <a href="/signin" className="underline underline-offset-4">
                  {t("signIn.submit")}
                </a>
              </div>
            </div>
          </form>
          <div className="relative hidden bg-[#F8FAFC] dark:bg-slate-800 md:block">
            <img
              src="/placeholderSignUp.svg"
              alt="Image"
              className="absolute top-1/2 -translate-y-1/2 object-cover"
            />
          </div>
        </CardContent>
      </Card>
      <div className=" text-xs text-balance px-6 text-center *:[a]:hover:text-primary text-muted-foreground *:[a]:underline *:[a]:underline-offset-4">
        {t("termsAgreement.prefix")} <a href="#">{t("termsAgreement.terms")}</a>{" "}
        {t("termsAgreement.link")} <a href="#">{t("termsAgreement.privacy")}</a>
      </div>
    </div>
  );
}
