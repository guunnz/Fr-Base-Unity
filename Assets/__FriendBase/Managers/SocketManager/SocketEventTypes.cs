using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Socket
{
    public class SocketEventTypes
    {
        public const string JOIN_ROOM = "join_room";
        public const string AVATAR_MOVE = "avatar_move";
        public const string USER_ENTER = "user_enter";
        public const string USER_LEAVE = "user_leave";
        public const string LEAVE_ROOM = "leave_room";
        public const string CHAT_MESSAGE = "chat_message";
        public const string ADD_FURNITURE = "add_furniture";
        public const string MOVE_FURNITURE = "move_furniture";
        public const string REMOVE_FURNITURE = "remove_furniture";
        public const string AVATAR_STATE = "avatar_state";
        public const string PET_INFO = "player_pet_change";
        public const string NEW_GEM = "new_gem";
        public const string GEM_PICKED = "gem_picked";
        public const string USER_STATUS = "user_status";
        public const string PLAY_AGAIN = "play_again";
        public const string PRIVATE_CHAT_MESSAGE = "private_chat_message";
        public const string MATCH_EVENT = "match";
        public const string MINIGAME_INVITE = "game_invitation_recieved";
        public const string MINIGAME_INVITE_STATUS_UPDATE = "game_invitation_status_update";
        public const string RACING_CUSTOM_MESSAGE = "custom_msg";
        public const string RACING_ACCELERATE = "accelerate";
        public const string RACING_MATCH_FINISH = "match_finish";
        public const string RACING_END = "user_finish";
        public const string RACING_TURN_RIGHT = "turn_right";
        public const string RACING_TURN_LEFT = "turn_left";
        public const string RACING_STOP = "stop";
    }
}