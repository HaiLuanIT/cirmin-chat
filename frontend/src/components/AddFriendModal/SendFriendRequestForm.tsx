import React from "react";
import type { UseFormRegister } from "react-hook-form";
import type { IFromValues } from "../chat/AddFriendModal";
import { Label } from "../ui/label";
import { Textarea } from "../ui/textarea";
import { DialogFooter } from "../ui/dialog";
import { Button } from "../ui/button";
import { UserPlus } from "lucide-react";

interface SendFriendRequestProps {
  register: UseFormRegister<IFromValues>;
  loading: boolean;
  searchedUsername: string;
  onSubmit?: (e?: React.FormEvent<HTMLFormElement>) => void;
  onBack: () => void;
}
const SendFriendRequestForm = ({
  register,
  loading,
  searchedUsername,
  onSubmit,
  onBack,
}: SendFriendRequestProps) => {
  return (
    <form onSubmit={onSubmit} className="space-y-4">
      <div>
        <span className="success-message">
          Tìm thấy <span className="font-semibold">@{searchedUsername}</span>{" "}
          rồi nè 🎉
        </span>

        <div className="space-y-2">
          <Label htmlFor="message" className="text-sm font-semibold">
            Giới thiệu
          </Label>

          <Textarea
            id="message"
            rows={3}
            placeholder="Chào bạn! Có thể kết bạn được không?..."
            className="glass border-border/50 focus:border-primary/50 transition-smooth resize-none"
            {...register("message")}
          />
        </div>

        <DialogFooter>
          <Button
            type="button"
            variant="outline"
            className="flex-1 glass  hover:text-destructive"
            onClick={onBack}
          >
            Quay lại
          </Button>

          <Button
            type="submit"
            disabled={loading}
            className="flex-1 bg-gradient-chat hover:opacity-90 text-white transition-smooth"
          >
            {loading ? (
              <span>Đang gửi</span>
            ) : (
              <>
                <UserPlus className="size-4 mr-2" /> Kết bạn
              </>
            )}
          </Button>
        </DialogFooter>
      </div>
    </form>
  );
};

export default SendFriendRequestForm;
