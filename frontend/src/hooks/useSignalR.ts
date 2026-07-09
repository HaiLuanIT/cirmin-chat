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
  const markAsSeen = useChatStore((s) => s.markAsSeen);
  const token = useAuthStore((s) => s.accessToken);

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
      addMessage(messageResponse);
      useChatStore.getState().updateConversation(messageResponse);
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
  }, [token, setOnlineUsers, setStatusUser]);
};
