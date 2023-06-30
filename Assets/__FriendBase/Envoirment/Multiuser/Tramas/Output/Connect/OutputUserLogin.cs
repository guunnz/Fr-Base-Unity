using Multiuser;
using Architecture.Injector.Core;
using Data.Users;

public class OutputUserLogin : AbstractTrama
{
    public override string TramaID => IDTramas.USER_LOGIN;

    public string Mail { get; }
    public string Password { get; }

    public OutputUserLogin(string mail, string password)
    {
        Mail = mail;
        Password = password;
    }

    public override void Serialize()
    {

    }

    public override void Send()
    {
        //Connect to the server and send request

        //When we have request
        //UserInformation userInformation = new UserInformation("0", "Matias", 100, 100, false, false, new AvatarCustomizationData());
        //Injection.Get<IMultiuser>().DeliverTramaToSuscribers(new IncomingUserLogin( TramaResult.OPERATION_SUCCEED, userInformation));
    }
}
