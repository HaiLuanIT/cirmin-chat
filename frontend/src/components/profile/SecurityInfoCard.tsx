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
import { Loader, Loader2, Save } from "lucide-react";
import { useAuthStore } from "@/stores/useAuthStore";
import { useUserStore } from "@/stores/useUserStore";
import { toast } from "sonner";
import { useTranslation } from "react-i18next";

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
    formState: { isSubmitting },
  } = useForm<IFormValue>({
    defaultValues: {
      password: "",
      newPassword: "",
      confirmNewPassword: "",
    },
  });

  const handleChangePassword = handleSubmit(async (data) => {
    if (data.newPassword != data.confirmNewPassword) {
      toast.error("Mật khẩu nhập lại không trùng khớp.");
      return;
    }
    await changePassword(data.password, data.newPassword);
    reset();
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
