import { useState } from "react";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "../ui/dialog";
import { UserPlus } from "lucide-react";
import type { SearchUser, SearchUserResponse } from "@/types/user";
import { useFriendStore } from "@/stores/useFriendStore";
import { useForm } from "react-hook-form";
import { toast } from "sonner";
import { useTranslation } from "react-i18next";
import {
  getApiErrorMessage,
  getApiProblemDetails,
  translateApiErrorCode,
} from "@/lib/api-error";
import SearchForm from "../AddFriendModal/SearchForm";
import SearchUserList from "../AddFriendModal/SearchUserList";
import SendFriendRequestForm from "../AddFriendModal/SendFriendRequestForm";

export interface IFromValues {
  username: string;
  message: string;
}
const LIMIT = 10;
const AddFriendModal = () => {
  const [searchResult, setSearchResult] = useState<SearchUserResponse | null>(
    null,
  );
  const { t } = useTranslation("common");

  const [selectedUser, setSelectedUser] = useState<SearchUser | null>(null);

  const [searchedUsername, setSearchedUsername] = useState("");
  const { loading, searchByUsername, addFriend } = useFriendStore();

  const {
    register,
    handleSubmit,
    watch,
    reset,
    setValue,
    setError,
    clearErrors,
    formState: { errors },
  } = useForm<IFromValues>({
    defaultValues: { username: "", message: "" },
  });

  const usernameValue = watch("username");

  const isFound =
    searchResult === null ? null : searchResult?.items?.length > 0;

  const search = async (username: string, pageNumber: number) => {
    try {
      const result = await searchByUsername(username, LIMIT, pageNumber);

      setSearchResult(result);
      setSearchedUsername(username);
    } catch (error) {
      console.error("Không thể tìm người dùng", error);
      setSearchResult(null);
      toast.error(getApiErrorMessage(error));
    }
  };
  const handleSearch = handleSubmit(async (data) => {
    const username = data.username.trim();
    if (!username) return;

    setSelectedUser(null);

    await search(username, 1);
  });

  const handleChangePage = async (pageNumber: number) => {
    if (!searchedUsername || pageNumber < 1) return;

    await search(searchedUsername, pageNumber);
  };

  const handleSelectUser = (user: SearchUser) => {
    setSelectedUser(user);
  };

  const handleSend = handleSubmit(async (data) => {
    if (!selectedUser) return;
    clearErrors();

    try {
      const message = await addFriend(selectedUser.id, data.message.trim());
      toast.success(message);

      handleCancel();
    } catch (error) {
      const problem = getApiProblemDetails(error);

      if (!problem) {
        setError("root.server", {
          message: translateApiErrorCode("SYSTEM.INTERNAL_ERROR"),
        });
      }

      if (problem?.errors) {
        for (const [field, fieldErrors] of Object.entries(problem?.errors)) {
          const firstError = fieldErrors[0];

          if (!firstError) {
            continue;
          }

          if (field === "message") {
            setError(field, {
              type: "server",
              message: translateApiErrorCode(
                firstError.code,
                firstError.params,
              ),
            });
          }
        }
        return;
      }

      setError("root.server", {
        type: "server",
        message: translateApiErrorCode(problem?.code, problem?.params),
      });
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
          <span className="sr-only">{t("sidebar.directs.form.title")}</span>
        </div>
      </DialogTrigger>
      <DialogContent className="sm:max-w-106.25 border-none">
        <DialogHeader>
          <DialogTitle>{t("sidebar.directs.form.title")}</DialogTitle>
        </DialogHeader>

        {!selectedUser && (
          <>
            <SearchForm
              register={register}
              setValue={setValue}
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
            setValue={setValue}
            loading={loading}
            searchedUsername={selectedUser.username}
            searchedDisplayName={selectedUser.displayName}
            searchedAvataUrl={selectedUser.avatarUrl}
            onSubmit={handleSend}
            onBack={handleBackToResults}
            errors={errors}
          />
        )}
      </DialogContent>
    </Dialog>
  );
};

export default AddFriendModal;
