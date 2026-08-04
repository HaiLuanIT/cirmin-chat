import { Navigate, Outlet } from "react-router";
import { useAuthStore } from "../../stores/useAuthStore";
import { useEffect, useState } from "react";

const ProtectedRoute = () => {
  const accessToken = useAuthStore((state) => state.accessToken);
  const loading = useAuthStore((state) => state.loading);
  const [starting, setStarting] = useState(true);

  useEffect(() => {
    let active = true;
    const init = async () => {
      try {
        const authStore = useAuthStore.getState();
        if (!authStore.accessToken) {
          await authStore.refresh();
        }

        const latestAuthStore = useAuthStore.getState();

        if (latestAuthStore.accessToken && !latestAuthStore.user) {
          await latestAuthStore.fetchMe();
        }
      } catch (error) {
        console.error("Không thể khôi phục phiên đăng nhập", error);
      } finally {
        if (active) {
          setStarting(false);
        }
      }
    };
    init();
  }, []);

  if (starting || loading) {
    return (
      <div className="flex h-screen items-center justify-center">
        Đang tải trang...
      </div>
    );
  }
  if (!accessToken) {
    return <Navigate to="/signin" replace />;
  }

  return <Outlet></Outlet>;
};

export default ProtectedRoute;
