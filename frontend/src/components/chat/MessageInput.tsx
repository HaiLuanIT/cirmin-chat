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
    <div className="flex items-center gap-2 p-3 min-h[56px] bg-background">
      <Button
        variant="ghost"
        size="icon"
        className="hover:bg-primary/10 transition-smooth"
      >
        <ImagePlus className="size-4" />
      </Button>
      <div className="flex-1 relative">
        <Input
          onKeyDown={handleKeyPress}
          value={value}
          onChange={(e) => setVallue(e.target.value)}
          placeholder={t("message.placeHolder")}
          className="pr-20 bg-white h-9 border-border/50 focus:border-primary/50 transition-smooth resize-none"
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
