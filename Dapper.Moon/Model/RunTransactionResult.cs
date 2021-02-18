using System;

namespace Dapper.Moon
{
    public class RunTransactionResult
    {
        public bool Success { get; set; }
        public Exception Exception { get; set; }
    }
}
