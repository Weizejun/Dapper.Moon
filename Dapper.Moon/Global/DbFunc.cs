using System;

namespace Dapper.Moon
{
    /// <summary>
    /// 数据库中Function
    /// </summary>
    public partial class DbFunc
    {
        public static TResult Sum<TResult>(TResult val) { return default(TResult); }
        public static TResult Max<TResult>(TResult val) { return default(TResult); }
        public static TResult Min<TResult>(TResult val) { return default(TResult); }
        public static TResult Avg<TResult>(TResult val) { return default(TResult); }
        public static TResult Count<TResult>(TResult val) { return default(TResult); }
        public static int Datediff_Year(DateTime field, DateTime date) { return 0; }
        public static int Datediff_Month(DateTime field, DateTime date) { return 0; }
        public static int Datediff_Day(DateTime field, DateTime date) { return 0; }
        public static int Datediff_Hour(DateTime field, DateTime date) { return 0; }
        public static int Datediff_Hour(DateTime field, string date) { return 0; }
        public static bool Between(object field, object a, object b) { return true; }
        public static TResult IsNull<TResult>(object field, TResult val) { return default(TResult); }
        public static string Sequence(string name) { return null; }
    }

    #region Func
    public class MoonFunc<T1, T2>
    {
        public MoonFunc(T1 t1, T2 t2)
        {
            this.t1 = t1;
            this.t2 = t2;
        }

        public T1 t1 { get; }
        public T2 t2 { get; }
    }

    public class MoonFunc<T1, T2, T3>
    {
        public MoonFunc(T1 t1, T2 t2, T3 t3)
        {
            this.t1 = t1;
            this.t2 = t2;
            this.t3 = t3;
        }

        public T1 t1 { get; }
        public T2 t2 { get; }
        public T3 t3 { get; }
    }

    public class MoonFunc<T1, T2, T3, T4>
    {
        public MoonFunc(T1 t1, T2 t2, T3 t3, T4 t4)
        {
            this.t1 = t1;
            this.t2 = t2;
            this.t3 = t3;
            this.t4 = t4;
        }

        public T1 t1 { get; }
        public T2 t2 { get; }
        public T3 t3 { get; }
        public T4 t4 { get; }
    }

    public class MoonFunc<T1, T2, T3, T4, T5>
    {
        public MoonFunc(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        {
            this.t1 = t1;
            this.t2 = t2;
            this.t3 = t3;
            this.t4 = t4;
            this.t5 = t5;
        }

        public T1 t1 { get; }
        public T2 t2 { get; }
        public T3 t3 { get; }
        public T4 t4 { get; }
        public T5 t5 { get; }
    }

    public class MoonFunc<T1, T2, T3, T4, T5, T6>
    {
        public MoonFunc(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
        {
            this.t1 = t1;
            this.t2 = t2;
            this.t3 = t3;
            this.t4 = t4;
            this.t5 = t5;
            this.t6 = t6;
        }

        public T1 t1 { get; }
        public T2 t2 { get; }
        public T3 t3 { get; }
        public T4 t4 { get; }
        public T5 t5 { get; }
        public T6 t6 { get; }
    }
    #endregion Func

    public enum DbType
    {
        SqlServer,
        MySql,
        Oracle
    }

    public enum OrderBy
    {
        Asc,
        Desc
    }
}
