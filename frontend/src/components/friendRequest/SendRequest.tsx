import { useFriendStore } from "@/stores/useFriendStore";
import React from "react";
import FriendRequestItem from "./FriendRequestItem";

const SendRequest = () => {
  const { sentList } = useFriendStore();

  if (!sentList || sentList.length === 0) {
    return (
      <p className="text-sm text-muted-foreground">
        Bạn chưa gửi lời mời kết bạn nào.
      </p>
    );
  }
  return (
    <div className="space-y-3 mt-4">
      <>
        {sentList.map((req) => (
          <FriendRequestItem
            key={req.id}
            requestInfo={req}
            type="sent"
            action={
              <p className="text-muted-foreground text-sm">
                Đang chờ trả lời...
              </p>
            }
          />
        ))}
      </>
    </div>
  );
};

export default SendRequest;
