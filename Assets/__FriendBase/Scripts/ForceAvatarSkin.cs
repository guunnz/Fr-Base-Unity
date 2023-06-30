using System.Collections;
using System.Collections.Generic;
using Architecture.Injector.Core;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class ForceAvatarSkin : MonoBehaviour
{
    [SerializeField] private AvatarCustomizationController avatar;
    void Start()
    {
        Injection.Get<ILoading>().Unload();

        TextAsset avatarSkinCarlos = Resources.Load("Avatar/SkinCarlos") as TextAsset;
        JObject jsonAvatar = JObject.Parse(avatarSkinCarlos.ToString());
        avatar.SetAvatarCustomizationData(jsonAvatar);
    }
}
