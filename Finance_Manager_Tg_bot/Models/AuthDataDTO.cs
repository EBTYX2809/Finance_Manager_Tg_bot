using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance_Manager_Tg_bot.Models;

public class AuthDataDTO
{
    private string _email = string.Empty;

    public string email
    {
        get => _email;
        set => _email = value?.Trim() ?? string.Empty; // For delete spaces from email
    }

    public string password { get; set; } = string.Empty;
}
