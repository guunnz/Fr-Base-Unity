namespace PlayerRoom.View
{
    public class RoomNavigation : RoomViewComponent
    {
        RoomFixedData[] colliders;


        protected override void DidLoadRoom()
        {
            colliders ??= GetComponentsInChildren<RoomFixedData>(true);

            foreach (var part in colliders)
            {
                part.gameObject.SetActive(part.roomId == RoomId);
            }
        }
    }
}