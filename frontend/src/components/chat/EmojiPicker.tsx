import { useThemeStore } from "@/stores/useThemeStore";
import { Popover, PopoverContent, PopoverTrigger } from "../ui/popover";
import { Smile } from "lucide-react";
import { Picker } from "emoji-mart";
import data from "@emoji-mart/data";
import { useEffect, useRef } from "react";

interface EmojiPickerProps {
  onChange: (value: string) => void;
}

interface EmojiSelection {
  native: string;
}
const EmojiPicker = ({ onChange }: EmojiPickerProps) => {
  const { isDark } = useThemeStore();
  const pickerContainerRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const container = pickerContainerRef.current;

    if (!container) return;

    const picker = new Picker({
      theme: isDark ? "dark" : "light",
      data,
      onEmojiSelect: (emoji: EmojiSelection) => onChange(emoji.native),
      emojiSize: 24,
      perLine: 7,
    });

    container.appendChild(picker as unknown as Node);
    return () => {
      container.replaceChildren();
    };
  }, [isDark, onChange]);
  return (
    <Popover>
      <PopoverTrigger className="cursor-pointer">
        <Smile className="size-4" />
      </PopoverTrigger>

      <PopoverContent
        side="top"
        align="end"
        sideOffset={12}
        collisionPadding={8}
        className="mb-0 w-auto max-w-[calc(100vw-1rem)] overflow-hidden border-none bg-transparent p-0 shadow-none drop-shadow-none"
      >
        <div ref={pickerContainerRef} />
      </PopoverContent>
    </Popover>
  );
};

export default EmojiPicker;
