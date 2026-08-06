namespace Moji.Contracts.Errors;

public static class ErrorCodes
{
    public static class Validation
    {
        public const string Failed = "VALIDATION.FAILED";
        public const string Required = "VALIDATION.REQUIRED";
        public const string Invalid = "VALIDATION.INVALID";
        public const string InvalidEmail = "VALIDATION.INVALID_EMAIL";
        public const string MinLength = "VALIDATION.MIN_LENGTH";
        public const string MaxLength = "VALIDATION.MAX_LENGTH";
        public const string InvalidFormat = "VALIDATION.INVALID_FORMAT";
    }

    public static class Auth
    {
        public const string InvalidCredentials =
            "AUTH.INVALID_CREDENTIALS";

        public const string InvalidSession =
            "AUTH.INVALID_SESSION";

        public const string SessionExpired =
            "AUTH.SESSION_EXPIRED";

        public const string OldPasswordIncorrect =
            "AUTH.OLD_PASSWORD_INCORRECT";

        public const string NewPasswordSameAsOld =
            "AUTH.NEW_PASSWORD_SAME_AS_OLD";

        public const string Forbidden =
            "AUTH.FORBIDDEN";
    }

    public static class User
    {
        public const string NotFound =
            "USER.NOT_FOUND";

        public const string UsernameAlreadyExists =
            "USER.USERNAME_ALREADY_EXISTS";

        public const string EmailAlreadyExists =
            "USER.EMAIL_ALREADY_EXISTS";
    }

    public static class Conversation
    {
        public const string MinMembers =
            "CONVERSATION.MIN_MEMBERS";

        public const string NotDuplicatedMember =
            "CONVERSATION.NOT_DUPLICATED_MEMBER";

        public const string CannotInviteSelf =
            "CONVERSATION.CANNOT_INVITE_SELF";

        public const string NotIsFriend =
            "CONVERSATION.NOT_IS_FRIEND";

        public const string NotFound =
            "CONVERSATION.NOT_FOUND";

        public const string Forbidden =
            "CONVERSATION.FORBIDDEN";
    }

    public static class Friendship
    {
        public const string AlreadyFriend =
            "FRIENDSHIP.ALREADY_FRIEND";

        public const string CannotMakeFriendWithSelf =
            "FRIENDSHIP.CANNOT_MAKE_FRIEND_WITH_SELF";

        public const string HasAlreadyRequestedOrIsFriend =
            "FRIENDSHIP.HAS_ALREADY_REQUESTED_OR_IS_FRIEND";

        public const string FriendRequestNotFound = "FRIENDSHIP.FRIEND_REQUEST_NOT_FOUND";

        public const string FriendRequestProcessed = "FRIENDSHIP.FRIEND_REQUEST_PROCESSED";
    }

    public static class Media
    {
        public const string ImageRequired =
            "MEDIA.IMAGE_REQUIRED";

        public const string EmptyImage =
            "MEDIA.EMPTY_IMAGE";

        public const string ImageTooLarge =
            "MEDIA.IMAGE_TOO_LARGE";

        public const string UnsupportedImageType =
            "MEDIA.UNSUPPORTED_IMAGE_TYPE";

        public const string UnsupportedImageFormat =
            "MEDIA.UNSUPPORTED_IMAGE_FORMAT";

        public const string StorageUnavailable =
            "MEDIA.STORAGE_UNAVAILABLE";
    }

    public static class Concurrency
    {
        public const string Conflict =
            "CONCURRENCY.CONFLICT";
    }

    public static class System
    {
        public const string InternalError =
            "SYSTEM.INTERNAL_ERROR";
    }
}