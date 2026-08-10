import { useAuthStore } from "@/stores/useAuthStore";
import { useChatStore } from "@/stores/useChatStore";
import type { Conversation } from "@/types/chat";
import React from "react";
import ChatCard from "./ChatCard";
import UnreadCountBadge from "./UnreadCountBadge";
import GroupChatAvatar from "./GroupChatAvatar";
import { useTranslation } from "react-i18next";

const GroupChatCard = ({ convo }: { convo: Conversation }) => {
  const { user } = useAuthStore();
  const { t } = useTranslation("chat");
  const {
    activeConversationId,
    setActiveConversation,
    messages,
    fetchMessages,
    markAsSeen,
  } = useChatStore();
  if (!user) return null;

  const unreadCount = convo.unreadCount;
  const name = convo.name ?? "";
  const handleSelectConversation = async (id: string) => {
    setActiveConversation(id);
    await markAsSeen(id);
    if (!messages[id]) {
      await fetchMessages();
    }
  };
  return (
    <ChatCard
      convoId={convo.id}
      name={name}
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
          {unreadCount > 0 && <UnreadCountBadge unreadCount={unreadCount} />}
          <GroupChatAvatar participants={convo.members} type="chat" />
        </>
      }
      subtile={
        <p className="text-sm truncate text-muted-foreground">
          {convo.members.length} {t("conversations.group.member")}
        </p>
      }
    />
  );
};

export default GroupChatCard;
