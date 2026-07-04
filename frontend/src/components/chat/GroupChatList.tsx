import { useChatStore } from "@/stores/useChatStore";
import React from "react";
import GroupChatCard from "./GroupChatCard";

const GroupChatList = () => {
  const { conversations } = useChatStore();

  if (!conversations) return;
  const groupConversation = conversations.filter(
    (convo) => convo.isGroup === true,
  );
  return (
    <div className="flex-1 overflow-auto p-2 space-y-2">
      {groupConversation.map((convo) => (
        <GroupChatCard convo={convo} key={convo.id} />
      ))}
    </div>
  );
};

export default GroupChatList;
