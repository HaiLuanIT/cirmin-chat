import { useChatStore } from "@/stores/useChatStore";
import ChatWelcomeScreen from "./ChatWelcomeScreen";
import ChatWindowSkeleton from "./ChatWindowSkeleton";
import { SidebarInset } from "../ui/sidebar";
import ChatWindowHeader from "./ChatWindowHeader";
import ChatWindowBody from "./ChatWindowBody";
import MessageInput from "./MessageInput";

const ChatWindowLayout = () => {
  const {
    activeConversationId,
    conversations,
    messageLoading: loading,
  } = useChatStore();

  const selectedConvo =
    conversations.find((c) => c?.id === activeConversationId) ?? null;
  if (!selectedConvo) return <ChatWelcomeScreen />;

  if (loading) {
    return <ChatWindowSkeleton />;
  }
  return (
    <SidebarInset className="flex h-full min-w-0 flex-1 flex-col overflow-hidden rounded-none shadow-md md:rounded-sm">
      <ChatWindowHeader chat={selectedConvo} />
      <div className="min-h-0 flex-1 overflow-y-auto bg-background">
        <ChatWindowBody />
      </div>
      <MessageInput />
    </SidebarInset>
  );
};

export default ChatWindowLayout;
