import { signalRService } from "@/services/signalRService";
import { useChatStore } from "@/stores/useChatStore";
import { useEffect } from "react";

export const useChatRoom = (conversationId: string, loading: boolean) => {
  const { activeConversationId, clearUnreadCount, markAsSeen } = useChatStore();

  useEffect(() => {
    if (!conversationId || loading) return;

    if (conversationId === activeConversationId) {
      const connection = signalRService.getConnection();
      if (connection || signalRService.isConnected) {
        clearUnreadCount(conversationId);
        connection
          .invoke("MarkConversationAsRead", conversationId)
          .catch(async (err) => {
            console.error(
              "Đã xảy ra lỗi khi cập nhật trạng thái đã xem tin nhắn",
              err,
            );
            markAsSeen(conversationId);
          });
      } else {
        markAsSeen(conversationId);
      }
    }
  }, [
    conversationId,
    loading,
    activeConversationId,
    clearUnreadCount,
    markAsSeen,
  ]);
};
