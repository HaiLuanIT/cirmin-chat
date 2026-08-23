import { useChatStore } from "@/stores/useChatStore";
import type { Conversation } from "@/types/chat";
import { SidebarTrigger } from "../ui/sidebar";
import { useAuthStore } from "@/stores/useAuthStore";
import { Separator } from "../ui/separator";
import UserAvatar from "./UserAvatar";
import StatusBadge from "./StatusBadge";
import GroupChatAvatar from "./GroupChatAvatar";
import { usePresenceStore } from "@/stores/usePresenceStore";

const ChatWindowHeader = ({ chat }: { chat?: Conversation }) => {
  const { conversations, activeConversationId } = useChatStore();
  const { user } = useAuthStore();
  const { isOnline } = usePresenceStore();

  let otherUser;
  chat = chat ?? conversations.find((c) => c.id === activeConversationId);

  if (!chat) {
    return (
      <header className="md:hidden sticky top-0 z-10 flex items-center gap-2 px-4 py-2 w-full">
        <SidebarTrigger className="-ml-1 text-foreground" />
      </header>
    );
  }

  if (!chat.isGroup) {
    const otherUsers = chat.members.filter((p) => p.userId !== user?.id);
    otherUser = otherUsers.length > 0 ? otherUsers[0] : null;

    if (!user || !otherUser) return;
  }

  return (
    <header className="sticky top-0 z-10 px-3 py-2 flex items-center bg-background">
      <div className="flex items-center gap-2 w-full">
        <SidebarTrigger className="-ml-1 text-foreground" />
        <Separator
          orientation="vertical"
          className="mr-2 data-[orientation=vertical]:h-4"
        />
        <div className="p-2 w-full flex items-center gap-3">
          <div className="relative">
            {chat.isGroup ? (
              <GroupChatAvatar participants={chat.members} type="sidebar" />
            ) : otherUser ? (
              <>
                <UserAvatar
                  type={"sidebar"}
                  name={otherUser.displayName}
                  avatarUrl={otherUser.avatarUrl ?? undefined}
                />
                <StatusBadge
                  status={isOnline(otherUser.userId) ? "online" : "offline"}
                />
              </>
            ) : (
              <UserAvatar type="sidebar" name="CirMin" />
            )}
          </div>

          <h2 className="font-semibold text-foreground">
            {!chat.isGroup ? otherUser?.displayName : chat.name}
          </h2>
        </div>
      </div>
    </header>
  );
};

export default ChatWindowHeader;
