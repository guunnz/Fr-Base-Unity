using System;
using PlayerRoom.View.RoomComponents;
using UnityEngine;

namespace PlayerRoom.View.Testers
{
    public class MaskTester : MonoBehaviour
    {
        public RoomMaskView maskView;
        public SpriteRenderer testRender;
        public Camera cam;

#if !UNITY_EDITOR
        void Start()
        {
            //just in case anyone forgot the tester object active on production
            Destroy(gameObject);
            
        }
#endif


        void Update()
        {
            Vector2 p = cam.ScreenToWorldPoint(Input.mousePosition);

            var d = maskView.ColorDistance(Color.black, p);
            testRender.color = Color.Lerp(Color.red, Color.green, d);
        }
    }
}