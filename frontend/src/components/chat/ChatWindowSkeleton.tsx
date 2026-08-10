import { Skeleton } from "@/components/ui/skeleton";
import { SidebarInset } from "@/components/ui/sidebar";

const messageSkeletons = [
  { align: "start", width: "w-2/5", lines: ["w-full", "w-3/4"] },
  { align: "end", width: "w-1/3", lines: ["w-full"] },
  { align: "start", width: "w-1/2", lines: ["w-full", "w-2/3"] },
  { align: "end", width: "w-2/5", lines: ["w-full", "w-1/2"] },
  { align: "start", width: "w-1/4", lines: ["w-full"] },
] as const;

const ChatWindowSkeleton = () => {
  return (
    <SidebarInset
      aria-busy="true"
      aria-label="Loading conversation"
      className="flex h-full flex-1 flex-col overflow-hidden rounded-sm shadow-md"
    >
      <header className="flex items-center bg-background px-3 py-2">
        <div className="flex w-full items-center gap-2">
          <Skeleton className="size-7 shrink-0" />
          <Skeleton className="mx-2 h-4 w-px shrink-0 rounded-none" />
          <div className="flex w-full items-center gap-3 p-2">
            <Skeleton className="size-10 shrink-0 rounded-full" />
            <div className="flex flex-1 flex-col gap-2">
              <Skeleton className="h-4 w-32" />
              <Skeleton className="h-3 w-20" />
            </div>
          </div>
        </div>
      </header>

      <div className="flex flex-1 flex-col justify-end gap-5 overflow-hidden bg-primary-foreground p-4">
        {messageSkeletons.map((message, index) => (
          <div
            className={`flex items-end gap-2 ${
              message.align === "end" ? "flex-row-reverse" : ""
            }`}
            key={index}
          >
            {message.align === "start" && (
              <Skeleton className="size-8 shrink-0 rounded-full" />
            )}
            <div
              className={`flex max-w-[75%] flex-col gap-2 rounded-2xl bg-muted/70 p-3 ${message.width}`}
            >
              {message.lines.map((width, lineIndex) => (
                <Skeleton
                  className={`h-3 bg-muted-foreground/15 ${width}`}
                  key={lineIndex}
                />
              ))}
            </div>
          </div>
        ))}
      </div>

      <div className="flex min-h-16 items-center gap-2 bg-background p-3">
        <Skeleton className="size-9 shrink-0 rounded-md" />
        <Skeleton className="h-9 flex-1 rounded-md" />
        <Skeleton className="h-9 w-10 shrink-0 rounded-md" />
      </div>
    </SidebarInset>
  );
};

export default ChatWindowSkeleton;
