import type { User } from "@/types/user";
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "../ui/card";
import { Field, FieldDescription, FieldError, FieldLabel } from "../ui/field";
import { Input } from "../ui/input";
import { Textarea } from "../ui/textarea";
import { Button } from "../ui/button";
import { Loader2, Save } from "lucide-react";
import { useForm, useWatch } from "react-hook-form";
import { useUserStore } from "@/stores/useUserStore";
import { useTranslation } from "react-i18next";
import { getApiProblemDetails, translateApiErrorCode } from "@/lib/api-error";
interface PersonalInfoCardProps {
  user: User;
}

interface PersonalInfoFormValues {
  displayName: string;
  email: string;
  bio: string;
}

const DISPLAY_NAME_MAX_LENGTH = 100;
const EMAIL_MAX_LENGTH = 50;
const BIO_MAX_LENGTH = 101;

const PersonalInfoCard = ({ user }: PersonalInfoCardProps) => {
  const { updateUserInfo } = useUserStore();

  const { t } = useTranslation(["common", "profile"]);
  const {
    register,
    handleSubmit,
    setError,
    reset,
    clearErrors,
    control,
    formState: { errors, isDirty, isSubmitting },
  } = useForm<PersonalInfoFormValues>({
    defaultValues: {
      displayName: user.displayName,
      email: user.email,
      // User.bio là optional, nhưng textarea luôn nên nhận một chuỗi.
      bio: user.bio ?? "",
    },
  });
  // useWatch chỉ đăng ký theo dõi field bio cho bộ đếm ký tự.
  const bioValue = useWatch({ control, name: "bio" });

  const handleUpdate = handleSubmit(async (formValues) => {
    const normalizedValues = {
      displayName: formValues.displayName.trim(),
      email: formValues.email.trim(),
      bio: formValues.bio.trim(),
    };

    clearErrors();
    try {
      await updateUserInfo(
        normalizedValues.displayName,
        normalizedValues.email,
        normalizedValues.bio,
      );

      // Sau khi lưu thành công, reset dùng dữ liệu mới làm mốc để isDirty trở về false.
      reset(normalizedValues);
    } catch (error) {
      const problem = getApiProblemDetails(error);

      if (!problem) {
        setError("root.server", {
          message: translateApiErrorCode("SYSTEM.INTERNAL_ERROR"),
        });
        return;
      }

      if (problem.errors) {
        for (const [field, fieldErrors] of Object.entries(problem.errors)) {
          const firstError = fieldErrors[0];

          if (
            (firstError && field === "displayName") ||
            field === "email" ||
            field === "bio"
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

      if (problem.code === "USER.EMAIL_ALREADY_EXISTS") {
        setError("email", {
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
  });

  return (
    // Form bọc Card để button trong CardFooter vẫn submit được toàn bộ form.
    <form onSubmit={handleUpdate}>
      <Card className="border-white/20 bg-card/80 shadow-sm backdrop-blur-xl dark:border-white/10">
        <CardHeader className="gap-2 border-b border-gray-300 bg-muted/30 px-5 py-5">
          <CardTitle className="text-xl font-semibold">
            {t("profile:account.title")}
          </CardTitle>
          <CardDescription>{t("profile:account.description")}</CardDescription>
        </CardHeader>

        <CardContent className="px-5">
          <div className="grid grid-cols-1 gap-5 md:grid-cols-2">
            <Field data-invalid={Boolean(errors.displayName)}>
              <FieldLabel htmlFor="displayName">
                {t("profile:account.fields.displayName")}
              </FieldLabel>
              <Input
                id="displayName"
                autoComplete="name"
                aria-invalid={Boolean(errors.displayName)}
                {...register("displayName", {
                  required: t("VALIDATION.REQUIRED", { ns: "errors" }),
                  maxLength: {
                    value: DISPLAY_NAME_MAX_LENGTH,
                    message: t("VALIDATION.MAX_LENGTH", {
                      ns: "errors",
                      max: DISPLAY_NAME_MAX_LENGTH,
                    }),
                  },
                  validate: (value) =>
                    value.trim().length > 0 ||
                    t("VALIDATION.NOT_ONLY_WHITESPACE", { ns: "errors" }),
                })}
              />
              <FieldError errors={[errors.displayName]} />
            </Field>

            <Field>
              <FieldLabel htmlFor="username">
                {t("profile:account.fields.username")}
              </FieldLabel>
              <Input
                id="username"
                value={user.username}
                readOnly
                aria-readonly="true"
                className="cursor-default bg-muted/50"
              />
              <FieldDescription>
                {t("profile:account.descriptions.username")}
              </FieldDescription>
            </Field>

            {/* Email dài và quan trọng nên chiếm trọn hàng trên desktop. */}
            <Field
              data-invalid={Boolean(errors.email)}
              className="md:col-span-2"
            >
              <FieldLabel htmlFor="email">
                {t("profile:account.fields.email")}
              </FieldLabel>
              <Input
                id="email"
                type="email"
                autoComplete="email"
                aria-invalid={Boolean(errors.email)}
                {...register("email", {
                  required: t("VALIDATION.REQUIRED", { ns: "errors" }),
                  maxLength: {
                    value: EMAIL_MAX_LENGTH,
                    message: t("VALIDATION.MAX_LENGTH", {
                      ns: "errors",
                      max: EMAIL_MAX_LENGTH,
                    }),
                  },
                  pattern: {
                    value: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
                    message: t("VALIDATION.INVALID_FORMAT", { ns: "errors" }),
                  },
                })}
              />
              <FieldError errors={[errors.email]} />
            </Field>

            <Field data-invalid={Boolean(errors.bio)} className="md:col-span-2">
              <FieldLabel htmlFor="bio">
                {t("profile:account.fields.bio")}
              </FieldLabel>
              <Textarea
                id="bio"
                rows={4}
                maxLength={BIO_MAX_LENGTH}
                placeholder={t("profile:account.placeholders.bio")}
                aria-invalid={Boolean(errors.bio)}
                className="resize-none"
                {...register("bio", {
                  maxLength: {
                    value: BIO_MAX_LENGTH,
                    message: t("profile:account.validation.bioMaxLength", {
                      max: BIO_MAX_LENGTH,
                    }),
                  },
                })}
              />

              {/* Meta dùng flex ngang để lỗi nằm trái và bộ đếm nằm phải. */}
              <div className="flex items-start justify-between gap-4">
                <FieldError errors={[errors.bio]} />
                <span className="ml-auto shrink-0 text-xs text-muted-foreground">
                  {bioValue.length}/{BIO_MAX_LENGTH}
                </span>
              </div>
            </Field>
          </div>
        </CardContent>

        <CardFooter className="flex-col items-stretch justify-between gap-3 px-5 sm:flex-row sm:items-center border-none bg-card/80">
          <p className="text-xs text-muted-foreground">
            {isDirty
              ? t("profile:account.status.unsaved")
              : t("profile:account.status.saved")}
          </p>

          {errors.root?.server && (
            <p className="error-message text-center">
              {errors.root.server.message}
            </p>
          )}
          <Button
            type="submit"
            disabled={!isDirty || isSubmitting}
            className="w-full sm:w-auto"
          >
            {isSubmitting ? (
              <>
                <Loader2 className="animate-spin" />
                {t("actions.saving")}
              </>
            ) : (
              <>
                <Save />
                {t("actions.save")}
              </>
            )}
          </Button>
        </CardFooter>
      </Card>
    </form>
  );
};

export default PersonalInfoCard;
