import { cn, formatMessageTime } from "@/lib/utils";
import type { Conversation, ConversationMember, Message } from "@/types/chat";
import UserAvatar from "./UserAvatar";
import { Card } from "../ui/card";
import { Badge } from "../ui/badge";
import { useAuthStore } from "@/stores/useAuthStore";
import { useTranslation } from "react-i18next";

interface MessageItemProps {
  message: Message;
  index: number;
  messages: Message[];
  selectedConvo: Conversation;
  lastMessageStatus: "delivered" | "seen";
}
const MessageItem = ({
  message,
  index,
  messages,
  selectedConvo,
  lastMessageStatus,
}: MessageItemProps) => {
  const { user } = useAuthStore();
  const { t } = useTranslation("chat");
  const prev = index + 1 < messages.length ? messages[index + 1] : undefined;
  const isShowTime =
    index === 0 ||
    new Date(message.sentAt).getTime() - new Date(prev?.sentAt || 0).getTime() >
      300000; //5 phut

  const isGroupBreak =
    isShowTime || message.sender.senderId !== prev?.sender.senderId;

  const participant = selectedConvo.members.find(
    (p: ConversationMember) =>
      p.userId.toString() === message.sender.senderId.toString(),
  );

  const seenByUsers = selectedConvo.members.filter(
    (member) =>
      member.userId !== user?.id &&
      member.lastMessageId.toString() === message.id.toString(),
  );

  lastMessageStatus = seenByUsers.length > 0 ? "seen" : "delivered";
  return (
    <>
      {/* time */}
      {isShowTime && (
        <div className="flex justify-center w-full my-3">
          <span className="text-xs px-1 text-muted-foreground">
            {formatMessageTime(new Date(message.sentAt))}
          </span>
        </div>
      )}

      <div
        className={cn(
          "flex min-w-0 gap-2 message-bounce mt-1",
          message.isOwn ? "justify-end" : "justify-start",
        )}
      >
        {/* avatar */}
        {!message.isOwn && (
          <div className="w-8 shrink-0">
            {isGroupBreak && (
              <UserAvatar
                type="chat"
                name={participant?.displayName ?? "CirMin"}
                avatarUrl={participant?.avatarUrl ?? undefined}
              />
            )}
          </div>
        )}
        {/* message */}
        <div
          className={cn(
            "flex max-w-[calc(100%-2.5rem)] min-w-0 flex-col space-y-1 sm:max-w-xs lg:max-w-md",
            message.isOwn ? "items-end" : "items-start",
          )}
        >
          <Card
            className={cn(
              "p-3",
              message.isOwn
                ? "chat-bubble-sent border-0"
                : "chat-bubble-received",
            )}
          >
            <p className="break-words text-sm leading-relaxed [overflow-wrap:anywhere]">
              {message.content}
            </p>
          </Card>
          {/* seen/delivered */}
          {!selectedConvo.isGroup
            ? message.isOwn &&
              message.id === selectedConvo.lastMessage?.id && (
                <Badge
                  variant="outline"
                  className={cn(
                    "text-xs px-1.5 py-0.5 h-4 border-0",
                    lastMessageStatus === "seen"
                      ? "bg-primary/20 text-primary"
                      : "bg-muted text-muted-foreground",
                  )}
                >
                  {t(`message.status.${lastMessageStatus}`)}
                </Badge>
              )
            : message.isOwn &&
              (seenByUsers.length > 0 ? (
                <div className="flex items-center gap-1 mt-1 px-1">
                  {seenByUsers.map((user) => (
                    <div
                      key={user.userId}
                      title={t("message.status.seenBy", {
                        name: user.displayName,
                      })}
                    >
                      <UserAvatar
                        type="chat"
                        name={user.displayName ?? "CirMin"}
                        avatarUrl={user?.avatarUrl ?? undefined}
                      />
                    </div>
                  ))}
                </div>
              ) : (
                message.id.toString() ===
                  selectedConvo?.lastMessage?.id?.toString() && (
                  <Badge
                    variant="outline"
                    className="text-xs px-1.5 py-0.5 h-4 border-0 select-none bg-muted text-muted-foreground transition-all duration-200"
                  >
                    {t("message.status.delivered")}
                  </Badge>
                )
              ))}
        </div>
      </div>
    </>
  );
};

export default MessageItem;
