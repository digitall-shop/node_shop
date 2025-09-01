/*
import { useEffect, useState, type FC, type ReactNode } from "react";
import { TelegramContext, type TelegramContextType } from "./TelegramContext";
import type { TelegramUser } from "./TelegramTypes";

interface TelegramProviderProps {
  children: ReactNode;
}

export const TelegramProvider: FC<TelegramProviderProps> = ({ children }) => {
  const [user, setUser] = useState<TelegramUser | null>(null);
  const [loading, setLoading] = useState<boolean>(true);

  useEffect(() => {
    const tg = window.Telegram?.WebApp;

    if (tg?.initDataUnsafe?.user) {
      setUser(tg.initDataUnsafe.user);
    }


    setLoading(false);
  }, []);

  const value: TelegramContextType = { user, loading };

  return (
    <TelegramContext.Provider value={value}>
      {children}
    </TelegramContext.Provider>
  );
};
*/

import { useEffect, useState, type FC, type ReactNode } from "react";
import { TelegramContext } from "./TelegramContext";
import type { TelegramUser } from "./TelegramTypes";

interface TelegramProviderProps {
  children: ReactNode;
}

export const TelegramProvider: FC<TelegramProviderProps> = ({ children }) => {
  const [user, setUser] = useState<TelegramUser | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const tg = window.Telegram?.WebApp;
    const env = import.meta.env;

    if (tg?.initDataUnsafe?.user) {
      setUser(tg.initDataUnsafe.user);
      setLoading(false);
      return;
    }

    if (env.DEV && env.VITE_MOCK_TG_USER === "true") {
      const fakeUser: TelegramUser = {
        id: Number(env.VITE_DEV_TG_USER_ID),
        first_name: env.VITE_DEV_TG_USER_FIRST_NAME || "Dev",
        username: env.VITE_DEV_TG_USER_USERNAME || "dev_user",
        language_code: env.VITE_DEV_TG_USER_LANG || "en",
      };
      console.warn("🚧 Mock Telegram User Activated", fakeUser);
      setUser(fakeUser);
    }

    setLoading(false);
  }, []);

  return (
      <TelegramContext.Provider value={{ user, loading }}>
        {children}
      </TelegramContext.Provider>
  );
};
