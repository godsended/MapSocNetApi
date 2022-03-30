namespace BumberAPI.Models
{
    public static class URLs
    {
        #region Auth
        public const string registration = "reg";
        public const string login = "log";
        public const string loginWithToken = "toklog";
        public const string genPrc = "genprc";
        public const string aplPrc = "aplprc";
        public const string changePass = "changepass";
        public const string changePassByCode = "changepassbycode";
        #endregion

        public const string loadDescription = "lddesc";
        public const string updateDescription = "upddesc";
        public const string updateShortDescription = "updsdesc";
        public const string updateStatus = "updst";
        public const string uploadImage = "uplimg";
        public const string getImage = "getimg";

        public const string addFriend = "addfr";
        public const string deleteFriend = "delfr";
        public const string loadFriendList = "getfrls";
        public const string findFriend = "findfr";
    }
}