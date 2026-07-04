import { SidebarProvider } from "../components/ui/sidebar";
import { AppSidebar } from "../components/sidebars/app-sidebar";
import ChatWindowLayout from "../components/chat/ChatWindowLayout";

const ChatAppPage = () => {
  return (
    <SidebarProvider>
      <AppSidebar />
      <div className="flex w-full h-screen p-2">
        <ChatWindowLayout />
      </div>
    </SidebarProvider>
  );
};

export default ChatAppPage;
