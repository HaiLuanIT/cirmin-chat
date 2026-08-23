import { SidebarInset } from "../ui/sidebar";
import { useTranslation } from "react-i18next";

const ChatWelcomeScreen = () => {
  const { t } = useTranslation("chat");
  return (
    <SidebarInset className="flex w-full h-full bg-transparent">
      <div className="flex bg-background rounded-2xl flex-1 items-center justify-center">
        <div className="text-center">
          <div className="size-24 mx-auto bg-gradient-chat rounded-full flex items-center justify-center shadow-glow pulse-ring mb-5">
            <span className="text-3xl">💭</span>
          </div>
          <h2 className="text-2xl font-bold mb-2 bg-gradient-chat bg-clip-text text-transparent">
            {t("welcome.title")}
          </h2>
          <p className="text-muted-foreground">{t("welcome.description")}</p>
        </div>
      </div>
    </SidebarInset>
  );
};

export default ChatWelcomeScreen;
