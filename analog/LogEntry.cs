using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace analog
{
    public class LogEntry
    {
        public LogEntry(IDataRecord record)
        {
            DateTime = DateTime.ParseExact(record.GetString(0), "yyyy-MM-dd HH:mm:ss", null);
            IPAddress = record.GetString(1);
            Status = record.GetInt32(2);
            Method = record.GetString(3);
            Url = record.GetString(4);
            Query = record[5] == DBNull.Value ? null : record.GetString(5);
            UserAgent = record.GetString(6);
            BytesOut = record.GetInt32(7);
            BytesIn = record.GetInt32(8);
        }

        public DateTime DateTime { get; private set; }
        public string IPAddress { get; private set; }
        public int Status { get; private set; }
        public string Method { get; private set; }
        public string Url { get; private set; }
        public string Query { get; private set; }
        public string UserAgent { get; private set; }
        public int BytesOut { get; private set; }
        public int BytesIn { get; private set; }
    }
}
