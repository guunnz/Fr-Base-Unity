using System;
using System.Collections.Generic;
using System.Linq;
using FriendsView.Core.Domain;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsView.View
{
    public class ModalSocial : MonoBehaviour
    {
        public List<FriendRow> friendRows;
        public List<RequestRow> requestsRows;

        public Button friendsTab;
        public Button requestsTab;

        public List<GameObject> tabs;
        public GameObject friendsBackgroundText;
        public GameObject reqBackgroundText;

        [SerializeField] GameObject requNumberGO;
        [SerializeField] TextMeshProUGUI requNumber;

        public IObservable<Unit> OnRequestsTab => requestsTab.OnClickAsObservable();
        public IObservable<Unit> OnFriendsTab => friendsTab.OnClickAsObservable();

        public IEnumerable<IObservable<Unit>> OnFriendCardBtns =>
            friendRows.Select(f => f.friendCardButton.OnClickAsObservable());

        public IEnumerable<IObservable<Unit>> OnFriendRowBtns =>
            friendRows.Select(f => f.rowButton.OnClickAsObservable());

        public IEnumerable<IObservable<Unit>> OnRequestRowBtns =>
            requestsRows.Select(f => f.rowButton.OnClickAsObservable());

        public IEnumerable<IObservable<Unit>> OnVisitFriendBtns =>
            friendRows.Select(f => f.visitFriendButton.OnClickAsObservable());

        public IEnumerable<IObservable<Unit>> OnAcceptBtn =>
            requestsRows.Select(f => f.acceptButton.OnClickAsObservable());

        public IEnumerable<IObservable<Unit>> OnRejectBtn =>
            requestsRows.Select(f => f.rejectButton.OnClickAsObservable());


        public void SetFriendRows(List<FriendData> friendData)
        {
            //Todo: restore original data when connection status and field gets ready

            friendsBackgroundText.SetActive(true);

            foreach (var row in friendRows)
            {
                row.gameObject.SetActive(false);
            }

            for (int i = 0; i < friendData.Count; i++)
            {
                friendsBackgroundText.SetActive(false);

                var data = new FriendData(friendData[i].userID, friendData[i].fireBaseUID, friendData[i].username,
                    friendData[i].realName, friendData[i].roomType,
                    friendData[i].friendCount, friendData[i].gems, friendData[i].gold,
                    friendData[i].friendRequestsCount, friendData[i].roomInstanceId,
                    friendData[i].roomNamePrefab,
                    friendData[i].avatarCustomizationData);
                if (i < friendRows.Count)
                {
                    friendRows[i].gameObject.SetActive(true);
                    friendRows[i].SetFriendRow(data);
                }
            }
        }

        public void SetRequestRows(List<FriendRequestData> friendRequest)
        {
            var filteredFriendRequest = friendRequest.FindAll(x => string.Equals(x.requestStatus, "pending"));
            foreach (var row in requestsRows)
            {
                row.gameObject.SetActive(false);
                row.ResetRow();
            }

            reqBackgroundText.SetActive(true);
            requNumberGO.SetActive(false);

            for (int i = 0; i < filteredFriendRequest.Count; i++)
            {
                reqBackgroundText.SetActive(false);
                requNumberGO.SetActive(true);
                requNumber.SetText(filteredFriendRequest.Count.ToString());

                if (i < requestsRows.Count)
                {
                    requestsRows[i].gameObject.SetActive(true);
                    requestsRows[i].SetFriendRequestRow(filteredFriendRequest[i]);
                }
            }
        }

        //Todo: 
        //Integration with MatiasSetFriendModal objects pool 
        public void SetFriendListButton(bool interactable)
        {
            friendsTab.interactable = interactable;
        }

        public void SwitchTabs()
        {
            if (!IsShowingFriendRows() && !friendsTab.interactable)
                return;
            foreach (var tab in tabs)
            {
                tab.gameObject.SetActive(!tab.gameObject.activeSelf);
            }
        }

        public bool IsShowingFriendRows()
        {
            if (tabs[0].activeSelf)
            {
                return true;
            }

            return false;
        }
    }
}