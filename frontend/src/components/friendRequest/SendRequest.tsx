import { useFriendStore } from "@/stores/useFriendStore";
import FriendRequestItem from "./FriendRequestItem";
import { useTranslation } from "react-i18next";

const SendRequest = () => {
  const { sentList } = useFriendStore();
  const { t } = useTranslation("friends");

  if (!sentList || sentList.length === 0) {
    return (
      <p className="text-sm text-muted-foreground">
        {t("friendRequest.dialog.sent.empty")}
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
                {t("friendRequest.dialog.sent.status.waiting")}
              </p>
            }
          />
        ))}
      </>
    </div>
  );
};

export default SendRequest;
