export interface ConversationMember {
  userId: string;
  displayName: string;
  avatarUrl?: string | null;
}

export interface LastMessage {
  id?: string | null;
  lastMessageContent?: string | null;
  lastMessageAt?: string | null;
}
export interface Conversation {
  id: string;
  name: string;
  isGroup: boolean;
  createdAt: string;
  lastMessage?: LastMessage | null;
  unreadCount: number;
  members: ConversationMember[];
}

export interface Message {
  id: string;
  senderId: string;
  conversationId: string;
  message: string;
  sentAt: string;
}

export interface ConversationResponse {
  conversations: Conversation[];
}
