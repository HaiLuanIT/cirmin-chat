import React from "react";
import { useChatStore } from "@/stores/useChatStore";
import DirectMessageCard from "./DirectMessageCard";

const DirectMessageList = () => {
  const { conversations } = useChatStore();
  if (!conversations) return;

  const directConversations = conversations.filter(
    (convo) => convo.isGroup === false,
  );
  return (
    <div className="flex-1 overflow-auto p-2 space-y-2">
      {directConversations.map((convo) => (
        <DirectMessageCard convo={convo} key={convo.id} />
      ))}
    </div>
  );
};

export default DirectMessageList;
