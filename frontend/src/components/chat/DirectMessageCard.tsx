import type { Conversation } from "@/types/chat";
import React from "react";
import ChatCard from "./ChatCard";
import { useAuthStore } from "@/stores/useAuthStore";
import { useChatStore } from "@/stores/useChatStore";
import { cn } from "@/lib/utils";
import UserAvatar from "./UserAvatar";
import StatusBadge from "./StatusBadge";
import UnreadCountBadge from "./UnreadCountBadge";

const DirectMessageCard = ({ convo }: { convo: Conversation }) => {
  const { user } = useAuthStore();
  const { activeConversationId, setActiveConversation, messages } =
    useChatStore();

  if (!user) return null;

  const otherUser = convo.members.find((p) => p.userId !== user.id);
  if (!otherUser) return null;

  const unreadCount = convo.unreadCount;
  const lastMessage = convo.lastMessage?.lastMessageContent ?? "";

  const handleSelectConversation = async (id: string) => {
    setActiveConversation(id);
    if (!messages[id]) {
      // todo: fetch messages
    }
  };
  console.log(`Lan ${convo.id}`, convo);
  return (
    <ChatCard
      convoId={convo.id}
      name={otherUser.displayName ?? ""}
      timestamp={
        convo.lastMessage?.lastMessageAt
          ? new Date(convo.lastMessage.lastMessageAt)
          : undefined
      }
      isActive={activeConversationId === convo.id}
      onSelect={handleSelectConversation}
      unreadCount={unreadCount}
      leftSection={
        <>
          {/* todo: user avatar */}
          <UserAvatar
            type="sidebar"
            name={otherUser.displayName ?? ""}
            avatarUrl={otherUser.avatarUrl ?? undefined}
          />
          {/* todo: status badge */}
          {/* todo: socket.io */}
          <StatusBadge status="offline" />
          {/* todo: unread count */}
          {unreadCount > 0 && <UnreadCountBadge unreadCount={unreadCount} />}
        </>
      }
      subtile={
        <p
          className={cn(
            "text-sm truncate",
            unreadCount > 0
              ? "font-medium text-foreground"
              : "text-muted-foreground",
          )}
        >
          {lastMessage}
        </p>
      }
    />
  );
};

export default DirectMessageCard;
