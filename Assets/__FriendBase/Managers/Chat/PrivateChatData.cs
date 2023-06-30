using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrivateChatData 
{
    public AvatarRoomData AvatarData { get; private set; }
    public List<UserChatData> ChatData { get; private set; }
    public bool HasUnreadMessages { get; set; }

    public PrivateChatData(AvatarRoomData avatarData, List<UserChatData> chatData)
    {
        AvatarData = avatarData;
        ChatData = chatData;
        HasUnreadMessages = false;
    }

    public void AddChat(UserChatData chatData)
    {
        ChatData.Add(chatData);
    }
}
