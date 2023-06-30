using Data.Users;

public class IncomingUserLogin : AbstractServerIncomingTrama
{
    public override string TramaID => IDTramas.USER_LOGIN;

    public UserInformation UserInformation { get; }

    public IncomingUserLogin(int state)
    {
        State = state;
    }

    public IncomingUserLogin(int state, UserInformation userInformation)
    {
        State = state;
        UserInformation = userInformation;
    }

    public override void Serialize()
    {

    }
}
