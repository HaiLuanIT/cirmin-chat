import { useNavigate } from "react-router";
import { useAuthStore } from "../../stores/useAuthStore";
import { Button } from "../ui/button";

const SignOut = () => {
  const { signOut } = useAuthStore();
  const navigate = useNavigate();
  const handleSignOut = async () => {
    try {
      await signOut();
      navigate("/signin");
    } catch (error) {
      console.error(error);
    }
  };
  return <Button onClick={handleSignOut}>Đăng xuất</Button>;
};

export default SignOut;
