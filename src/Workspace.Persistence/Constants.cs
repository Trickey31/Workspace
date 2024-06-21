namespace Workspace.Persistence
{
    public static class Constants
    {
        #region *********** TableNames ***********

        // *********** Plural Nouns ***********
        internal const string Actions = nameof(Actions);
        internal const string Functions = nameof(Functions);
        internal const string ActionInFunctions = nameof(ActionInFunctions);
        internal const string Permissions = nameof(Permissions);

        internal const string Users = nameof(Users);
        internal const string Roles = nameof(Roles);
        internal const string UserRoles = nameof(UserRoles);

        internal const string UserClaims = nameof(UserClaims); // IdentityUserClaim
        internal const string RoleClaims = nameof(RoleClaims); // IdentityRoleClaim
        internal const string UserLogins = nameof(UserLogins); // IdentityRoleClaim
        internal const string UserTokens = nameof(UserTokens); // IdentityUserToken

        // *********** Singular Nouns ***********
        internal const string Task = nameof(Task);
        internal const string Project = nameof(Project);
        internal const string Users_Projects = nameof(Users_Projects);
        internal const string Parent_CTerms = nameof(Parent_CTerms);
        internal const string CTerms = nameof(CTerms);
        internal const string Files = nameof(Files);
        internal const string Log = nameof(Log);
        internal const string Notification = nameof(Notification);
        internal const string Comment = nameof(Comment);



        #endregion

        #region *********** Constants ***********

        public const int DELETED = 1;
        public const int IS_DELETE = 0;
        public const int ADMINISTRATOR = 1;
        public const int MEMBER = 2;
        public const int VIEWER = 3;
        public const int HIGH = 3;
        public const int LOW = 1;
        public const int MEDIUM = 2;
        public const int TODO = 1;
        public const int INPROGRESS = 2;
        public const int DONE = 3;

        public const int TASK_NOTIFICATION = 1;
        public const int ACCESS_NOTIFICATION = 2;
        public const int COMMENT_NOTIFICATION = 3;



        #endregion
    }
}
