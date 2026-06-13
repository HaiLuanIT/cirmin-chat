import React from "react";
import SignOut from "../components/auth/signout";
import { useAuthStore } from "../stores/useAuthStore";
import { Button } from "../components/ui/button";
import api from "../lib/axios";
import { toast } from "sonner";

const ChatAppPage = () => {
  const user = useAuthStore((x) => x.user);

  const handleTest = async () => {
    try {
      await api.get("/auth/test", { withCredentials: true });
      toast.success("Thành công");
    } catch (error) {
      toast.error("Thất bại");
      console.error(error);
    }
  };
  return (
    <div>
      {user?.userName}
      <SignOut />
      <Button onClick={handleTest}>Test</Button>
    </div>
  );
};

export default ChatAppPage;
