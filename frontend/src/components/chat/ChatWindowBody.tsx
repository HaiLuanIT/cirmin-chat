import { useChatStore } from "@/stores/useChatStore";
import React, { useLayoutEffect, useRef } from "react";
import ChatWelcomeScreen from "./ChatWelcomeScreen";
import MessageItem from "./MessageItem";
import { useChatRoom } from "@/hooks/useChatRoom";
import InfiniteScroll from "react-infinite-scroll-component";

const ChatWindowBody = () => {
  const {
    activeConversationId,
    conversations,
    messages: allMessages,
    messageLoading: loading,
  } = useChatStore();

  const messages = allMessages[activeConversationId!]?.items ?? [];
  const hasMore = allMessages[activeConversationId]?.hasMore ?? false;
  const { fetchMessages } = useChatStore();
  const key = `chat-scroll-${activeConversationId}`;
  //ref
  const messageEndRef = useRef<HTMLDivElement>(null);
  const containerRef = useRef<HTMLDivElement>(null);

  const selectedConversation = conversations.find(
    (c) => c.id === activeConversationId,
  );

  useChatRoom(selectedConversation?.id ?? "", loading);

  // kéo xuống dưới khi load convo
  useLayoutEffect(() => {
    if (!messageEndRef.current) return;

    messageEndRef.current.scrollIntoView({
      behavior: "smooth",
      block: "end",
    });
  }, [activeConversationId]);

  //scroll tới vị trí cuối cùng đã scroll
  useLayoutEffect(() => {
    const container = containerRef.current;
    if (!container) return;
    const item = sessionStorage.getItem(key);
    if (item) {
      const { scrollTop } = JSON.parse(item);
      requestAnimationFrame(() => {
        container.scrollTop = scrollTop;
      });
    }
  }, [messages.length]);

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

  const fetchMoreMessage = async () => {
    if (!activeConversationId) return;
    try {
      await fetchMessages(activeConversationId);
    } catch (error) {
      console.error("Lỗi xảy ra khi load thêm tin", error);
    }
  };

  const handleScrollSave = () => {
    const container = containerRef.current;
    if (!container || !activeConversationId) return;

    sessionStorage.setItem(
      key,
      JSON.stringify({
        scrollTop: container.scrollTop,
        scrollHeight: container.scrollHeight,
      }),
    );
  };

  return (
    <div className="p-4 bg-primary-foreground h-full flex flex-col overflow-hidden">
      <div
        id="scollableDiv"
        ref={containerRef}
        onScroll={handleScrollSave}
        className="flex overflow-y-auto overflow-x-hidden beautiful-scrollbar flex-col-reverse"
      >
        <div ref={messageEndRef}></div>
        <InfiniteScroll
          dataLength={messages.length}
          next={fetchMoreMessage}
          hasMore={hasMore}
          scrollableTarget="scollableDiv"
          loader={<p>Đang tải...</p>}
          inverse={true}
          style={{
            display: "flex",
            flexDirection: "column-reverse",
            overflow: "visible",
          }}
        >
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
        </InfiniteScroll>
      </div>
    </div>
  );
};

export default ChatWindowBody;
