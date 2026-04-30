"use client";

import { useEffect, useState } from "react";

type Theme = "light" | "dark";

export function useTheme() {
  const [theme, setTheme] = useState<Theme>("light");

  // Load saved theme
  useEffect(() => {
    const saved = localStorage.getItem("theme") as Theme | null;
    const initial = saved || "light";

    setTheme(initial);
    document.documentElement.setAttribute("data-theme", initial);
  }, []);

  // Apply theme
  const toggleTheme = () => {
    const next: Theme = theme === "light" ? "dark" : "light";

    setTheme(next);
    localStorage.setItem("theme", next);

    document.documentElement.setAttribute("data-theme", next);
  };

  return { theme, toggleTheme };
}
