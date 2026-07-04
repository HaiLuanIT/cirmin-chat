import { useChatStore } from "@/stores/useChatStore";
import React from "react";
import ChatWelcomeScreen from "./ChatWelcomeScreen";
import MessageItem from "./MessageItem";

const ChatWindowBody = () => {
  const {
    activeConversationId,
    conversations,
    messages: allMessages,
  } = useChatStore();

  const messages = allMessages[activeConversationId!]?.items ?? [];

  const selectedConversation = conversations.find(
    (c) => c.id === activeConversationId,
  );

  if (!selectedConversation) {
    return <ChatWelcomeScreen />;
  }

  if (!messages?.length) {
    return (
      <div className="flex h-full items-center justify-center text-muted-foreground">
        Chưa có tin nhắn nào trong cuộc trò chuyện này
      </div>
    );
  }

  return (
    <div className="p-4 bg-primary-foreground h-full flex flex-col overflow-hidden">
      <div className="flex overflow-y-auto overflow-x-hidden beautiful-scrollbar flex-col-reverse">
        {messages.map((message, index) => (
          <MessageItem
            message={message}
            index={index}
            messages={messages}
            selectedConvo={selectedConversation}
            key={message.id ?? index}
            lastMessageStatus="delivered"
          />
        ))}
      </div>
    </div>
  );
};

export default ChatWindowBody;
