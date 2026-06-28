import api from "@/lib/axios";
import type { Conversation, ConversationResponse, Message } from "@/types/chat";

export const chatService = {
  async fetchConversations(): Promise<ConversationResponse> {
    const res = await api.get("/Conversations");
    console.log("Group1", res.data);
    return res.data;
  },
};
