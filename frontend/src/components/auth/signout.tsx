import { useNavigate } from "react-router";
import { useAuthStore } from "../../stores/useAuthStore";
import { Button } from "../ui/button";
import { LogOut } from "lucide-react";
import { signalRService } from "@/services/signalRService";
import { useTranslation } from "react-i18next";

const SignOut = () => {
  const { signOut } = useAuthStore();
  const navigate = useNavigate();
  const { t } = useTranslation("auth");

  const handleSignOut = async () => {
    try {
      await signOut();

      await signalRService.stopConnection();
      navigate("/signin");
    } catch (error) {
      console.error(error);
    }
  };
  return (
    <Button onClick={handleSignOut} variant="completeGhost">
      <LogOut className="text-destructive" />
      {t("signOut.submit")}
    </Button>
  );
};

export default SignOut;
