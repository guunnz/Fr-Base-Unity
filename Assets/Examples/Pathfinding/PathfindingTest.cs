using System.Collections;
using System.Collections.Generic;
using Data.Users;
using Pathfinding;
using UnityEngine;

public class PathfindingTest : MonoBehaviour
{
    [SerializeField] private AstarPath astarPath;
    [SerializeField] private AvatarRoomController avatarPrefab;

    void Start()
    {
        StartCoroutine(CreateRoom());
        //AstarPath.active.Scan();

    }

    IEnumerator CreateRoom()
    {
        yield return new WaitForEndOfFrame();
        string id = "rio";
        TextAsset pathsInfo = Resources.Load<TextAsset>(id + "-path");
        AstarPath.active.data.DeserializeGraphs(pathsInfo.bytes);
        yield return new WaitForEndOfFrame();

        //CreateUser(new AvatarRoomData("firebaseId", "userName", "avatarState", 3, -1, 1, new AvatarCustomizationData()));
    }


    void CreateUser(AvatarRoomData avatarData)
    {
        AvatarRoomController avatarRoomController = Instantiate(avatarPrefab, Vector3.zero, avatarPrefab.transform.rotation);
        AvatarRoomController avatarRoomControllerResult = CurrentRoom.Instance.AvatarsManager.AddAvatar(avatarData);
        if (avatarRoomControllerResult == null)
        {
            Destroy(avatarRoomController.gameObject);
        }
    }

    public void UpdateMesh()
    {
        var bounds = GetComponent<Collider2D>().bounds;
        // Expand the bounds along the Z axis
        bounds.Expand(Vector3.forward * 1000);
        var guo = new GraphUpdateObject(bounds);
        // change some settings on the object
        AstarPath.active.UpdateGraphs(guo);
    }

}
