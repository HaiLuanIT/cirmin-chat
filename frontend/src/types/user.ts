export interface User {
  id: string;
  userName: string;
  email: string;
  displayName: string;
  avatarUrl?: string | null;
  bio?: string;
  createdAt?: string;
  updatedAt?: string;
}
export interface Friend {
  userId: string;
  displayName: string;
  AvatarUrl?: string;
}

export interface FriendRequest {
  id: string;
  userId: string;
  displayName: string;
  avatarUrl?: string | null;
  status: string;
}
