import type { User } from "@/types/user";
import { Card, CardContent } from "../ui/card";
import UserAvatar from "../chat/UserAvatar";
import { Badge } from "../ui/badge";
import { cn } from "@/lib/utils";
import { usePresenceStore } from "@/stores/usePresenceStore";
import AvatarUploader from "./AvatarUploader";

interface ProfileCardProps {
  user: User | null;
}
const ProfileCard = ({ user }: ProfileCardProps) => {
  const { isOnline } = usePresenceStore();
  if (!user) return;
  // Không mutate object user từ store ngay trong quá trình render.
  const displayedBio = user.bio || "Default bio ^.^";
  return (
    <Card className="overflow-hidden p-0 h-52 bg-gradient-to-r from-indigo-500 via-purple-500 to-pink-500">
      <CardContent className="mt-20 pb-8 flex flex-col sm:flex-row items-center sm:items-end gap-6">
        <div className="relative">
          <UserAvatar
            type="profile"
            name={user.displayName}
            avatarUrl={user.avatarUrl ?? undefined}
            className="ring-4 ring-white shadow-lg"
          />

          <AvatarUploader />
        </div>
        {/* user info */}
        <div className="text-center sm:text-left flex-1">
          <h1 className="text-2xl font-semibold tracking-tight text-white">
            {user.displayName}
          </h1>
          {displayedBio && (
            <p className="text-white/70 text-sm mt-2 maw-w-lg line-clamp-2">
              {displayedBio}
            </p>
          )}
        </div>
        {/* status */}
        <div>
          <Badge
            className={cn(
              "flex items-center gap-1 capitalize",
              isOnline
                ? "bg-green-100 text-green-700"
                : "bg-slate-100 text-slate-700",
            )}
          >
            <div
              className={cn(
                "size-2 rounded-full",
                isOnline ? "bg-green-500 animate-pulse" : "bg-slate-500",
              )}
            ></div>
            {isOnline ? "online" : "offline"}
          </Badge>
        </div>
      </CardContent>
    </Card>
  );
};

export default ProfileCard;
