import { chatService } from "@/services/chatService";
import type { ChatState } from "@/types/store";
import { create } from "zustand";
import { persist } from "zustand/middleware";
import { useAuthStore } from "./useAuthStore";

export const useChatStore = create<ChatState>()(
  persist(
    (set, get) => ({
      conversations: [],
      messages: {},
      activeConversationId: null,
      convoLoading: false,
      messageLoading: false,
      loading: false,

      setActiveConversation: (id) => set({ activeConversationId: id }),
      openConversation: async (id) => {
        const { messages, markAsSeen, fetchMessages } = get();

        set({ activeConversationId: id });
        await markAsSeen(id);

        if (!messages[id]) {
          await fetchMessages(id);
        }
      },
      reset: () => {
        set({
          conversations: [],
          messages: {},
          activeConversationId: null,
          convoLoading: false,
          messageLoading: false,
        });
      },
      fetchConversations: async () => {
        try {
          set({ convoLoading: true });
          const { conversations } = await chatService.fetchConversations();

          set({ conversations, convoLoading: false });
        } catch (error) {
          console.error("Lỗi xảy ra khi fetch conversations:", error);
          set({ convoLoading: false });
        }
      },

      fetchMessages: async (conversationId) => {
        const { activeConversationId, messages } = get();
        const { user } = useAuthStore.getState();

        const convoId = conversationId ?? activeConversationId;
        if (!convoId) return;
        const current = messages?.[convoId];
        const nextCursor =
          current?.nextCursor === undefined ? "" : current?.nextCursor;
        if (nextCursor === null) return;

        set({ messageLoading: true });
        try {
          const { messages: fetched, cursor } = await chatService.fetchMessage(
            convoId,
            nextCursor,
          );

          const processed = fetched.map((m) => ({
            ...m,
            isOwn: m.sender.senderId === user?.id,
          }));

          set((state) => {
            const prev = state.messages[convoId]?.items ?? [];

            const merged =
              processed.length > 0 ? [...prev, ...processed] : prev;

            console.log("message:", merged);
            return {
              messages: {
                ...state.messages,
                [convoId]: {
                  items: merged,
                  hasMore: !!cursor,
                  nextCursor: cursor ?? null,
                },
              },
            };
          });
        } catch (error) {
          console.error("Lỗi xảy ra khi  fetch messages:", error);
        } finally {
          set({ messageLoading: false });
        }
      },

      sendMessage: async (content, imgUrl) => {
        try {
          const { activeConversationId } = get();
          await chatService.sendMessage(
            content,
            imgUrl,
            activeConversationId || undefined,
          );

          set((state) => ({
            conversations: state.conversations.map((c) =>
              c.id === activeConversationId ? { ...c, seenBy: [] } : c,
            ),
          }));
        } catch (error) {
          console.error("Lỗi xảy ra khi gửi tin nhắn", error);
        }
      },
      addMessage: async (message) => {
        try {
          const { user } = useAuthStore.getState();
          const { fetchMessages } = get();
          message.isOwn = message.sender.senderId === user?.id;
          const convoId = message.conversationId;

          let prevItems = get().messages[convoId]?.items ?? [];

          if (prevItems.length === 0) {
            await fetchMessages(message.conversationId);
            prevItems = get().messages[convoId]?.items ?? [];
          }

          set((state) => {
            if (prevItems.some((m) => m.id === message.id)) {
              return state;
            }
            return {
              messages: {
                ...state.messages,
                [convoId]: {
                  items: [message, ...prevItems],
                  hasMore: state.messages[convoId].hasMore,
                  nextCursor: state.messages[convoId].nextCursor ?? undefined,
                },
              },
            };
          });
        } catch (error) {
          console.error("Lỗi xảy ra khi add mesage", error);
        }
      },
      updateLastMessage: (message) => {
        set((state) => {
          const updateConversations = state.conversations.map((c) => {
            if (c.id === message.conversationId) {
              return {
                ...c,
                lastMessage: {
                  id: message.id,
                  lastMessageContent: message.content,
                  lastMessageAt: message.sentAt,
                },
                members: c.members.map((m) =>
                  m.userId === message.sender.senderId
                    ? { ...m, lastMessageId: message.id }
                    : m,
                ),
              };
            }
            return c;
          });
          return {
            conversations: updateConversations,
          };
        });
      },
      markAsSeen: async (conversationId) => {
        try {
          await chatService.markAsSeenMessage(conversationId);
          set((state) => ({
            conversations: state.conversations.map((c) =>
              c.id === conversationId ? { ...c, unreadCount: 0 } : c,
            ),
          }));
        } catch (error) {
          console.error("Lỗi xảy ra khi đánh dấu tin nhắn đã xem!", error);
        }
      },
      incrementUnreadCount: (conversationId) => {
        set((state) => ({
          conversations: state.conversations.map((c) =>
            c.id === conversationId
              ? { ...c, unreadCount: (c?.unreadCount || 0) + 1 }
              : c,
          ),
        }));
      },
      clearUnreadCount: (conversationId) => {
        const { user } = useAuthStore.getState();
        set((state) => {
          const roomMessages = state.messages[conversationId]?.items ?? [];
          const latestMessage = roomMessages[0];
          const updatedConversations = state.conversations.map((c) => {
            if (c.id !== conversationId) return c;

            return {
              ...c,
              unreadCount: 0,
              members: c.members.map((member) =>
                member.userId === user.id
                  ? {
                      ...member,
                      lastMessageId: latestMessage?.id ?? member?.lastMessageId,
                    }
                  : member,
              ),
            };
          });

          return {
            conversations: updatedConversations,
          };
        });
      },
      updateMemberSeenConcurrently: (userId, conversationId, lastMessageId) => {
        set((state) => {
          const updatedConversations = state.conversations.map((c) => {
            if (c.id !== conversationId) return c;

            return {
              ...c,
              members: c.members.map((member) =>
                member.userId === userId
                  ? { ...member, lastMessageId: lastMessageId }
                  : member,
              ),
            };
          });

          return {
            conversations: updatedConversations,
          };
        });
      },
      addConversation: (conversation) => {
        set((state) => {
          const exist = state.conversations.some(
            (current) => current.id == conversation.id,
          );
          if (exist) return state;
          return {
            conversations: [conversation, ...state.conversations],
          };
        });
      },
      createConversation: async (name, userIds) => {
        try {
          set({ loading: true });
          await chatService.createConversation(name, userIds);
        } catch (error) {
          console.error("Lỗi xảy ra khi tạo mới group chat", error);
        } finally {
          set({ loading: false });
        }
      },
    }),
    {
      name: "chat-storage",
      partialize: (state) => ({
        conversations: state.conversations,
      }),
    },
  ),
);
