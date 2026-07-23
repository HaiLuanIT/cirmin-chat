import { friendService } from "@/services/friendService";
import { useFriendStore } from "@/stores/useFriendStore";
import React from "react";
import FriendRequestItem from "./FriendRequestItem";
import { Button } from "../ui/button";
import { toast } from "sonner";

const ReceivedRequest = () => {
  const { acceptFriendRequest, rejectFriendRequest, loading, receivedList } =
    useFriendStore();

  if (!receivedList || receivedList.length === 0) {
    return (
      <p className="text-sm text-muted-foreground">
        Bạn chưa có lời mời kết bạn nào.
      </p>
    );
  }

  const handleAccept = async (requestId: string) => {
    try {
      await friendService.acceptFriendRequest(requestId);
      toast.success("Đã chấp nhận lời mời kết bạn thành công!");
    } catch (error) {
      console.error(error);
      toast.error("Lỗi khi chấp nhận lời mới kết bạn.");
    }
  };

  const handleReject = async (requestId: string) => {
    try {
      await friendService.rejectFriendRequest(requestId);
      toast.success("Đã từ chối lời mời kết bạn thành công!");
    } catch (error) {
      console.error(error);
      toast.error("Lỗi khi từ chối lời mới kết bạn.");
    }
  };
  return (
    <div className="space-y-3 mt-4">
      {receivedList.map((req) => (
        <FriendRequestItem
          key={req.id}
          requestInfo={req}
          action={
            <div className="flex gap-2">
              <Button
                size="sm"
                variant="primary"
                onClick={() => handleAccept(req.id)}
                disabled={loading}
              >
                Chấp nhận
              </Button>
              <Button
                size="sm"
                variant="destructiveOutline"
                onClick={() => handleReject(req.id)}
                disabled={loading}
              >
                Từ chối
              </Button>
            </div>
          }
          type="received"
        />
      ))}
    </div>
  );
};

export default ReceivedRequest;
