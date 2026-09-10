import { useChatStore } from "@/stores/useChatStore";
import { useLayoutEffect, useRef } from "react";
import ChatWelcomeScreen from "./ChatWelcomeScreen";
import MessageItem from "./MessageItem";
import { useChatRoom } from "@/hooks/useChatRoom";
import InfiniteScroll from "react-infinite-scroll-component";
import { useTranslation } from "react-i18next";

const ChatWindowBody = () => {
  const {
    activeConversationId,
    conversations,
    messages: allMessages,
    messageLoading: loading,
  } = useChatStore();

  const currentMessage = activeConversationId
    ? allMessages[activeConversationId]
    : undefined;
  const messages = currentMessage?.items ?? [];
  const { t } = useTranslation("chat");
  const hasMore = currentMessage?.hasMore ?? false;
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
  }, [key, messages.length]);

  if (!selectedConversation) {
    return <ChatWelcomeScreen />;
  }
  if (!messages?.length) {
    return (
      <div className="flex h-full items-center justify-center bg-background text-muted-foreground">
        {t("message.empty")}
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
    <div className="flex h-full min-h-0 flex-col overflow-hidden bg-background p-2 sm:p-4">
      <div
        id="scollableDiv"
        ref={containerRef}
        onScroll={handleScrollSave}
        className="beautiful-scrollbar flex min-h-0 flex-1 flex-col-reverse overflow-y-auto overflow-x-hidden"
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
