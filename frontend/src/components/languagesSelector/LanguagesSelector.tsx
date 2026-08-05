import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuLabel,
  DropdownMenuRadioGroup,
  DropdownMenuRadioItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { cn } from "@/lib/utils";
import { supportedLanguages, useLocaleStore } from "@/stores/useLocaleStore";
import type { SupportedLanguage } from "@/types/store";
import { ChevronDown, Globe2 } from "lucide-react";
import { useTranslation } from "react-i18next";

const languageMetadata: Record<SupportedLanguage, { shortLabel: string }> = {
  vi: { shortLabel: "VI" },
  en: { shortLabel: "EN" },
};

interface LanguagesSelectorProps {
  className?: string;
  compact?: boolean;
}

const LanguagesSelector = ({
  className,
  compact = false,
}: LanguagesSelectorProps) => {
  const lang = useLocaleStore((state) => state.lang);
  const setLang = useLocaleStore((state) => state.setLang);
  const { t } = useTranslation("common");

  const currentLanguage = languageMetadata[lang];

  const getLanguageLabel = (language: SupportedLanguage) => {
    return t(`language.options.${language}`);
  };
  const handleSelectLanguage = (value: string) => {
    const language = value as SupportedLanguage;

    if (!supportedLanguages.includes(language)) return;
    void setLang(language);
  };

  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <Button
          type="button"
          variant="outline"
          size="sm"
          aria-label={t("language.selectLabel")}
          className={cn(
            "h-9 gap-2 rounded-full border-border/70 bg-background/85 px-3 text-foreground shadow-sm backdrop-blur-md hover:bg-accent",
            compact && "h-8 min-w-0 rounded-lg px-2",
            className,
          )}
        >
          <Globe2 className="size-4 text-primary" aria-hidden="true" />
          <span className={cn("font-medium", compact && "sr-only")}>
            {getLanguageLabel(lang)}
          </span>
          {compact && (
            <span className="text-xs font-semibold tracking-wide">
              {currentLanguage.shortLabel}
            </span>
          )}
          <ChevronDown
            className="size-3.5 text-muted-foreground transition-transform group-aria-expanded/button:rotate-180"
            aria-hidden="true"
          />
        </Button>
      </DropdownMenuTrigger>

      <DropdownMenuContent
        align="end"
        sideOffset={8}
        className="min-w-44 p-1.5"
      >
        <DropdownMenuLabel className="px-2 py-1.5">
          {t("language.selectLabel")}
        </DropdownMenuLabel>
        <DropdownMenuRadioGroup
          value={lang}
          onValueChange={handleSelectLanguage}
        >
          {supportedLanguages.map((language) => {
            const metadata = languageMetadata[language];

            return (
              <DropdownMenuRadioItem
                key={language}
                value={language}
                className="gap-2.5 px-2 py-2.5"
              >
                <span className="flex size-7 items-center justify-center rounded-md bg-primary/10 text-[0.7rem] font-bold tracking-wide text-primary">
                  {metadata.shortLabel}
                </span>
                <span className="font-medium">
                  {getLanguageLabel(language)}
                </span>
              </DropdownMenuRadioItem>
            );
          })}
        </DropdownMenuRadioGroup>
      </DropdownMenuContent>
    </DropdownMenu>
  );
};

export default LanguagesSelector;
