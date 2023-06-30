using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data.Users;

public class AvatarRoomData
{
    public string FirebaseId { get; private set; }
    public string UserId { get; private set; }
    public string Username { get; private set; }
    public string AvatarState { get; private set; }
    public string PetPrefabName { get; private set; }
    public float Positionx { get; set; }
    public float Positiony { get; set; }
    public int Orientation { get; set; }
    public int? CurrentPetId { get; set; }
    public int? PetIdInGame { get; set; }
    public bool IsGuest { get; set; }
    public AvatarCustomizationData AvatarCustomizationData { get; set; }

    public AvatarRoomData(string firebaseId, string userId, string userName, string avatarState, float positionx, float positiony, int orientation, AvatarCustomizationData avatarCustomizationData, int? currentPetId = null, int? petIdInGame = null, string PetPrefabName = "", bool isGuest = false)
    {
        FirebaseId = firebaseId;
        Username = userName;
        AvatarState = avatarState;
        Positionx = positionx;
        Positiony = positiony;
        Orientation = orientation;
        AvatarCustomizationData = avatarCustomizationData;
        CurrentPetId = currentPetId;
        PetIdInGame = PetIdInGame;
        UserId = userId;
        this.PetPrefabName = PetPrefabName;
        IsGuest = isGuest;
    }

    public void SetState(string state)
    {
        this.AvatarState = state;
    }
}
