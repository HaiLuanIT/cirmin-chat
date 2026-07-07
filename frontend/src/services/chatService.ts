import api from "@/lib/axios";
import type { ConversationResponse, Message } from "@/types/chat";

const pageLimit = 50;
interface FetchMessageProps {
  messages: Message[];
  cursor?: string;
  hasMore: boolean;
}
export const chatService = {
  async fetchConversations(): Promise<ConversationResponse> {
    const res = await api.get("/Conversations");
    console.log("Group1", res.data);
    return res.data;
  },

  async fetchMessage(id: string, cursor?: string): Promise<FetchMessageProps> {
    const res = await api.get(
      `/conversations/${id}/messages?limit=${pageLimit}&cursor=${cursor}`,
    );
    return {
      messages: res.data.items,
      cursor: res.data.nextCursor,
      hasMore: res.data.hasMore,
    };
  },

  async sendMessage(
    content: string = "",
    imgUrl?: string,
    conversationId?: string,
  ) {
    const res = await api.post("/message/send", {
      content,
      conversationId,
      imgUrl,
    });
    return res.data;
  },
};
