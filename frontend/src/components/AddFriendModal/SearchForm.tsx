import React from "react";
import type {
  FieldErrors,
  UseFormRegister,
  UseFormSetValue,
} from "react-hook-form";
import type { IFromValues } from "../chat/AddFriendModal";
import { Label } from "../ui/label";
import { Input } from "../ui/input";
import { DialogClose, DialogFooter } from "../ui/dialog";
import { Button } from "../ui/button";
import { Search, SearchX, AlertCircle, Loader2, X } from "lucide-react";
import { Trans, useTranslation } from "react-i18next";

interface SearchFormProps {
  register: UseFormRegister<IFromValues>;
  setValue?: UseFormSetValue<IFromValues>;
  errors: FieldErrors<IFromValues>;
  loading: boolean;
  usernameValue: string;
  isFound: boolean | null;
  searchedUsername: string;
  onSubmit?: (e?: React.FormEvent<HTMLFormElement>) => void;
  onCancel: () => void;
}

const SearchForm = ({
  register,
  setValue,
  errors,
  loading,
  usernameValue,
  isFound,
  searchedUsername,
  onSubmit,
  onCancel,
}: SearchFormProps) => {
  const handleClear = () => {
    if (setValue) {
      setValue("username", "", { shouldValidate: true });
    }
  };
  const { t } = useTranslation(["common", "friends"]);

  return (
    <form onSubmit={onSubmit} className="space-y-4">
      <div className="space-y-2">
        <Label
          htmlFor="username"
          className="text-xs font-semibold text-muted-foreground uppercase tracking-wider flex items-center gap-1.5"
        >
          <Search className="size-3.5 text-primary" />
          {t("common:sidebar.directs.form.searchField")}
        </Label>

        <div className="relative flex items-center">
          <Search className="absolute left-3.5 size-4 text-muted-foreground/70 pointer-events-none transition-colors" />

          <Input
            id="username"
            placeholder={t("common:sidebar.directs.form.searchPlaceholder")}
            className="pl-10 pr-9 h-11 rounded-xl glass border-border/60 focus-visible:border-primary/60 focus-visible:ring-2 focus-visible:ring-primary/20 transition-all text-sm shadow-xs"
            {...register("username", {
              required: t("VALIDATION.REQUIRED", { ns: "errors" }),
            })}
          />

          {usernameValue && (
            <button
              type="button"
              onClick={handleClear}
              className="absolute right-3 p-1 rounded-full text-muted-foreground/60 hover:text-foreground hover:bg-muted/60 transition-colors"
              title={t("search.actions.clearInput", { ns: "friends" })}
            >
              <X className="size-3.5" />
            </button>
          )}
        </div>

        {errors.username && (
          <div className="flex items-center gap-2 p-2.5 text-xs font-medium text-destructive bg-destructive/10 border border-destructive/20 rounded-xl animate-in fade-in slide-in-from-top-1">
            <AlertCircle className="size-3.5 shrink-0" />
            <span>{errors.username.message}</span>
          </div>
        )}

        {isFound === false && (
          <div className="flex items-start gap-3 p-3 bg-destructive/10 border border-destructive/20 rounded-xl text-foreground/90 animate-in fade-in zoom-in-95">
            <div className="p-2 bg-destructive/15 rounded-lg text-destructive shrink-0 mt-0.5">
              <SearchX className="size-4" />
            </div>
            <div className="space-y-0.5">
              <p className="font-semibold text-xs text-destructive uppercase tracking-wide">
                {t("friends:search.noResults.title")}
              </p>
              <p className="text-xs text-muted-foreground">
                <Trans
                  ns="friends"
                  i18nKey="search.noResults.description"
                  values={{ username: searchedUsername }}
                  components={{
                    username: (
                      <span className="font-semibold text-foreground" />
                    ),
                  }}
                />
              </p>
            </div>
          </div>
        )}
      </div>

      <DialogFooter className="gap-2 sm:gap-2 sm:flex-row border-t-0 bg-transparent p-0 mx-0 mb-0">
        <DialogClose asChild>
          <Button
            type="button"
            variant="outline"
            className="flex-1 rounded-xl h-10 border-border/60 hover:bg-destructive/10 hover:text-destructive hover:border-destructive/30 transition-all duration-200"
            onClick={onCancel}
          >
            {t("sidebar.directs.form.cancel")}
          </Button>
        </DialogClose>

        <Button
          type="submit"
          disabled={loading || !usernameValue?.trim()}
          className="flex-1 rounded-xl h-10 bg-gradient-chat text-white font-medium shadow-sm hover:shadow-glow hover:scale-[1.01] active:scale-[0.98] transition-all duration-200 disabled:opacity-50 disabled:hover:scale-100 disabled:hover:shadow-none"
        >
          {loading ? (
            <div className="flex items-center justify-center gap-2">
              <Loader2 className="size-4 animate-spin" />
              <span>{t("search.states.searchLoading", { ns: "friends" })}</span>
            </div>
          ) : (
            <div className="flex items-center justify-center gap-2">
              <Search className="size-4" />
              <span>{t("sidebar.directs.form.submit")}</span>
            </div>
          )}
        </Button>
      </DialogFooter>
    </form>
  );
};

export default SearchForm;
