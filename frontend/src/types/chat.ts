export interface ConversationMember {
  userId: string;
  displayName: string;
  avatarUrl?: string | null;
  lastMessageId: string;
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
  sender: {
    senderId: string;
    displayName: string;
    avatarUrl: string;
  };
  conversationId: string;
  content: string;
  sentAt: string;
  isOwn: boolean;
}

export interface ConversationResponse {
  conversations: Conversation[];
}
