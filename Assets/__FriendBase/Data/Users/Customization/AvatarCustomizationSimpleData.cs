using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data.Catalog;
using Data;
using Data.Users;

public class AvatarCustomizationSimpleData
{
    public static AvatarCustomizationSimpleData Default => new AvatarCustomizationData().GetSerializeData();
    public AvatarCustomizationSimpleDataUnit[] DataUnits { get; private set; }
    
    public AvatarCustomizationSimpleData(AvatarCustomizationSimpleDataUnit[] dataUnits)
    {
        DataUnits = dataUnits;
    }
}
