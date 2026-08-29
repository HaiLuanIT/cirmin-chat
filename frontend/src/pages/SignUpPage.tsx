import { SignupForm } from "../components/auth/signup-form";
import LanguagesSelector from "../components/languagesSelector/LanguagesSelector";

const SignUpPage = () => {
  return (
    <div className="relative z-0 flex min-h-svh flex-col items-center justify-start overflow-y-auto bg-muted bg-gradient-purple px-4 pb-6 pt-20 sm:p-6 md:justify-center md:p-10">
      <div className="absolute right-4 top-4 z-10 sm:right-6 sm:top-6">
        <LanguagesSelector />
      </div>
      <div className="w-full max-w-sm md:max-w-4xl">
        <SignupForm />
      </div>
    </div>
  );
};

export default SignUpPage;
