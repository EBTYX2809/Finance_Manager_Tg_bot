namespace Finance_Manager_Tg_bot.Models;

public class UserBalanceDTO
{
    public CurrencyBalanceDTO PrimaryBalance { get; set; }
    public CurrencyBalanceDTO? SecondaryBalance1 { get; set; }
    public CurrencyBalanceDTO? SecondaryBalance2 { get; set; }
}
