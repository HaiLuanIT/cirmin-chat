import type { Conversation, Message } from "./chat";
import type { Friend, FriendRequest, SearchUserResponse, User } from "./user";

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
  refresh: () => void;
  setUser: (user: User) => void;
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
  loading: boolean;
  reset: () => void; //reset state

  setActiveConversation: (id: string | null) => void;
  openConversation: (id: string) => Promise<void>;
  fetchConversations: () => Promise<void>;
  fetchMessages: (conversationId?: string) => Promise<void>;
  sendMessage: (content: string, imgUrl?: string) => Promise<void>;
  addMessage: (message: Message) => Promise<void>;
  updateLastMessage: (message: Message) => void;
  markAsSeen: (conversationId: string) => Promise<void>;
  incrementUnreadCount: (conversationId: string) => void;
  clearUnreadCount: (conversationId: string) => void;
  updateMemberSeenConcurrently: (
    userId: string,
    conversationId: string,
    lastMessageId: string,
  ) => void;
  addConversation: (conversation: Conversation) => void;
  createConversation: (name: string, userIds: string[]) => Promise<void>;
}

export interface PresenceState {
  onlineUsers: Set<string>;
  setOnlineUsers: (userIds: string[]) => void;
  setStatusUser: (userId: string, isOnline: boolean) => void;
  isOnline: (userId: string) => boolean;
  clearState: () => void;
}

export interface FriendState {
  loading: boolean;
  friends: Friend[];
  receivedList: FriendRequest[];
  sentList: FriendRequest[];
  searchByUsername: (
    username: string,
    limit?: number,
    pageNumber?: number,
  ) => Promise<SearchUserResponse>;
  addFriend: (to: string, message?: string) => Promise<string>;
  getAllFriendRequests: () => Promise<void>;
  acceptFriendRequest: (requestId: string) => Promise<void>;
  rejectFriendRequest: (requestId: string) => Promise<void>;
  getFriends: () => Promise<void>;
}

export interface UserState {
  uploadAvatarUrl: (formData: FormData) => Promise<void>;
  updateUserInfo: (
    displayName?: string,
    email?: string,
    bio?: string,
  ) => Promise<void>;
  changePassword: (oldPassword: string, newPassword: string) => Promise<void>;
}

export type SupportedLanguage = "vi" | "en";
export interface LocaleState {
  lang: SupportedLanguage;
  setLang: (lang: SupportedLanguage) => Promise<void>;
}
