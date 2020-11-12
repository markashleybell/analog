using System.Data;

namespace Analog
{
    public class QueryResult
    {
        public bool Success { get; set; }

        public DataTable Results { get; set; }

        public string Error { get; set; }
    }
}
