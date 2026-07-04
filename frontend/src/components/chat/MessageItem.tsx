import { cn, formatMessageTime } from "@/lib/utils";
import type { Conversation, ConversationMember, Message } from "@/types/chat";
import React from "react";
import UserAvatar from "./UserAvatar";
import { Card } from "../ui/card";
import { Badge } from "../ui/badge";

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
  const prev = messages[index - 1];
  const isGroupBreak =
    index === 0 ||
    message.sender.senderId !== prev?.sender.senderId ||
    new Date(message.sentAt).getTime() - new Date(prev?.sentAt || 0).getTime() >
      300000; //5 phut

  const participant = selectedConvo.members.find(
    (p: ConversationMember) =>
      p.userId.toString() === message.sender.senderId.toString(),
  );
  return (
    <div
      className={cn(
        "flex gap-2 message-bounce mt-2",
        message.isOwn ? "justify-end" : "justify-start",
      )}
    >
      {/* avatar */}
      {!message.isOwn && (
        <div className="w-8">
          {isGroupBreak && (
            <UserAvatar
              type="chat"
              name={participant.displayName ?? "Moji"}
              avatarUrl={participant?.avatarUrl ?? undefined}
            />
          )}
        </div>
      )}
      {/* message */}
      <div
        className={cn(
          "max-w-xs lg:max-w-md space-y-1 flex flex-col",
          message.isOwn ? "items-end" : "items-start",
        )}
      >
        <Card
          className={cn(
            "p-3",
            message.isOwn
              ? "chat-bubble-sent border-0"
              : "bg-chat-bubble-received",
          )}
        >
          <p className="text-sm leading-relaxed break-words">
            {message.content}
          </p>

          {/* time */}
          {isGroupBreak && (
            <span
              className={cn(
                "text-xs px-1",
                message.isOwn ? "text-muted" : "text-muted-foreground",
              )}
            >
              {formatMessageTime(new Date(message.sentAt))}
            </span>
          )}
        </Card>
        {/* seen/deliverd */}
        {message.isOwn && message.id === selectedConvo.lastMessage?.id && (
          <Badge
            variant="outline"
            className={cn(
              "text-xs px-1.5 py-0.5 h-4 border-0",
              lastMessageStatus === "seen"
                ? "bg-primary/20 text-primary"
                : "bg-muted text-muted-foreground",
            )}
          >
            {lastMessageStatus}
          </Badge>
        )}
      </div>
    </div>
  );
};

export default MessageItem;
