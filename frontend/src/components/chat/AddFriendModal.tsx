import React, { useState } from "react";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "../ui/dialog";
import { UserPlus } from "lucide-react";
import type { SearchUser, SearchUserResponse, User } from "@/types/user";
import { useFriendStore } from "@/stores/useFriendStore";
import { useForm } from "react-hook-form";
import { toast } from "sonner";
import SearchForm from "../AddFriendModal/SearchForm";
import SendFriendRequestForm from "../AddFriendModal/SendFriendRequestForm";
import SearchUserList from "../AddFriendModal/SearchUserList";

export interface IFromValues {
  username: string;
  message: string;
}
const LIMIT = 10;
const AddFriendModal = () => {
  const [searchResult, setSearchResult] = useState<SearchUserResponse | null>(
    null,
  );

  const [selectedUser, setSelectedUser] = useState<SearchUser | null>(null);

  const [searchedUsername, setSearchedUsername] = useState("");
  const { loading, searchByUsername, addFriend } = useFriendStore();

  const {
    register,
    handleSubmit,
    watch,
    reset,
    formState: { errors },
  } = useForm<IFromValues>({
    defaultValues: { username: "", message: "" },
  });

  const usernameValue = watch("username");

  const isFound = searchResult === null ? null : searchResult.items.length > 0;

  const search = async (username: string, pageNumber: number) => {
    try {
      const result = await searchByUsername(username, LIMIT, pageNumber);

      setSearchResult(result);
      setSearchedUsername(username);
    } catch (error) {
      console.error("Không thể tìm người dùng", error);
      setSearchResult(null);
      toast.error("Không thể tìm người dùng. Hãy thử lại");
    }
  };
  const handleSearch = handleSubmit(async (data) => {
    const username = data.username.trim();
    if (!username) return;

    setSelectedUser(null);

    await search(username, 1);
  });

  const handleChangePage = async (pageNumber: number) => {
    if (!searchedUsername && pageNumber < 1) return;

    await search(searchedUsername, pageNumber);
  };

  const handleSelectUser = (user: SearchUser) => {
    setSelectedUser(user);
  };

  const handleSend = handleSubmit(async (data) => {
    if (!selectedUser) return;

    try {
      const message = await addFriend(selectedUser.id, data.message.trim());
      toast.success(message);

      handleCancel();
    } catch (error) {
      console.error("Lỗi xảy ra khi gửi request từ form", error);
    }
  });

  const handleBackToResults = () => {
    setSelectedUser(null);
  };

  const handleCancel = () => {
    reset();
    setSearchedUsername("");
    setSelectedUser(null);
    setSearchResult(null);
  };
  return (
    <Dialog>
      <DialogTrigger asChild>
        <div className="flex justify-center items-center size-5 rounded-full hover:bg-sidebar-accent cursor-pointer z-10">
          <UserPlus className="size-4" />
          <span className="sr-only">Kết bạn</span>
        </div>
      </DialogTrigger>
      <DialogContent className="sm:max-w-[425px] border-none">
        <DialogHeader>
          <DialogTitle>Kết bạn</DialogTitle>
        </DialogHeader>

        {!selectedUser && (
          <>
            <SearchForm
              register={register}
              errors={errors}
              usernameValue={usernameValue}
              loading={loading}
              isFound={isFound}
              searchedUsername={searchedUsername}
              onSubmit={handleSearch}
              onCancel={handleCancel}
            />
            {searchResult && (
              <SearchUserList
                result={searchResult}
                loading={loading}
                onChangePage={handleChangePage}
                onSelectUser={handleSelectUser}
              />
            )}
          </>
        )}

        {selectedUser && (
          <SendFriendRequestForm
            register={register}
            loading={loading}
            searchedUsername={selectedUser.username}
            onSubmit={handleSend}
            onBack={handleBackToResults}
          />
        )}
      </DialogContent>
    </Dialog>
  );
};

export default AddFriendModal;
