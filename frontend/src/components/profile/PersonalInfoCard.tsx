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
const BIO_MAX_LENGTH = 500;

const PersonalInfoCard = ({ user }: PersonalInfoCardProps) => {
  const { updateUserInfo } = useUserStore();

  const {
    register,
    handleSubmit,
    reset,
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

    try {
      await updateUserInfo(
        normalizedValues.displayName,
        normalizedValues.email,
        normalizedValues.bio,
      );

      // Sau khi lưu thành công, reset dùng dữ liệu mới làm mốc để isDirty trở về false.
      reset(normalizedValues);
    } catch {
      // Store đã hiển thị toast lỗi; giữ nguyên form để người dùng có thể thử lại.
    }
  });

  return (
    // Form bọc Card để button trong CardFooter vẫn submit được toàn bộ form.
    <form onSubmit={handleUpdate}>
      <Card className="border-white/20 bg-card/80 shadow-sm backdrop-blur-xl dark:border-white/10">
        <CardHeader className="gap-2 border-b border-gray-300 bg-muted/30 px-5 py-5">
          <CardTitle className="text-xl font-semibold">
            Thông tin cá nhân
          </CardTitle>
          <CardDescription>
            Cập nhật thông tin hiển thị trên hồ sơ và trong các cuộc trò chuyện.
          </CardDescription>
        </CardHeader>

        <CardContent className="px-5">
          {/*
            Wrapper này chỉ quản lý layout giữa các field:
            - Mobile: một cột.
            - Từ md: hai cột.
            Mỗi Field bên trong vẫn tự xếp label, input và lỗi theo chiều dọc.
          */}
          <div className="grid grid-cols-1 gap-5 md:grid-cols-2">
            <Field data-invalid={Boolean(errors.displayName)}>
              <FieldLabel htmlFor="displayName">Tên hiển thị</FieldLabel>
              <Input
                id="displayName"
                autoComplete="name"
                aria-invalid={Boolean(errors.displayName)}
                {...register("displayName", {
                  required: "Tên hiển thị không được để trống.",
                  maxLength: {
                    value: DISPLAY_NAME_MAX_LENGTH,
                    message: `Tên hiển thị không được vượt quá ${DISPLAY_NAME_MAX_LENGTH} ký tự.`,
                  },
                  validate: (value) =>
                    value.trim().length > 0 ||
                    "Tên hiển thị không được chỉ chứa khoảng trắng.",
                })}
              />
              <FieldError errors={[errors.displayName]} />
            </Field>

            <Field>
              <FieldLabel htmlFor="username">Username</FieldLabel>
              <Input
                id="username"
                value={user.username}
                readOnly
                aria-readonly="true"
                className="cursor-default bg-muted/50"
              />
              <FieldDescription>
                Username hiện không thể thay đổi.
              </FieldDescription>
            </Field>

            {/* Email dài và quan trọng nên chiếm trọn hàng trên desktop. */}
            <Field
              data-invalid={Boolean(errors.email)}
              className="md:col-span-2"
            >
              <FieldLabel htmlFor="email">Email</FieldLabel>
              <Input
                id="email"
                type="email"
                autoComplete="email"
                aria-invalid={Boolean(errors.email)}
                {...register("email", {
                  required: "Email không được để trống.",
                  maxLength: {
                    value: EMAIL_MAX_LENGTH,
                    message: `Email không được vượt quá ${EMAIL_MAX_LENGTH} ký tự.`,
                  },
                  pattern: {
                    value: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
                    message: "Email không đúng định dạng.",
                  },
                })}
              />
              <FieldError errors={[errors.email]} />
            </Field>

            <Field data-invalid={Boolean(errors.bio)} className="md:col-span-2">
              <FieldLabel htmlFor="bio">Giới thiệu</FieldLabel>
              <Textarea
                id="bio"
                rows={4}
                maxLength={BIO_MAX_LENGTH}
                placeholder="Viết một vài điều về bạn..."
                aria-invalid={Boolean(errors.bio)}
                className="resize-none"
                {...register("bio", {
                  maxLength: {
                    value: BIO_MAX_LENGTH,
                    message: `Giới thiệu không được vượt quá ${BIO_MAX_LENGTH} ký tự.`,
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
              ? "Bạn có thay đổi chưa được lưu."
              : "Thông tin hiện tại đã được lưu."}
          </p>

          <Button
            type="submit"
            disabled={!isDirty || isSubmitting}
            className="w-full sm:w-auto"
          >
            {isSubmitting ? (
              <>
                <Loader2 className="animate-spin" />
                Đang lưu...
              </>
            ) : (
              <>
                <Save />
                Lưu thay đổi
              </>
            )}
          </Button>
        </CardFooter>
      </Card>
    </form>
  );
};

export default PersonalInfoCard;
