namespace Iprogs.Matesup.Models
{
    public static class MatesUpConstants
    {
        public const long MatesUpArenaId = 1;

        public enum ChatRoomPrivacy
        {
            Private = 1,
            Public = 2,
            Protected = 3
        }

        public enum ChatRoomType
        {
            MatesUpArena = 1,
            Private = 2,
            Public = 3,
            Group = 4,
            ProtectedPublic = 5
        }

        public enum Gender
        {
            Male = 1,
            Female = 2,
            Trans = 3,
            Genderless = 4
        }

        public enum RelationshipStatus
        {
            Single = 1,
            InaRelationship = 2,
            Married = 3
        }

        public static List<string> SupportedImageFileTypes
        {
            get
            {
                return new List<string>()
                {
                    "jpg", "jpeg", "png", "gif"
                };
            }
        }

        public static List<string> UnsupportedHTMLTags
        {
            get
            {
                return new List<string>()
                {
                    "script", "video", "embed", "link", "meta", "noscript", "plaintext", "xmp", "object", "frame", "iframe", "frameset", "listing"
                };
            }
        }

        public static List<string> AllowedImageTypes
        {
            get
            {
                var _list = new List<string>();
                _list.Add("image/jpeg");
                _list.Add("image/jpg");
                _list.Add("image/gif");
                _list.Add("image/png");

                return _list;
            }
        }

        public static List<int> PublicChatRoomPrivacy
        {
            get
            {
                var _list = new List<int>();
                _list.Add(2);
                _list.Add(3);

                return _list;
            }
        }

        public static List<int> PublicChatRoomTypes
        {
            get
            {
                var _list = new List<int>();
                _list.Add(3);

                return _list;
            }
        }
    }
}
