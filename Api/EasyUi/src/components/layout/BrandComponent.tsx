import { useTelegramUser } from "../../hooks/useTelegramUser";
import { useTranslation } from "react-i18next";
import { useDirection } from "../../hooks/useDirection";

const BrandComponent = () => {
    const { t } = useTranslation();
    const dir = useDirection();
    const { user, loading } = useTelegramUser();

    if (loading) {
        return (
            <div className="text-gray-400 dark:text-white">
                {t("brand.loading")}
            </div>
        );
    }

    return (
        <div className={dir.classes("flex items-center gap-3", dir.isRtl)}>
            {user?.photo_url && (
                <img
                    src={user.photo_url}
                    alt={t("brand.userAvatarAlt")}
                    className="w-8 h-8 rounded-full border border-cyan-400 shadow"
                />
            )}
            <div
                className={dir.classes(
                    "flex flex-col leading-tight",
                    dir.styles.text.start
                )}
            >
        <span className="font-bold text-gray-600 dark:text-white text-sm sm:text-base">
          {user?.username
              ? `@${user.username}`
              : t("brand.defaultUsername")}
        </span>
                {user?.first_name && (
                    <span className="text-gray-300 text-xs hidden sm:inline">
            {user.first_name} {user.last_name ?? ""}
          </span>
                )}
            </div>
        </div>
    );
};

export default BrandComponent;
