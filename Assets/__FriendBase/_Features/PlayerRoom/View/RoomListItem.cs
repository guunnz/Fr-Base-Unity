using System;
using Functional.Maybe;
using PlayerRoom.Core.Domain;
using PlayerRoom.Delivery;
using TMPro;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerRoom.View
{
    public class RoomListItem : MonoBehaviour
    {
        RoomsData roomsDataCache;
        public RoomsData RoomsData => roomsDataCache ??= Resources.Load<RoomsData>("RoomsData");

        [SerializeField] Button button;
        [SerializeField] UIWidget<string> nameLabel;
        [SerializeField] UIWidget<string> countLabel;
        [SerializeField] Image thumbnail;
        [SerializeField] AspectRatioFitter fitter;

        [SerializeField] Image littlePerson;
        [SerializeField] Sprite littlePersonColor;
        [SerializeField] Sprite littlePersonGrey;

        [SerializeField] Color numberColor = Color.red;
        [SerializeField] Color numberGrey = Color.grey;


        public void ShowInfo(RoomInfo roomInfo, bool isInstance)
        {
            nameLabel.Value = roomInfo.RoomName;
            var count = Mathf.Max(isInstance ? roomInfo.PlayersOnRoom : roomInfo.PlayersOnArea, 0);
            countLabel.Value = count.ToString();

            if (countLabel is ISetColor setColor)
            {
                setColor.SetColor(count == 0 ? numberGrey : numberColor);
            }

            littlePerson.sprite = count == 0 ? littlePersonGrey : littlePersonColor;

            gameObject.name = "Join Room ID : " + roomInfo.AreaId;
            RoomsData.GetItem(roomInfo.AreaId).Do(data =>
            {
                var sprite = data.thumbnail ?? data.background;
                if (sprite)
                {
                    thumbnail.sprite = sprite;
                    var rect = sprite.rect;
                    var aspect = rect.width / rect.height;
                    fitter.aspectRatio = aspect;
                }
            });
        }

        public IObservable<Unit> OnClick =>
            button.OnClickAsObservable().Do(_ => Debug.Log("click on " + gameObject.name, this));
    }
}