namespace Finance_Manager_Tg_bot.Models;

public class AuthUserTokensDTO
{
    public UserDTO UserDTO { get; set; } = new();
    public string AccessJwtToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
