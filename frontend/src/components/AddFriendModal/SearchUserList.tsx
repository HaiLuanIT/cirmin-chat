import type { SearchUser, SearchUserResponse } from "@/types/user";
import React from "react";
import { Button } from "../ui/button";
import { Avatar, AvatarFallback, AvatarImage } from "../ui/avatar";
import { ChevronLast, ChevronLeft, ChevronRight } from "lucide-react";

interface SearchUserListProps {
  result: SearchUserResponse;
  loading: boolean;
  onSelectUser: (user: SearchUser) => void;
  onChangePage: (pageNumber: number) => void;
}
const SearchUserList = ({
  result,
  loading,
  onSelectUser,
  onChangePage,
}: SearchUserListProps) => {
  if (result?.items?.length === 0) return null;

  return (
    <div className="space-y-3">
      <div className="space-y-2 max-h-72 overflow-y-auto">
        {result?.items?.map((user) => (
          <Button
            key={user.id}
            type="button"
            variant="ghost"
            className="h-auto w-full justify-start gap-3 whitespace-normal border p-3"
            onClick={() => onSelectUser(user)}
          >
            {/* Avatar */}
            <Avatar className="shrink-0">
              <AvatarImage
                src={user.avatarUrl ?? undefined}
                alt={user.displayName}
              />
              <AvatarFallback>
                {user.displayName.charAt(0).toUpperCase()}
              </AvatarFallback>
            </Avatar>

            <div className="min-w-0 flex-1 text-left">
              <p className="truncate font-medium">{user?.displayName}</p>
              <p className="truncate text-sm text-muted-foreground">
                {user?.username}
              </p>
            </div>

            <span className="shrink-0 text-xs text-muted-foreground">
              {user.relationStatus}
            </span>
          </Button>
        ))}
      </div>
      {/* button */}
      <div className="flex items-center justify-between">
        <Button
          type="button"
          variant="outline"
          size="sm"
          disabled={loading || result.pageNumber <= 1}
          onClick={() => onChangePage(result.pageNumber - 1)}
        >
          <ChevronLeft className="size-4" />
          Trước
        </Button>
        <span className="text-sm text-muted-foreground">
          Trang {result.pageNumber}
        </span>
        <Button
          type="button"
          variant="outline"
          size="sm"
          disabled={loading || !result.hasNextPage}
          onClick={() => onChangePage(result.pageNumber + 1)}
        >
          Sau
          <ChevronRight className="size-4" />
        </Button>
      </div>
    </div>
  );
};

export default SearchUserList;
