using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Moon
{
    public class SqlServerDeleteable<T> : DeleteableProvide<T>
    {
        public SqlServerDeleteable(IRepository _Repository)
            : base(_Repository) { }
    }
}
