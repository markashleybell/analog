﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace analog
{
    public class QueryResult
    {
        public bool Success { get; set; }
        public DataTable Results { get; set; }
        public string Error { get; set; }
    }
}
