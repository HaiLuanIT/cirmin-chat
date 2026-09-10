import type { Dispatch, SetStateAction } from "react";
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "../ui/dialog";
import ProfileCard from "./ProfileCard";
import { useAuthStore } from "@/stores/useAuthStore";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "../ui/tabs";
import PersonalInfoCard from "./PersonalInfoCard";
import { ShieldCheck, UserRound } from "lucide-react";
import SecurityInfoCard from "./SecurityInfoCard";
import { useTranslation } from "react-i18next";

interface ProfileDialogProps {
  open: boolean;
  setOpen: Dispatch<SetStateAction<boolean>>;
}
const ProfileDialog = ({ open, setOpen }: ProfileDialogProps) => {
  const { user } = useAuthStore();
  const { t } = useTranslation("profile");
  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogContent className="overflow-y-auto p-0 bg-transparent border-0 shadow-2xl max-h-[90vh] sm:max-w-4xl">
        <div className="bg-gradient-glass">
          <div className="mx-auto p-4">
            {/* heading */}
            <DialogHeader className="mb-6">
              <DialogTitle className="text-2xl font-bold text-foreground">
                {t("title")}
              </DialogTitle>
            </DialogHeader>
            <ProfileCard user={user} />
            {/* Tabs */}
            <Tabs defaultValue="account" className="mt-5 gap-4 flex-col">
              <TabsList className="grid w-full grid-cols-2 gap-1 rounded-xl border border-white/30 bg-background/45 p-1.5 shadow-sm backdrop-blur-xl group-data-horizontal/tabs:h-auto dark:border-white/10 dark:bg-background/25">
                <TabsTrigger
                  value="account"
                  className="profile-tab-trigger cursor-pointer min-h-11 rounded-lg px-2 py-2.5 text-xs font-semibold transition-smooth hover:bg-background/50 hover:text-foreground sm:text-sm"
                >
                  <UserRound className="size-4" />
                  {t("tabs.account")}
                </TabsTrigger>
                <TabsTrigger
                  value="security"
                  className="profile-tab-trigger cursor-pointer min-h-11 rounded-lg px-2 py-2.5 text-xs font-semibold transition-smooth hover:bg-background/50 hover:text-foreground sm:text-sm"
                >
                  <ShieldCheck className="size-4" />
                  {t("tabs.security")}
                </TabsTrigger>
              </TabsList>
              <TabsContent
                value="account"
                className="mt-0 animate-in fade-in-0 slide-in-from-bottom-1 duration-200"
              >
                {/* Auth store khởi tạo user là null, chỉ render form khi đã có dữ liệu. */}
                {user && <PersonalInfoCard user={user} />}
              </TabsContent>
              <TabsContent
                value="security"
                className="mt-0 animate-in fade-in-0 slide-in-from-bottom-1 duration-200"
              >
                <SecurityInfoCard />
              </TabsContent>
            </Tabs>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
};

export default ProfileDialog;
