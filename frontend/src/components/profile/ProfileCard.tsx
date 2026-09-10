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
  const userIsOnline = isOnline(user.id);
  const displayedBio = user.bio || "Default bio ^.^";
  return (
    <Card className="min-h-64 overflow-hidden bg-gradient-primary p-0 sm:h-52 sm:min-h-0">
      <CardContent className="mt-10 flex flex-col items-center gap-4 pb-6 sm:mt-20 sm:flex-row sm:items-end sm:gap-6 sm:pb-8">
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
        <div className="min-w-0 text-center sm:text-left flex-1">
          <h1 className="text-2xl font-semibold tracking-tight text-white">
            {user.displayName}
          </h1>
          {displayedBio && (
            <p className="text-white/70 text-sm mt-2 max-w-lg line-clamp-2 wrap-anywhere">
              {displayedBio}
            </p>
          )}
        </div>
        {/* status */}
        <div>
          <Badge
            className={cn(
              "flex items-center gap-1 capitalize",
              userIsOnline
                ? "bg-green-100 text-green-700"
                : "bg-slate-100 text-slate-700",
            )}
          >
            <div
              className={cn(
                "size-2 rounded-full",
                userIsOnline ? "bg-green-500 animate-pulse" : "bg-slate-500",
              )}
            ></div>
            {userIsOnline ? "online" : "offline"}
          </Badge>
        </div>
      </CardContent>
    </Card>
  );
};

export default ProfileCard;
