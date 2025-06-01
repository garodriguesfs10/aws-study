namespace Producer.API.Command
{    
    public record UserRegistrationCommand(string UserName, string Email);
}