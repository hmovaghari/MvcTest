using MyAccounting.Data;

namespace MyAccounting.Repository
{
    public class ErrorLogRepository
    {
        public static void SaveErrorLog(SqlDBContext _context, Exception ex, string @class, string method, string callerName, object input)
        {

        }
    }
}
