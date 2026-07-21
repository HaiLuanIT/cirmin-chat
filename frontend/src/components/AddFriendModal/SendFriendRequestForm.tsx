import React from "react";
import type { UseFormRegister, UseFormSetValue } from "react-hook-form";
import type { IFromValues } from "../chat/AddFriendModal";
import { Label } from "../ui/label";
import { Textarea } from "../ui/textarea";
import { DialogFooter } from "../ui/dialog";
import { Button } from "../ui/button";
import {
  UserPlus,
  ArrowLeft,
  Sparkles,
  MessageSquare,
  Loader2,
} from "lucide-react";
import { Avatar, AvatarFallback, AvatarImage } from "../ui/avatar";

interface SendFriendRequestProps {
  register: UseFormRegister<IFromValues>;
  setValue?: UseFormSetValue<IFromValues>;
  loading: boolean;
  searchedUsername: string;
  searchedDisplayName: string;
  searchedAvataUrl: string;
  onSubmit?: (e?: React.FormEvent<HTMLFormElement>) => void;
  onBack: () => void;
}

const PRESET_MESSAGES = [
  "Chào bạn! 👋",
  "Kết bạn với mình nhé! ✨",
  "Rất vui được làm quen! 😊",
];

const SendFriendRequestForm = ({
  register,
  setValue,
  loading,
  searchedUsername,
  searchedDisplayName,
  searchedAvataUrl,
  onSubmit,
  onBack,
}: SendFriendRequestProps) => {
  const handleSelectPreset = (text: string) => {
    if (setValue) {
      setValue("message", text, { shouldValidate: true });
    }
  };

  return (
    <form onSubmit={onSubmit} className="space-y-4">
      {/* Target User Found Card */}
      <div className="flex items-center gap-3 p-3.5 bg-gradient-primary rounded-2xl animate-in fade-in zoom-in-95 shadow-xs">
        <div className="flex items-center justify-center size-10 rounded-full text-white shrink-0 font-bold">
          <Avatar className="shrink-0">
            <AvatarImage
              src={searchedAvataUrl ?? undefined}
              alt={searchedDisplayName}
            />
            <AvatarFallback>
              {searchedDisplayName.charAt(0).toUpperCase()}
            </AvatarFallback>
          </Avatar>
        </div>
        <div className="min-w-0 flex-1">
          <div className="flex items-center gap-1.5">
            <span className="text-xs font-semibold text-white tracking-wider">
              {searchedDisplayName}
            </span>
          </div>
          <p className="text-sm font-bold truncate text-white">
            @{searchedUsername}
          </p>
        </div>
      </div>

      {/* Message Input Section */}
      <div className="space-y-2.5">
        <div className="flex items-center justify-between">
          <Label
            htmlFor="message"
            className="text-xs font-semibold text-muted-foreground uppercase tracking-wider flex items-center gap-1.5"
          >
            <MessageSquare className="size-3.5 text-primary" />
            Lời nhắn giới thiệu
          </Label>
        </div>

        <Textarea
          id="message"
          rows={3}
          placeholder="Chào bạn! Có thể kết bạn với mình được không?..."
          className="rounded-xl glass border-border/60 focus-visible:border-primary/60 focus-visible:ring-2 focus-visible:ring-primary/20 transition-all text-sm resize-none shadow-xs p-3"
          {...register("message")}
        />

        {/* Quick Presets */}
        {setValue && (
          <div className="space-y-1.5">
            <p className="text-[11px] text-muted-foreground/80 font-medium">
              Gợi ý câu chào nhanh:
            </p>
            <div className="flex flex-wrap gap-1.5">
              {PRESET_MESSAGES.map((msg) => (
                <button
                  key={msg}
                  type="button"
                  onClick={() => handleSelectPreset(msg)}
                  className="px-2.5 py-1 text-xs rounded-lg glass border-border/40 text-muted-foreground hover:text-foreground hover:border-primary/40 hover:bg-primary/5 transition-all cursor-pointer"
                >
                  {msg}
                </button>
              ))}
            </div>
          </div>
        )}
      </div>

      {/* Action Buttons */}
      <DialogFooter className="gap-2 sm:gap-2 sm:flex-row border-t-0 bg-transparent p-0 mx-0 mb-0 pt-1">
        <Button
          type="button"
          variant="outline"
          className="flex-1 rounded-xl h-10 border-border/60 hover:bg-muted/80 transition-all duration-200"
          onClick={onBack}
        >
          <ArrowLeft className="size-4 mr-1.5" />
          Quay lại
        </Button>

        <Button
          type="submit"
          disabled={loading}
          className="flex-1 rounded-xl h-10 bg-gradient-chat text-white font-medium shadow-sm hover:shadow-glow hover:scale-[1.01] active:scale-[0.98] transition-all duration-200 disabled:opacity-50 disabled:hover:scale-100 disabled:hover:shadow-none"
        >
          {loading ? (
            <div className="flex items-center justify-center gap-2">
              <Loader2 className="size-4 animate-spin" />
              <span>Đang gửi...</span>
            </div>
          ) : (
            <div className="flex items-center justify-center gap-2">
              <UserPlus className="size-4" />
              <span>Kết bạn</span>
            </div>
          )}
        </Button>
      </DialogFooter>
    </form>
  );
};

export default SendFriendRequestForm;
