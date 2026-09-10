import type { LocaleState, SupportedLanguage } from "@/types/store";
import { create } from "zustand";
import { persist } from "zustand/middleware";
import i18n from "@/i18n";

export const supportedLanguages: SupportedLanguage[] = ["vi", "en"];
const DEFAULT_LANGUAGE: SupportedLanguage = "vi";

const applyLanguage = async (languages: SupportedLanguage) => {
  await i18n.changeLanguage(languages);
  document.documentElement.lang = languages;
};
export const useLocaleStore = create<LocaleState>()(
  persist(
    (set) => ({
      lang: DEFAULT_LANGUAGE,
      setLang: async (lang) => {
        await applyLanguage(lang);
        set({ lang });
      },
    }),
    {
      name: "cirmin-languages",
      onRehydrateStorage: () => (state) => {
        if (!state) return;

        const languages = supportedLanguages.includes(state.lang)
          ? state.lang
          : DEFAULT_LANGUAGE;

        void applyLanguage(languages);
      },
    },
  ),
);
