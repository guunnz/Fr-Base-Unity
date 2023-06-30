using Architecture.Injector.Core;
using Audio.Music;
using Functional.Maybe;
using PlayerRoom.Core.Services;
using PlayerRoom.Delivery;
using UnityEngine;

namespace PlayerRoom.View.RoomComponents
{
    public interface IRoomBackground
    {
        Bounds Bounds { get; }
    }

    public class RoomBackgroundView : RoomViewComponent, IRoomBackground
    {
        [SerializeField] SpriteRenderer backgroundRenderer;
        [SerializeField] Transform player;

        IMusicPlayer musicPlayer;
        IRoomMask roomMask;

        public override void Write()
        {
            Injection.Register<IRoomBackground>(this);
        }

        public override void Read()
        {
            Injection.Get(out musicPlayer);
            Injection.Get(out roomMask);
        }

        //test
        // #if UNITY_EDITOR
        string song;

        void Update()
        {
            var worldPoint = player.transform.position;
            var newSong = GetBestSong(worldPoint).OrElse(string.Empty);

            if (string.IsNullOrEmpty(newSong)) return;
            if (newSong == song) return;

            song = newSong;
            //musicPlayer.Play(song, 0.5f);
        }


        public Maybe<string> GetBestSong(Vector2 worldPosition) //returns song id
        {
            return RoomData.Select(roomData =>
            {
                const float minColourDistance = 0.1f;
                var count = roomData.musicPerArea.Count;
                for (var i = 0; i < count; ++i)
                {
                    var colourMusic = roomData.musicPerArea[i];
                    var colorDistance = roomMask.ColorDistance(colourMusic.color, worldPosition);

                    if (colorDistance < minColourDistance)
                    {
                        return colourMusic.trackId;
                    }
                }

                return roomData.musicClipKey; //default value
            });
        }

        protected override void DidLoadRoom()
        {
            RoomData.Do(roomData =>
            {
                backgroundRenderer.sprite = roomData.background;
                song = roomData.musicClipKey;
                //musicPlayer.Play(song);
            });
        }

        public Bounds Bounds => backgroundRenderer.bounds;
    }
}