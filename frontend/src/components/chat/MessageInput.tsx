import { useAuthStore } from "@/stores/useAuthStore";
import React, { useState } from "react";
import { Button } from "../ui/button";
import { ImagePlus, Send } from "lucide-react";
import { Input } from "../ui/input";
import EmojiPicker from "./EmojiPicker";
import { useChatStore } from "@/stores/useChatStore";
import { toast } from "sonner";
import { getApiErrorMessage } from "@/lib/api-error";
import { useTranslation } from "react-i18next";

const MessageInput = () => {
  const { user } = useAuthStore();
  const { t } = useTranslation("chat");

  const [value, setVallue] = useState("");

  const { sendMessage } = useChatStore();

  if (!user) return;

  const onSendMessage = async () => {
    if (!value.trim()) return;
    const currentValue = value;
    setVallue("");

    try {
      await sendMessage(currentValue);
    } catch (error) {
      console.error(error);
      toast.error(getApiErrorMessage(error));
    }
  };

  const handleKeyPress = (e: React.KeyboardEvent) => {
    if (e.key === "Enter") {
      e.preventDefault();
      onSendMessage();
    }
  };
  return (
    <div className="flex min-h-14 shrink-0 items-center gap-1.5 bg-background px-2 py-2 pb-[max(0.5rem,env(safe-area-inset-bottom))] sm:gap-2 sm:p-3">
      <Button
        variant="ghost"
        size="icon"
        aria-label="Attach image"
        className="shrink-0 hover:bg-primary/10 transition-smooth"
      >
        <ImagePlus className="size-4" />
      </Button>
      <div className="relative min-w-0 flex-1">
        <Input
          onKeyDown={handleKeyPress}
          value={value}
          onChange={(e) => setVallue(e.target.value)}
          placeholder={t("message.placeHolder")}
          className="pr-20 bg-card text-card-foreground h-9 border-border/50 focus:border-primary/50 transition-smooth resize-none"
        ></Input>
        <div className="absolute right-2 top-1/2 transform -translate-y-1/2 flex items-center gap-1">
          <Button
            asChild
            variant="ghost"
            size="icon"
            className="size-8 hover:bg-primary/10 transition-smooth"
          >
            <div>
              <EmojiPicker
                onChange={(emoji: string) => setVallue(`${value}${emoji}`)}
              />
            </div>
          </Button>
        </div>
      </div>
      <Button
        size="icon"
        aria-label={t("message.send", { defaultValue: "Send message" })}
        className="bg-gradient-chat hover:shadow-glow transition-smooth hover:scale-105"
        onClick={onSendMessage}
        disabled={!value.trim() === true}
      >
        <Send className="size-4 text-white" />
      </Button>
    </div>
  );
};

export default MessageInput;
