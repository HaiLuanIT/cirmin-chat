import { useFriendStore } from "@/stores/useFriendStore";
import { DialogContent, DialogHeader, DialogTitle } from "../ui/dialog";
import { MessageCircleMore, User } from "lucide-react";
import UserAvatar from "../chat/UserAvatar";

interface FriendListModalProps {
  loading: boolean;
  onSelectFriend: (friendId: string) => Promise<void>;
}

const FriendListModal = ({
  loading,
  onSelectFriend,
}: FriendListModalProps) => {
  const { friends } = useFriendStore();

  return (
    <DialogContent className="glass max-w-md">
      <DialogHeader>
        <DialogTitle className="flex items-center gap-2 capitalize text-xl">
          <MessageCircleMore />
          bắt đầu hội thoại mới
        </DialogTitle>
      </DialogHeader>

      {loading ? (
        <p className="py-8 text-center text-sm text-muted-foreground">
          Đang tải danh sách bạn bè...
        </p>
      ) : friends.length === 0 ? (
        <div className="py-8 text-center text-muted-foreground">
          <User className="mx-auto mb-3 size-12 opacity-50" />
          Chưa có bạn bè. Hãy kết bạn để bắt đầu chat.
        </div>
      ) : (
        <div className="space-y-4">
          <h2 className="mb-3 text-sm font-semibold uppercase tracking-wide text-muted-foreground">
            Danh sách bạn bè
          </h2>
          <div className="max-h-60 space-y-2 overflow-y-auto">
            {friends.map((friend) => (
              <button
                key={friend.userId}
                type="button"
                disabled={loading}
                className="glass group/friendCard flex w-full cursor-pointer items-center gap-3 rounded-xl p-3 text-left ring-1 ring-foreground/10 transition-smooth hover:bg-muted/30 hover:shadow-soft disabled:pointer-events-none disabled:opacity-50"
                onClick={() => void onSelectFriend(friend.userId)}
              >
                <div className="relative shrink-0">
                  <UserAvatar
                    type="sidebar"
                    name={friend.displayName}
                    avatarUrl={friend.avatarUrl}
                  />
                </div>
                {/* info */}
                <div className="flex-1 min-w-0 flex flex-col">
                  <h2 className="font-semibold text-sm truncate">
                    {friend.displayName}
                  </h2>
                  <span className="text-sm text-muted-foreground">
                    @{friend.username}
                  </span>
                </div>
              </button>
            ))}
          </div>
        </div>
      )}
    </DialogContent>
  );
};

export default FriendListModal;
