import { friendService } from "@/services/friendService";
import { useFriendStore } from "@/stores/useFriendStore";
import React from "react";
import FriendRequestItem from "./FriendRequestItem";
import { Button } from "../ui/button";
import { toast } from "sonner";
import { getApiErrorMessage } from "@/lib/api-error";
import { useTranslation } from "react-i18next";

const ReceivedRequest = () => {
  const { acceptFriendRequest, rejectFriendRequest, loading, receivedList } =
    useFriendStore();
  const { t } = useTranslation("friends");

  if (!receivedList || receivedList.length === 0) {
    return (
      <p className="text-sm text-muted-foreground">
        {t("friendRequest.dialog.received.empty")}
      </p>
    );
  }

  const handleAccept = async (requestId: string) => {
    try {
      await acceptFriendRequest(requestId);
      toast.success(t("friendRequest.toasts.accept"));
    } catch (error) {
      toast.error(getApiErrorMessage(error));
    }
  };

  const handleReject = async (requestId: string) => {
    try {
      await rejectFriendRequest(requestId);
      toast.success(t("friendRequest.toasts.reject"));
    } catch (error) {
      console.error(error);
      toast.error(getApiErrorMessage(error));
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
                {t("friendRequest.actions.accept")}
              </Button>
              <Button
                size="sm"
                variant="destructiveOutline"
                onClick={() => handleReject(req.id)}
                disabled={loading}
              >
                {t("friendRequest.actions.reject")}
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
