import i18n from "i18next";
import { initReactI18next } from "react-i18next";
import viCommon from "./locales/vi/common.json";
import enCommon from "./locales/en/common.json";
import viAuth from "./locales/vi/auth.json";
import enAuth from "./locales/en/auth.json";
import viFriend from "./locales/vi/friends.json";
import enFriend from "./locales/en/friends.json";
import viProfile from "./locales/vi/profile.json";
import enProfile from "./locales/en/profile.json";
import viErrors from "./locales/vi/errors.json";
import enErrors from "./locales/en/errors.json";
import viChat from "./locales/vi/chat.json";
import enChat from "./locales/en/chat.json";

const resources = {
  vi: {
    common: viCommon,
    auth: viAuth,
    friends: viFriend,
    profile: viProfile,
    errors: viErrors,
    chat: viChat,
  },
  en: {
    common: enCommon,
    auth: enAuth,
    friends: enFriend,
    profile: enProfile,
    errors: enErrors,
    chat: enChat,
  },
};
void i18n.use(initReactI18next).init({
  resources,
  debug: true,
  lng: "vi",
  fallbackLng: "vi",
  interpolation: {
    escapeValue: false, // not needed for react as it escapes by default
  },
});
export default i18n;
