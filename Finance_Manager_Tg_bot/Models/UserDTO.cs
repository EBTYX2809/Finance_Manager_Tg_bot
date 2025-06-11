namespace Finance_Manager_Tg_bot.Models;

public class UserDTO
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public decimal Balance { get; set; }
}
