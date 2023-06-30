using Data.Users;
public class MinigameInformation
{
    public Game idGame;
    public UserInformation userInfo1;
    public UserInformation userInfo2;
    public bool isMultiplayer;

    public MinigameInformation(Game game, UserInformation user1, UserInformation user2, bool isMultiplayer)
    {
        this.idGame = game;
        userInfo1 = user1;
        userInfo2 = user2;
        this.isMultiplayer = false;
    }
}