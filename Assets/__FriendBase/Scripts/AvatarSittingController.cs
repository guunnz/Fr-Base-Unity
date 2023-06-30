using Data.Rooms;
using PlayerRoom.View;
using Socket;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarSittingController : MonoBehaviour
{
    [SerializeField] float distanceToSit;
    [SerializeField] AvatarRoomController avatarRoomController;
    Chair chair;

    private Transform chairTransform;

    public void GoSit(Chair chair)
    {
        chairTransform = chair.transform;
        this.chair = chair;
        var sitPos = GetChairSitPoint();
        avatarRoomController.SetWalkToDestination(sitPos.x, sitPos.y, true);
    }

    private void Update()
    {
        if (avatarRoomController.GetState() == AvatarState.GoingToSit && chairTransform != null)
        {
            if (Vector3.Distance(GetChairSitPoint(), this.transform.position) < chair.GetChairSittingLeaway())
            {
                if (chair.IsOccupied())
                {
                    avatarRoomController.SetState(AvatarState.Moving, useSocket: false);
                    return;
                }
                //Debug.LogError(chair.transform.lossyScale.x);
                transform.localScale = new Vector3(chair.transform.lossyScale.x > 0 ? 1 : -1, transform.localScale.y, transform.localScale.z);
                avatarRoomController.SetState(AvatarState.Sitting, chair.GetSitpoint().x + (avatarRoomController.AnimationController.ButtOffset.x * (chair.transform.lossyScale.x > 0 ? 1 : -1)), chair.GetSitpoint().y - avatarRoomController.AnimationController.ButtOffset.y, (int)chair.transform.lossyScale.x > 0 ? 1 : -1, true);
            }
        }
    }

    Vector3 GetChairSitPoint()
    {
        if (chair == null)
        {
            this.chair = chairTransform.GetComponent<Chair>();
        }
        var buttOffset = avatarRoomController.AnimationController.ButtOffset;
        if (chair.transform.lossyScale.x > 0)
        {
            buttOffset.x = -Mathf.Abs(buttOffset.x);
        }
        else
        {
            buttOffset.x = +Mathf.Abs(buttOffset.x);
        }

        return chair.GetSitpoint() - buttOffset;
    }

    public void SetChair(Chair chair)
    {
        this.chair = chair;
    }

    public void OnDestroy()
    {
        if (this.chair != null)
        {
            DeocupyChair();
        }
    }

    public void OccupyChair()
    {
        try
        {
            StartCoroutine(OccupyChairCoroutine());
        }
        catch
        { }
    }

    private IEnumerator OccupyChairCoroutine()
    {
        while (chairTransform == null || this.chair == null)
        {
            this.chair = CurrentRoom.Instance.GetChair(this.transform.position);
            if (chair != null)
            {
                chairTransform = this.chair.transform;
            }
            yield return null;
        }

        this.transform.localScale = new Vector3(chair.transform.lossyScale.x > 0 ? 1 : -1, transform.localScale.y, transform.localScale.z);
        this.chair.SetChairOccupied(true, this);
    }



    public void DeocupyChair()
    {
        try
        {
            this.chair.SetChairOccupied(false, this);
            this.chair = null;
        }
        catch
        {

        }
    }
}