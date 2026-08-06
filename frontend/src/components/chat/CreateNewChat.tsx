import { useFriendStore } from "@/stores/useFriendStore";
import { useState } from "react";
import { Dialog, DialogTrigger } from "../ui/dialog";
import { MessageCircle } from "lucide-react";
import FriendListModal from "../createNewChat/FriendListModal";
import { useChatStore } from "@/stores/useChatStore";
import { toast } from "sonner";
import { useTranslation } from "react-i18next";

const CreateNewChat = () => {
  const [open, setOpen] = useState(false);
  const [preparing, setPreparing] = useState(false);
  const { getFriends } = useFriendStore();
  const { conversations, fetchConversations, openConversation } =
    useChatStore();
  const { t } = useTranslation("common");

  const handleOpenChange = (nextOpen: boolean) => {
    setOpen(nextOpen);

    if (nextOpen) {
      setPreparing(true);
      void Promise.all([getFriends(), fetchConversations()]).finally(() => {
        setPreparing(false);
      });
    }
  };

  const handleSelectFriend = async (friendId: string) => {
    const directConversation = conversations.find(
      (conversation) =>
        !conversation.isGroup &&
        conversation.members.some((member) => member.userId === friendId),
    );

    if (!directConversation) {
      toast.error("Không tìm thấy cuộc trò chuyện với người bạn này.");
      return;
    }

    await openConversation(directConversation.id);
    setOpen(false);
  };

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogTrigger asChild>
        <button
          type="button"
          className="glass group/card flex w-full cursor-pointer items-center gap-4 rounded-xl p-3 text-left ring-1 ring-foreground/10 transition-smooth hover:shadow-soft"
        >
          <div className="flex size-8 items-center justify-center rounded-full bg-gradient-chat transition-bounce group-hover/card:scale-110">
            <MessageCircle className="size-4 text-white" />
          </div>
          <span className="text-sm font-medium capitalize">
            {t("sidebar.newMessage.label")}
          </span>
        </button>
      </DialogTrigger>

      <FriendListModal
        loading={preparing}
        onSelectFriend={handleSelectFriend}
      />
    </Dialog>
  );
};

export default CreateNewChat;
