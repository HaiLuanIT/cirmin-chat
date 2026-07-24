import { useFriendStore } from "@/stores/useFriendStore";
import React, { useState } from "react";
import {
  Dialog,
  DialogContent,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "../ui/dialog";
import { Button } from "../ui/button";
import { User, UserPlus, Users } from "lucide-react";
import { Label } from "../ui/label";
import { Input } from "../ui/input";
import type { Friend } from "@/types/user";
import InviteSuggestionList from "../newGroupChat/InviteSuggestionList";
import SelectedUsersList from "../newGroupChat/SelectedUsersList";
import { toast } from "sonner";
import { useChatStore } from "@/stores/useChatStore";

const NewGroupChatModal = () => {
  const [groupName, setGroupName] = useState("");
  const [search, setSearch] = useState("");
  const { friends, getFriends } = useFriendStore();
  const [invitedUsers, setInvitedUsers] = useState<Friend[]>([]);
  const { loading, createConversation } = useChatStore();

  const handleGetFriend = async () => {
    await getFriends();
  };

  const handleSelectFriend = (friend: Friend) => {
    setInvitedUsers([...invitedUsers, friend]);
    setSearch("");
  };

  const handleRemoveFriend = (user: Friend) => {
    setInvitedUsers(invitedUsers.filter((u) => u.userId !== user.userId));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    try {
      e.preventDefault();
      if (invitedUsers.length < 2) {
        toast.warning("Bạn phải mời ít nhất 1 thành viên vào nhóm");
        return;
      }
      await createConversation(
        groupName,
        invitedUsers.map((f) => f.userId),
      );
      setSearch("");
      setInvitedUsers([]);
    } catch (error) {
      console.error("Lỗi xảy ra khi tạo mới group chat", error);
      toast.error("Lỗi xảy ra khi tạo mới group chat");
    }
  };
  const filterFriends = friends.filter(
    (friend) =>
      friend.displayName.toLowerCase().includes(search.toLowerCase()) &&
      !invitedUsers.some((u) => u.userId === friend.userId),
  );
  return (
    <Dialog>
      <DialogTrigger>
        <Button
          variant="ghost"
          onClick={handleGetFriend}
          className="flex z-10 justify-center items-center size-5 rounded-full hover:bg-sidebar-accent transition cursor-pointer"
        >
          <Users className="size-4" />
          <span className="sr-only">Tạo nhóm</span>
        </Button>
      </DialogTrigger>
      <DialogContent className="sm:max-w-106.25 border-none">
        <DialogHeader>
          <DialogTitle className="capitalize">tạo nhóm chat mới</DialogTitle>
          <form className="space-y-4" onSubmit={handleSubmit}>
            {/* name group */}
            <div className="space-y-2">
              <Label htmlFor="groupName" className="text-sm font-semibold">
                Tên nhóm
              </Label>
              <Input
                id="groupName"
                placeholder="Gõ tên nhóm vào đây..."
                className="glass border-border/50 focus:border-primary/50 transition-smooth"
                value={groupName}
                onChange={(e) => setGroupName(e.target.value)}
                required
              ></Input>
            </div>
            {/* invite member */}
            <div className="space-y-2">
              <Label htmlFor="invite" className="text-sm font-semibold">
                Mời thành viên
              </Label>
              <Input
                id="invite"
                placeholder="Tìm theo tên hiển thị..."
                value={search}
                onChange={(e) => setSearch(e.target.value)}
              ></Input>

              {/* danh sách gợi ý */}
              {search && filterFriends.length > 0 && (
                <InviteSuggestionList
                  filteredFriends={filterFriends}
                  onSelect={handleSelectFriend}
                />
              )}

              {/* danh sách user đã select */}
              <SelectedUsersList
                invitedUsers={invitedUsers}
                onRemove={handleRemoveFriend}
              />
            </div>
            <DialogFooter>
              <Button
                type="submit"
                disabled={loading}
                className="flex-1 bg-gradient-chat text-white hover:opacity-90 transition-smooth"
              >
                {loading ? (
                  <span>Đang tạo...</span>
                ) : (
                  <>
                    <UserPlus className="size-4 mr-2" />
                    Tạo nhóm
                  </>
                )}
              </Button>
            </DialogFooter>
          </form>
        </DialogHeader>
      </DialogContent>
    </Dialog>
  );
};

export default NewGroupChatModal;
