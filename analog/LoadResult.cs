using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace analog
{
    public class LoadResult
    {
        public bool Success { get; set; }
        public int Files { get; set; }
        public int Count { get; set; }
        public string Error { get; set; }
    }
}
