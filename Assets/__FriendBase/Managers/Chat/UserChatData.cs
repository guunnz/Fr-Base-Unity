using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UserChatData 
{
    public AvatarRoomData AvatarData { get; private set; }
    public string ChatText { get; set; }
    public bool IsMe { get; private set; }
    public Color Color { get;  set; }
    public float ChatBubbleHeight { get; set; } // I've made this set public so that ChatRow script can change it
    public bool PrivateChat { get; private set; }
    

    public UserChatData(AvatarRoomData avatarData, string chatText, bool isMe, bool privateChat = false)
    {
        AvatarData = avatarData;
        ChatText = chatText;
        IsMe = isMe;
        PrivateChat = privateChat;
    }
}