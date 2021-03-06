﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Moon
{
    public class OracleDialect : SqlDialectProvide
    {
        public override string ParameterPrefix
        {
            get { return ":"; }
        }

        public override string SetSqlName(string name)
        {
            return $"\"{name.ToUpper()}\"";
        }

        public override string GetDate { get { return "sysdate"; } }

        public override string Datediff(string dateType, string column, string dt)
        {
            switch (dateType)
            {
                case "Year":
                    return $"trunc(months_between({column},to_date({dt},'yyyy-MM-dd HH24:mi:ss') / 12))";
                case "Month":
                    return $"trunc(months_between({column},to_date({dt},'yyyy-MM-dd HH24:mi:ss')))";
                case "Day":
                    return $"ceil(({column} - to_date({dt},'yyyy-MM-dd HH24:mi:ss')))";
                case "Hour":
                    return $"ceil(({column} - to_date({dt},'yyyy-MM-dd HH24:mi:ss')) * 24)";
            }
            return "";
        }

        public override string Concat(string column, string val)
        {
            return $"({column}||{val})";
        }

        public override string ToDateTime(string field)
        {
            return $"to_date({field},'yyyy-MM-dd HH24:mi:ss')";
        }

        public override string IsNull(object column, object val)
        {
            if (val == null || val.ToString() == "") val = "''";
            return $"nvl({column},{val})";
        }

        public override string Guid { get { return "rawtohex(sys_guid())"; } }

        public override string Length(string field) { return $"length({field})"; }

        public override string IndexOf(string field, object val)
        {
            return $"instr({field}, {val}, 1, 1) - 1";
        }

        public override string Year(string field)
        {
            return $"extract(year from {field})";
        }

        public override string Month(string field)
        {
            return $"extract(month from {field})";
        }

        public override string Day(string field)
        {
            return $"extract(day from {field})";
        }

        public override string Hour(string field)
        {
            return $"cast(to_char({field},'HH24') as number)";
        }
    }
}
