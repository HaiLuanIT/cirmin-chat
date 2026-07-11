import { usePresenceStore } from "@/stores/usePresenceStore";
import React, { useEffect } from "react";
import { signalRService } from "@/services/signalRService";
import * as signalR from "@microsoft/signalr";
import { useAuthStore } from "@/stores/useAuthStore";
import { useChatStore } from "@/stores/useChatStore";
import type { Message } from "@/types/chat";

export const useSignalR = () => {
  const setOnlineUsers = usePresenceStore((s) => s.setOnlineUsers);
  const setStatusUser = usePresenceStore((s) => s.setStatusUser);
  const addMessage = useChatStore((s) => s.addMessage);
  const updateLastMessage = useChatStore((s) => s.updateLastMessage);
  const token = useAuthStore((s) => s.accessToken);
  const { user } = useAuthStore();
  const { incrementUnreadCount, clearUnreadCount } = useChatStore();

  useEffect(() => {
    if (!token) {
      return;
    }
    const connection = signalRService.initConnection(
      useAuthStore.getState().accessToken,
    );

    const handleSetOnlineUsers = (userIds: string[]) => {
      setOnlineUsers(userIds);
    };

    const handleSetUserStatus = (userId: string, isOnline: boolean) => {
      setStatusUser(userId, isOnline);
    };

    const handleAddMessage = (messageResponse: Message) => {
      const currentActiveId = useChatStore.getState().activeConversationId;
      addMessage(messageResponse);
      updateLastMessage(messageResponse);

      if (messageResponse?.sender?.senderId !== user.id) {
        if (currentActiveId === messageResponse?.conversationId) {
          connection
            .invoke("MarkConversationAsRead", messageResponse?.conversationId)
            .catch((err) =>
              console.error("Lỗi khi cập nhật trạng thái đã xem", err),
            );
          clearUnreadCount(messageResponse.conversationId);
        } else {
          incrementUnreadCount(messageResponse.conversationId);
        }
      }
    };

    connection.on("GetOnlineUsers", handleSetOnlineUsers);

    connection.on("UserStatusChanged", handleSetUserStatus);

    connection.on("ReceiveMessage", handleAddMessage);

    if (connection.state === signalR.HubConnectionState.Disconnected) {
      connection
        .start()
        .then(() => console.log("Kết nối với signalR thành công."))
        .catch((err) => console.error("Kết nối với signalR thất bại", err));
    }
    return () => {
      connection.off("GetOnlineUsers", handleSetOnlineUsers);
      connection.off("UserStatusChanged", handleSetUserStatus);
      connection.off("ReceiveMessage", handleAddMessage);
      console.log("Tắt lắng nghe sự kiện SignalR");
    };
  }, [token, user?.id, setOnlineUsers, setStatusUser]);
};
