using Newtonsoft.Json.Linq;

namespace Socket
{
    public class IncomingEventManager
    {
        private bool isInsideRoom;

        public IncomingEventManager()
        {
            isInsideRoom = false;
        }

        public void OnCloseConnection()
        {
            isInsideRoom = false;
        }

        public AbstractIncomingSocketEvent GetIncomingSocketEvent(string message)
        {
            JObject jsonMessage = JObject.Parse(message);

            string eventType = jsonMessage[SocketTags.EVENT].Value<string>();
            JObject payload = jsonMessage[SocketTags.PAYLOAD].Value<JObject>();
            string reference = jsonMessage[SocketTags.REF].Value<string>();
            string topic = jsonMessage[SocketTags.TOPIC].Value<string>();

            if (eventType.Equals("phx_reply") && reference.Contains("join") && !isInsideRoom)
            {
                if (message.Contains("minigame") || message.Contains("racing"))
                {
                    isInsideRoom = false;
                    return null;
                }
                IncomingEventJoinRoom incomingEventJoinRoom = new IncomingEventJoinRoom(jsonMessage);
                isInsideRoom = incomingEventJoinRoom.State == SocketEventResult.OPERATION_SUCCEED;
                return incomingEventJoinRoom;
            }

            if (eventType.Equals("positions_update") && isInsideRoom)
            {
                return new IncomingEventMoveAvatar(jsonMessage);
            }

            if (eventType.Equals("match"))
            {
                return new IncomingEventMatchFound(jsonMessage);
            }

            if (eventType.Equals("game_invitation_status_update"))
            {
                return new IncomingEventMinigameStatus(jsonMessage);
            }

            if (eventType.Equals("game_invitation_recieved"))
            {
                return new IncomingEventMinigameInvite(jsonMessage);
            }

            if (eventType.Equals("accelerate"))
            {
                return new IncomingEventRacingAccelerate(jsonMessage);
            }

            if (eventType.Equals("user_finish"))
            {
                return new IncomingEventRacingEnd(jsonMessage);
            }

            if (eventType.Equals("match_finish"))
            {
                return new IncomingEventRacingMatchFinished(jsonMessage);
            }

            if (eventType.Equals("turn_left"))
            {
                return new IncomingEventRacingTurnLeft(jsonMessage);
            }

            if (eventType.Equals("play_again"))
            {
                return new IncomingEventPlayAgain(jsonMessage);
            }

            if (eventType.Equals("turn_right"))
            {
                return new IncomingEventRacingTurnRight(jsonMessage);
            }


            if (eventType.Equals("stop"))
            {
                return new IncomingEventRacingStop(jsonMessage);
            }

            if (eventType.Equals("custom_msg"))
            {
                return new IncomingEventRacingCustomMessage(jsonMessage);
            }


            if (eventType.Equals("member_join") && isInsideRoom)
            {
                return new IncomingEventUserEnter(jsonMessage);
            }

            if (eventType.Equals("phx_close") && isInsideRoom)
            {
                isInsideRoom = false;
                return new IncomingEventLeaveRoom(jsonMessage);
            }

            if (eventType.Equals("member_left") && isInsideRoom)
            {
                return new IncomingEventUserLeave(jsonMessage);
            }

            if (eventType.Equals("player_state_update") && isInsideRoom)
            {
                return new IncomingEventChangePlayerState(jsonMessage);
            }

            if (eventType.Equals("message") && isInsideRoom)
            {
                return new IncomingEventChatMessage(jsonMessage);
            }

            if (eventType.Equals("private_msg") && isInsideRoom)
            {
                return new IncomingEventPrivateChatMessage(jsonMessage);
            }

            if (eventType.Equals("furniture_set") && isInsideRoom)
            {
                return new IncomingEventFurnitureAdd(jsonMessage);
            }

            if (eventType.Equals("furniture_moved") && isInsideRoom)
            {
                return new IncomingEventFurnitureMove(jsonMessage);
            }

            if (eventType.Equals("furniture_removed") && isInsideRoom)
            {
                return new IncomingEventFurnitureRemove(jsonMessage);
            }

            if (eventType.Equals("new_gem") && isInsideRoom)
            {
                return new IncomingEventNewGem(jsonMessage);
            }

            if (eventType.Equals("player_pet_change"))
            {
                return new IncomingEventNewPetInfo(jsonMessage);
            }

            if (eventType.Equals("gem_picked") && isInsideRoom)
            {
                return new IncomingEventUserClaimedGem(jsonMessage);
            }

            if (eventType.Equals("user_status_update"))
            {
                return new IncomingEventUserStatus(jsonMessage);
            }

            if (eventType.Equals("phx_join"))
            {
                return new IncomingEventUserStatus(jsonMessage);
            }
            return null;
        }
    }
}