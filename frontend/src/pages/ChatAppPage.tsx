import { SidebarProvider } from "../components/ui/sidebar";
import { AppSidebar } from "../components/sidebars/app-sidebar";
import ChatWindowLayout from "../components/chat/ChatWindowLayout";

const ChatAppPage = () => {
  return (
    <SidebarProvider className="h-svh min-h-0 overflow-hidden">
      <AppSidebar />
      <div className="flex h-full min-w-0 flex-1 p-0 md:p-2">
        <ChatWindowLayout />
      </div>
    </SidebarProvider>
  );
};

export default ChatAppPage;
