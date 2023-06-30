using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Socket
{
    public class SocketTags
    {
        public const string EVENT = "event";
        public const string PAYLOAD = "payload";
        public const string REF = "ref";
        public const string TOPIC = "topic";
        public const string RESPONSE = "response";

        public const string ROOM_OWNER_ID = "room_owner_id";
        public const string FIREBASE_ID = "user_firebase_uid";
        public const string USER_ID = "user_id";
        public const string PET_ID = "pet_item_id";
        public const string PET_ID_IN_GAME = "pet_item_id_in_game";
        public const string PET_PREFAB = "pet_item_name_prefab";
        public const string MEMBER_POSITIONS = "member_positions";
        public const string POSITION_X = "position_x";
        public const string POSITION_Y = "position_y";
        public const string DESTINATION_X = "destination_x";
        public const string DESTINATION_Y = "destination_y";

        public const string AVATAR_STATE = "state";
        public const string AVATAR_DATA = "user_avatar";
        public const string PET_DATA = "user_pet";
        public const string ORIENTATION = "orientation";
        public const string USER_NAME = "username";

        public const string AFFECTED_MEMBER = "affected_member";

        public const string CHAT_CONTENT = "content";

        //In room gems
        public const string BODY = "body";
        public const string TOKEN = "token";
        public const string GEMS_USER_FIREBASE_UID = "firebase_uid";
        //

        public const string INVENTORY_ITEM_ID = "inventory_item_id";
        public const string ITEM_ID = "item_id";
        public const string ITEM_TYPE = "item_type";
        public const string ID = "id";

        public const string FURNITURE = "furniture";
        public const string ITEM_DATA = "item_data";
        public const string ID_IN_GAME = "id_in_game";
        public const string TYPE = "type";
        public const string SET_FURNITURE = "set_furniture";
        public const string MOVED_FURNITURE = "moved_furniture";
        public const string REMOVED_FURNITURE = "removed_furniture";

        public const string EVENT_DATA = "event_data";
        public const string EVENT_TYPE = "event_type";

        public const string STATUS = "status";
    }
}
