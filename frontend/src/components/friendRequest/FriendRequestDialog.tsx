import { useFriendStore } from "@/stores/useFriendStore";
import { useEffect, useState, type Dispatch, type SetStateAction } from "react";
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "../ui/dialog";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "../ui/tabs";
import SendRequest from "./SendRequest";
import ReceivedRequest from "./ReceivedRequest";
import { useTranslation } from "react-i18next";

interface FriendRequestDialogProps {
  open: boolean;
  setOpen: Dispatch<SetStateAction<boolean>>;
}
const FriendRequestDialog = ({ open, setOpen }: FriendRequestDialogProps) => {
  const [tab, setTab] = useState("received");
  const { getAllFriendRequests } = useFriendStore();
  const { t } = useTranslation("friends");

  useEffect(() => {
    const loadRequest = async () => {
      try {
        await getAllFriendRequests();
      } catch (error) {
        console.error("Lỗi xảy ra khi loadRequest", error);
      }
    };
    loadRequest();
  }, []);
  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogContent className="sm:max-w-lg">
        <DialogHeader>
          <DialogTitle>{t("friendRequest.dialog.title")}</DialogTitle>
        </DialogHeader>
        <Tabs
          value={tab}
          onValueChange={setTab}
          className="flex w-full flex-col"
        >
          <TabsList className="grid w-full grid-cols-2">
            <TabsTrigger value="received">
              {t("friendRequest.dialog.received.title")}
            </TabsTrigger>
            <TabsTrigger value="sent">
              {t("friendRequest.dialog.sent.title")}
            </TabsTrigger>
          </TabsList>
          <TabsContent value="received" className="w-full">
            <ReceivedRequest />
          </TabsContent>
          <TabsContent value="sent" className="w-full">
            <SendRequest />
          </TabsContent>
        </Tabs>
      </DialogContent>
    </Dialog>
  );
};

export default FriendRequestDialog;
