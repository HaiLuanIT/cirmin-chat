import { useUserStore } from "@/stores/useUserStore";
import React, { useRef } from "react";
import { Button } from "../ui/button";
import { Camera } from "lucide-react";
import { toast } from "sonner";
import { getApiErrorMessage } from "@/lib/api-error";

const AvatarUploader = () => {
  const fileInputRef = useRef<HTMLInputElement>(null);
  const { uploadAvatarUrl } = useUserStore();

  const handleClick = () => {
    fileInputRef.current?.click();
  };
  const handleUpload = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;

    const formData = new FormData();
    formData.append("image", file);
    try {
      await uploadAvatarUrl(formData);
    } catch (error) {
      toast.error(getApiErrorMessage(error));
    } finally {
      e.target.value = "";
    }
  };
  return (
    <>
      <Button
        size="icon"
        variant="secondary"
        onClick={handleClick}
        className="absolute -bottom-2 -right-2 rounded-full shadow-md hover:scale-115 transition duration-300 hover:bg-background"
      >
        <Camera className="size-4" />
      </Button>
      <input type="file" hidden ref={fileInputRef} onChange={handleUpload} />
    </>
  );
};

export default AvatarUploader;
