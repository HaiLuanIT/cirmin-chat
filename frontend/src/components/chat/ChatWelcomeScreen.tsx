import { SidebarInset, SidebarTrigger } from "../ui/sidebar";
import { useTranslation } from "react-i18next";

const ChatWelcomeScreen = () => {
  const { t } = useTranslation("chat");
  return (
    <SidebarInset className="flex h-full min-w-0 flex-col bg-transparent">
      <header className="flex h-14 shrink-0 items-center px-3">
        <SidebarTrigger className="text-foreground" />
      </header>
      <div className="flex flex-1 items-center justify-center rounded-none bg-background px-5 md:rounded-2xl">
        <div className="max-w-sm text-center">
          <div className="mx-auto mb-5 flex size-20 items-center justify-center rounded-full bg-gradient-chat shadow-glow pulse-ring sm:size-24">
            <span className="text-3xl">💭</span>
          </div>
          <h2 className="mb-2 bg-gradient-chat bg-clip-text text-xl font-bold text-transparent sm:text-2xl">
            {t("welcome.title")}
          </h2>
          <p className="text-sm text-muted-foreground sm:text-base">
            {t("welcome.description")}
          </p>
        </div>
      </div>
    </SidebarInset>
  );
};

export default ChatWelcomeScreen;
