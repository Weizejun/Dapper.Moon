﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Moon
{
    public class MySqlUpdateable<T> : UpdateableProvide<T>
    {
        public MySqlUpdateable(IRepository _Repository)
            : base(_Repository) { }
    }
}
