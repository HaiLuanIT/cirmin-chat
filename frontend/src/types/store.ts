import type { Conversation, Message } from "./chat";
import type { User } from "./user";

export interface AuthState {
  accessToken: string | null;
  user: User | null;
  loading: boolean;

  clearState: () => void;
  setAccessToken: (accessToken: string) => void;
  signUp: (
    userName: string,
    password: string,
    email: string,
    firstName: string,
    lastName: string,
  ) => Promise<void>;

  signIn: (username: string, password: string) => Promise<void>;
  signOut: () => Promise<void>;
  fetchMe: () => Promise<void>;
  refresh: () => Promise<void>;
}

export interface ThemeState {
  isDark: boolean;
  toggleTheme: () => void;
  setTheme: (dark: boolean) => void;
}

export interface ChatState {
  conversations: Conversation[];
  messages: Record<
    string,
    {
      items: Message[];
      hasMore: boolean; //infinite scroll
      nextCursor?: string | null; //phân trang
    }
  >;
  activeConversationId: string | null; //lưu hội thoại đang click vào
  convoLoading: boolean; //kiểm tra request đã load chưa
  messageLoading: boolean;
  reset: () => void; //reset state

  setActiveConversation: (id: string | null) => void;
  fetchConversations: () => Promise<void>;
  fetchMessages: (conversationId?: string) => Promise<void>;
  sendMessage: (
    content: string,
    receiverId?: string,
    imgUrl?: string,
  ) => Promise<void>;
}
