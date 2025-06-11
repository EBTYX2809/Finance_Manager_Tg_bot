using Finance_Manager_Tg_bot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance_Manager_Tg_bot.BackendApi;

public class ApiException : Exception
{
    public ErrorResponse? Error { get; }

    public ApiException(ErrorResponse? error)
        : base(error?.Message ?? "API error occurred")
    {
        Error = error;
    }
}
