import * as React from "react";
import { NavUser } from "./nav-user";
import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarGroup,
  SidebarGroupAction,
  SidebarGroupContent,
  SidebarGroupLabel,
  SidebarHeader,
} from "../ui/sidebar";
import { Moon, Sun } from "lucide-react";
import { Switch } from "../ui/switch";
import CreateNewChat from "../chat/CreateNewChat";
import NewGroupChatModal from "../chat/NewGroupChatModal";
import GroupChatList from "../chat/GroupChatList";
import AddFriendModal from "../chat/AddFriendModal";
import DirectMessageList from "../chat/DirectMessageList";
import { useThemeStore } from "@/stores/useThemeStore";
import { useAuthStore } from "@/stores/useAuthStore";
import LanguagesSelector from "../languagesSelector/LanguagesSelector";
import { useTranslation } from "react-i18next";

export function AppSidebar({ ...props }: React.ComponentProps<typeof Sidebar>) {
  const { isDark, toggleTheme } = useThemeStore();
  const { user } = useAuthStore();
  const { t } = useTranslation("common");

  return (
    <Sidebar variant="inset" {...props}>
      {/* Header */}
      <SidebarHeader>
        <div className="rounded-xl bg-gradient-primary p-2.5 shadow-sm">
          <div className="flex items-center justify-between gap-2">
            <h1 className="px-1 text-xl font-bold tracking-tight text-white">
              Moji
            </h1>

            <div className="flex items-center gap-1.5">
              <LanguagesSelector
                compact
                className="border-white/20 bg-white/10 text-white shadow-none hover:bg-white/20 hover:text-white [&_svg]:text-white/80"
              />

              <div className="flex h-8 items-center gap-1 rounded-lg border border-white/20 bg-white/10 px-1.5">
                <Sun className="size-3.5 text-white/80" aria-hidden="true" />
                <Switch
                  checked={isDark}
                  onCheckedChange={toggleTheme}
                  aria-label="Toggle theme"
                  className="scale-90 data-[state=checked]:bg-background/80"
                />
                <Moon className="size-3.5 text-white/80" aria-hidden="true" />
              </div>
            </div>
          </div>
        </div>
      </SidebarHeader>
      {/* Content*/}
      <SidebarContent className="beautiful-scrollbar">
        {/* New chat */}
        <SidebarGroup>
          <SidebarGroupContent>
            <CreateNewChat />
          </SidebarGroupContent>
        </SidebarGroup>

        {/* Group chat */}
        <SidebarGroup>
          <div className="flex items-center justify-between">
            <SidebarGroupLabel className="uppercase">
              {t("sidebar.groups.label")}
            </SidebarGroupLabel>
            <NewGroupChatModal />
          </div>

          <SidebarGroupContent>
            <GroupChatList />
          </SidebarGroupContent>
        </SidebarGroup>

        {/* Direct message */}
        <SidebarGroup>
          <SidebarGroupLabel className="uppercase">
            {t("sidebar.directs.label")}
          </SidebarGroupLabel>
          <SidebarGroupAction title="Kết Bạn" className="cursor-pointer">
            <AddFriendModal />
          </SidebarGroupAction>
          <SidebarGroupContent>
            <DirectMessageList />
          </SidebarGroupContent>
        </SidebarGroup>
      </SidebarContent>
      {/* Footer */}
      <SidebarFooter>{user && <NavUser user={user} />}</SidebarFooter>
    </Sidebar>
  );
}
