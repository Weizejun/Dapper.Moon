using System.Collections.Generic;

namespace Dapper.Moon
{
    public class QueryPageResult<T>
    {
        public int total { get; set; }
        public List<T> rows { get; set; }
    }
}
