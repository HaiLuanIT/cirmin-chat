import React from "react";
import { Card } from "../ui/card";
import { formatOnlineTime, cn } from "@/lib/utils";
import { MoreHorizontal } from "lucide-react";

interface ChatCardProps {
  convoId: string;
  name: string;
  timestamp?: Date;
  isActive: boolean;
  onSelect: (id: string) => void;
  unreadCount?: number;
  leftSection: React.ReactNode;
  subtile: React.ReactNode;
}
const ChatCard = ({
  convoId,
  name,
  timestamp,
  isActive,
  onSelect,
  unreadCount,
  leftSection,
  subtile,
}: ChatCardProps) => {
  return (
    <Card
      key={convoId}
      className={cn(
        "border-none p-3 cursor-pointer transition-smooth glass hover:bg-muted/30",
        isActive &&
          "chat-card-active ring-2 ring-primary/50 dark:ring-primary/60",
      )}
      onClick={() => onSelect(convoId)}
    >
      <div className="flex items-center gap-3">
        <div className="relative">{leftSection}</div>
        <div className="flex-1 min-w-0">
          <div className="flex items-center justify-between mb-1">
            <h3
              className={cn(
                "font-semibold text-sm truncate",
                unreadCount && unreadCount > 0 && "text-foreground",
              )}
            >
              {name}
            </h3>
            <span className="text-xs text-muted-foreground">
              {timestamp ? formatOnlineTime(timestamp) : ""}
            </span>
          </div>
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-1 flex-1 min-w-0">
              {subtile}
            </div>
            <MoreHorizontal className="size-4 text-muted-foreground opacity-0 group-hover:opacity-100 hover:size-5 transition-smooth" />
          </div>
        </div>
      </div>
    </Card>
  );
};

export default ChatCard;
