using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Moon.Test
{
    public class SqlServerTest
    {
        //public static string connectionString = "server=192.168.101.21;uid=sa;pwd=ccn123.;database=TDC_DB";
        public static string connectionString = "server=.;uid=sa;pwd=123456;database=moon.cms.v2";

        #region DapperMoonFactory
        private class DapperMoonFactory
        {
            private DapperMoonFactory() { }

            public static DapperMoon Create()
            {
                DapperMoon dm = DapperMoon.Create(connectionString);
                dm.OnExecuting = (e) =>
                {
                    Console.WriteLine(e.Sql);
                    Console.WriteLine(e.DynamicParameters?.RawString());
                };
                return dm;
            }
        }
        #endregion DapperMoonFactory

        #region script
        /*
CREATE TABLE t_moon_user  (
  Id varchar(32) PRIMARY KEY,
  Account varchar(100),
  Password varchar(100),
  NickName varchar(100),
  Mobile varchar(32),
  Email varchar(32),
  Expire varchar(32),
  Flag int,
  Icon varchar(255),
  UserType int,
  CreateDate datetime,
  CreatorId varchar(50),
  ModifyDate datetime,
  Modifier varchar(50),
  Version varchar(50)
);

CREATE TABLE t_moon_user_role  (
  Id int IDENTITY PRIMARY KEY,
  UserId varchar(32),
  RoleId varchar(32)
);

CREATE TABLE t_moon_role  (
  Id int IDENTITY PRIMARY KEY,
  RoleCode varchar(50),
  RoleName varchar(50)
);        

CREATE PROCEDURE [dbo].[Proc_Test]
	@code varchar(20),
	@name varchar(20) output
AS
BEGIN
	set @name = 'sqlserver proc';
	select 'sqlserver dapper.moon';

	select top 10 * from t_moon_user;
END
        */
        #endregion script

        public void BulkInsertTest()
        {
            using (DapperMoon dm = DapperMoonFactory.Create())
            {
                List<User> list = Enumerable.Range(0, 300).Select(i => new User()
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Account = "moon_" + i,
                    NickName = "moon_nick" + i,
                    Password = MD5To($"moon_{i}"),
                    Email = $"1489755444{i}@qq.com",
                    Mobile = $"1489755444{i}",
                    CreateDate = DateTime.Now,
                    CreatorId = "",
                    Expire = DateTime.Now.ToShortDateString(),
                    //Flag = 1,
                    Modifier = "",
                    //ModifyDate = DateTime.Now,
                    Version = null
                }).ToList();

                var result = dm.RunTransaction(() =>
                {
                    Stopwatch watch = new Stopwatch();
                    watch.Start();
                    dm.Insertable<User>(list).BulkInsert();
                    watch.Stop();
                    Console.WriteLine("耗时:" + watch.ElapsedMilliseconds + " 毫秒");
                });
                Console.WriteLine($"BulkInsertTest={result.Success}," + result.Exception?.Message);
            }
        }

        public void InsertTest()
        {
            using (DapperMoon dm = DapperMoonFactory.Create())
            {
                List<User> list = Enumerable.Range(0, 10).Select(i => new User()
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Account = "moon_" + i,
                    NickName = "moon_nick" + i,
                    Password = MD5To($"moon_{i}"),
                    Email = $"1489755444{i}@qq.com",
                    Mobile = $"1489755444{i}",
                    CreateDate = DateTime.Now,
                    CreatorId = "",
                    Expire = DateTime.Now.ToShortDateString(),
                    //Flag = 1,
                    Modifier = "",
                    //ModifyDate = DateTime.Now,
                    Version = null
                }).ToList();

                dm.Insertable<User>(list).Execute();

                dm.RunTransaction(() =>
                {
                    dm.Insertable<User>(new User()
                    {
                        Id = Guid.NewGuid().ToString("N"),
                        Account = "moon_",
                        NickName = "moon_nick",
                        Password = MD5To($"moon_"),
                        Email = $"1489755444@qq.com",
                        Mobile = $"1489755444",
                        CreateDate = DateTime.Now,
                        CreatorId = "",
                        Expire = DateTime.Now.ToShortDateString(),
                        Flag = 1,
                        Modifier = "moon",
                        ModifyDate = DateTime.Now,
                        Version = null
                    }).Execute();
                });

                long id = dm.Insertable<Role>(new Role()
                {
                    RoleCode = "Super",
                    RoleName = "Administrator"
                }).ExecuteIdentity();
                Console.WriteLine("result id = " + id);

                List<UserRole> userRoles = new List<UserRole>();
                var result2 = dm.Queryable<User>().Where(i => true).Take(100).ToList();
                foreach (var item in result2)
                {
                    userRoles.Add(new UserRole()
                    {
                        RoleId = Convert.ToInt32(id),
                        UserId = item.Id
                    });
                }
                dm.Insertable<UserRole>(userRoles).Execute();
            }
        }

        public void UpdateTest()
        {
            using (DapperMoon dm = DapperMoonFactory.Create())
            {
                User user = dm.Queryable<User>().Where(i => true).First();
                user.Account = "update...";
                user.ModifyDate = DateTime.Now;
                user.Modifier = "junz.wei";

                var ids = dm.Queryable<User>().Where(i => true).Take(10).ToList().Select(i=>i.Id).ToArray();

                dm.Updateable<User>().SetColumns("Flag=@flag,UserType=@usertype",
                    new { flag = 1, usertype = 1 }).Where(i => i.Id == user.Id).Execute();

                dm.Updateable<User>(user)
                .SetColumns(i => new { i.Account, i.ModifyDate })
                //in
                .Where(i => ids.Contains(i.Id)).Execute();

                dm.Updateable<User>(user)
                .SetColumns(i => new { i.Account, i.ModifyDate })
                //in
                .Where(i => new[] {
                "009319aea728439fb249b68c0d2074aa",
                "010c85bd3e7342fd99fd32e7490d4154",
                "011c9df0b2b247468e33fae649ed156f" }.Contains(i.Id)).Execute();

                dm.Updateable<User>(user)
                        .IgnoreColumns(i => new
                        {
                            i.Expire,
                            i.Flag,
                            i.Icon,
                            i.UserType,
                            i.Version
                        })
                        .Where(i => i.Id == user.Id).Execute();

                dm.Updateable<User>()
               .SetColumns(i => i.Flag + 1)//Flag = Flag + 1
                .SetColumns(i => i.Account == "administrator")
                .SetColumns(i => i.NickName == "junz.wei")
                .Where(i => i.Id == user.Id).Execute();
            }
        }

        public void DeleteTest()
        {
            using (DapperMoon dm = DapperMoonFactory.Create())
            {
                RunTransactionResult result = dm.RunTransaction(() =>
                {
                    dm.Deleteable<User>().Where(i => i.Id == "a").Execute();

                    dm.Deleteable<User>(new string[] { "a", "b", "c" }).Execute();

                    dm.Deleteable<User>().Where(i =>
                        i.Account.Contains("nick") &&
                       !i.NickName.Contains("nick")).Execute();

                    dm.Deleteable<User>().Where(i => i.Account.StartsWith("nick")).Execute();

                    dm.Deleteable<User>().Where(i => i.Account.EndsWith("nick")).Execute();

                    dm.Deleteable<User>().Where(i => new[] {
                        "009319aea728439fb249b68c0d2074aaX",
                        "010c85bd3e7342fd99fd32e7490d4154X",
                        "011c9df0b2b247468e33fae649ed156fX" }.Contains(i.Id)).Execute();
                });
            }
        }

        public void ProcTest()
        {
            using (DapperMoon dm = DapperMoonFactory.Create())
            {
                SqlServerDynamicParameters parameters = new SqlServerDynamicParameters();
                parameters.Add("code", "");
                parameters.Add("name",
                    dbType: System.Data.SqlDbType.VarChar,
                    size: 100,
                    direction: System.Data.ParameterDirection.Output);

                var ds = dm.Repository.ExecuteDataSet("Proc_Test",
                    parameters, System.Data.CommandType.StoredProcedure);
                object outputVal = parameters.Get("name");


                parameters.Clear();

                parameters.Add("account", "administrator");
                var ds2 = dm.Repository.ExecuteDataSet(
                    @"select count(1) from t_moon_user where account=@account;
                            select * from t_moon_user where account = @account", parameters);
            }
        }

        public void TransactionTest()
        {
            using (TransactionFactory tran = TransactionFactory.Create(new DapperMoon[] {
                                DapperMoon.Create(connectionString),
                                DapperMoon.Create(MySqlTest.connectionString,DbType.MySql),
                                DapperMoon.Create(OracleTest.connectionString,DbType.Oracle)
                            }))
            {
                int c = DateTime.Now.Minute;

                tran.ChangeDatabase(0).OnExecuting = (e) =>
                {
                    Console.WriteLine("database 1");
                    Console.WriteLine(e.Sql);
                    Console.WriteLine(e.DynamicParameters?.RawString());
                };

                tran.ChangeDatabase(1).OnExecuting = (e) =>
                {
                    Console.WriteLine("database 2");
                    Console.WriteLine(e.Sql);
                    Console.WriteLine(e.DynamicParameters?.RawString());
                };

                tran.ChangeDatabase(2).OnExecuting = (e) =>
                {
                    Console.WriteLine("database 3");
                    Console.WriteLine(e.Sql);
                    Console.WriteLine(e.DynamicParameters?.RawString());
                };

                try
                {
                    tran.BeginTransaction();

                    tran.ChangeDatabase(0).Insertable<Role>(new Role()
                    {
                        RoleCode = $"Super{c}",
                        RoleName = $"Administrator{c}"
                    }).Execute();

                    tran.ChangeDatabase(1).Insertable<Role>(new Role()
                    {
                        RoleCode = $"Super{c}",
                        RoleName = $"Administrator{c}"
                    }).Execute();

                    tran.ChangeDatabase(2).Insertable<Role>(new Role()
                    {
                        RoleCode = $"Super{c}",
                        RoleName = $"Administrator{c}"
                    }).Execute();

                    tran.CommitTransaction();
                }
                catch (Exception ex)
                {
                    tran.RollbackTransaction();
                }

                RunTransactionResult result = tran.RunTransaction(() =>
               {
                   c++;
                   tran.ChangeDatabase(0).Insertable<Role>(new Role()
                   {
                       RoleCode = $"Super{c}",
                       RoleName = $"Administrator{c}"
                   }).Execute();

                   tran.ChangeDatabase(1).Insertable<Role>(new Role()
                   {
                       RoleCode = $"Super{c}",
                       RoleName = $"Administrator{c}"
                   }).Execute();

                   tran.ChangeDatabase(2).Insertable<Role>(new Role()
                   {
                       RoleCode = $"Super{c}",
                       RoleName = $"Administrator{c}"
                   }).Execute();
               });
            }
        }

        public void DapperTest()
        {
            using (DapperMoon dm = DapperMoonFactory.Create())
            {
                string sql = @"select count(1) from t_moon_user where account=@account;
                            select * from t_moon_user where account=@account";
                var param = new { account = "administrator" };

                using (var query = dm.Repository.Connection.QueryMultiple(sql, param))
                {
                    int total = query.ReadFirstOrDefault<int>();
                    var rows = query.Read<User>().ToList();
                }
            }
        }

        public void QueryTest()
        {
            using (DapperMoon dm = DapperMoonFactory.Create())
            {
//                DateTime beginDate1 = DateTime.Parse("2021-02-14 23:58:48");
//                DateTime endDate2 = DateTime.Parse("2021-02-14 23:59:59");

//                var resultx2= dm.Queryable<User>().Where(
//                     i => 
//DbFunc.Between(i.CreateDate, beginDate1, endDate2)
//&& DbFunc.Datediff_Hour(i.CreateDate, beginDate1) < 24 
//&& DbFunc.ToDateTime(i.ExpireX2) >= DateTime.Now
//                     ).Select(i=> new {
//                         //CreateDate Expire 小时差
//                         XXX = DbFunc.Datediff_Hour(i.CreateDate, i.Expire)
//                     })
//                    .Take(10).ToList();

                //var result11 = dm.Queryable<User>().Where(
                // i => DbFunc.Between(i.CreateDate, "2021-02-14 23:58:48", "2021-02-14 23:59:59"))
                //.Take(10).ToList();

                var resultX = dm.Queryable<User>().Where(i =>
                  i.CreateDate == DateTime.Now &&
                  i.CreateDate.Year == 2021 || i.CreateDate.Hour == 12
              ).Select(i => new
              {
                  x2 = DateTime.Now
              }).ToList();

                var result = dm.Queryable<User>().Where(i =>
                    i.Account.IndexOf("a") == -1 &&
                    i.Account.Replace("a", "c") == "x" &&
                    string.Concat(i.Account, "x") == "a" &&
                    i.Account != null
                ).Select(i => new
                {
                    x2 = i.Account.Replace("a", "c"),
                    x3 = string.Concat(i.Account, "x"),
                    x4 = i.Account.ToUpper(),
                    x6 = i.Account.TrimEnd(),
                    x7 = i.Account.TrimStart(),
                    x8 = i.Account.Substring(2, 3)
                }).ToSql().Sql;//.ToList();

                //select * from [t_moon_user] where ([Account] is not null or [Account] <> '' )
                var result0 = dm.Queryable<User>().Where(i => !string.IsNullOrEmpty(i.Account)).ToSql()?.Sql;
                //in
                var result0_1 = dm.Queryable<User>().Where(i => new[] { "a","b","c" }.Contains(i.Id)).ToList();

                var result0_2 = dm.Queryable<User>()
                .Select(n => new
                {
                    //CreateDate Expire 小时差
                    XXX = DbFunc.Datediff_Hour(n.CreateDate, n.Expire)
                }).ToList<dynamic>();
                //in
                var result0_3 = dm.Queryable<User>().Where(i => new string[] { "a1", "b1", "c1" }.Contains(i.Id)).ToList();

                //len(account) = 10
                var result0_4 = dm.Queryable<User>().Where(i => i.Account.Length == 10).ToSql()?.Sql;

                //Account is not null or Account <> ''
                var result1 = dm.Queryable<User>()
                .Where(i => !string.IsNullOrEmpty(i.Account)).First();

                //NickName not like '%%'
                dm.Queryable<User>()
                    .Where(i => !i.NickName.Contains("administrator")
                    ).First();

                //Account is null or Account = ''
                var result2 = dm.Queryable<User>()
                .TableName("t_moon_user")
                .Where(i => string.IsNullOrEmpty(i.Account)).First<UserDto>();

                //select * from [t_moon_user] where (([Account] is null) or ([Account] is not null))
                var result2_1 = dm.Queryable<User>()
                .TableName("t_moon_user")
                .Where(i => i.Account == null || i.Account != null).First<UserDto>();

                var result3 = dm.Queryable<User, UserRole, Role>()
                //动态表名 例如：t_moon_user2021
                .TableName("t_moon_user", "t_moon_user_role", "t_moon_role")
                .LeftJoin(n => n.t1.Id == n.t2.UserId)
                .LeftJoin(n => n.t3.Id == n.t2.RoleId)
                .OrderBy(n => n.t1.CreateDate).Distinct()
                .Where(n => n.t3.RoleCode == "Super"
                ).Select(n => new
                {
                    n.t1.Id,
                    n.t1.Account,
                    n.t1.NickName,
                    n.t3.RoleCode,
                    n.t3.RoleName,
                    n.t1.CreateDate
                }).ToPageList<UserDto>(0, 10);

                var result3_2 = dm.Queryable<User, UserRole, Role>()
                    .UseSqlServer(new SqlServerOption(useRowNumberForPaging:true))//启用RowNumber分页
                .TableName("t_moon_user", "t_moon_user_role", "t_moon_role")
                .LeftJoin(n => n.t1.Id == n.t2.UserId)
                .LeftJoin(n => n.t3.Id == n.t2.RoleId)
                .OrderBy(n => n.t1.CreateDate)
                .Where(n => n.t3.RoleCode == "Super"
                ).Select(n => new
                {
                    n.t1.Id,
                    n.t1.Account,
                    n.t1.NickName,
                    n.t3.RoleCode,
                    n.t3.RoleName
                }).ToPageList<UserDto>(0, 10);

                var result4 = dm.Queryable<User, UserRole, Role>()
                .LeftJoin(n => n.t1.Id == n.t2.UserId)
                .LeftJoin(n => n.t3.Id == n.t2.RoleId)
                .OrderBy(n => n.t1.CreateDate)
                .Where(n => n.t3.RoleCode == "Super"
                ).Select(n => new
                {
                    n.t1,//t1.*
                    n.t3 //t3.*
                }).Take(10).ToList<UserDto>();

                //
                var result5 = dm.Queryable<User>()
                    .Where(i => i.Flag != null && string.IsNullOrEmpty(i.Icon)
                    && !string.IsNullOrEmpty(i.NickName)
                    ).Select(i => new
                    {
                        //别名
                        x1 = string.Concat(DbFunc.IsNull(i.Icon, ""), "这是1个很奇怪的值"),
                        Id = Guid.NewGuid()
                    }).ToList<UserDto>();

                var result6 = dm.Queryable<User>().Where(
                    i => i.NickName.EndsWith("i") || i.NickName.StartsWith("j")
                    && DbFunc.IsNull(i.Icon, "") == "abc")
                  .Take(10).OrderBy(i => i.CreateDate, OrderBy.Desc).ToList();

                var result7 = dm.Queryable<User, UserRole>()
                .LeftJoin(n => n.t1.Id == n.t2.UserId)
              .OrderBy(n => n.t1.Id)
              .Select(n => new { n.t1.Id, n.t1.Account })
              .GroupBy(n => new { n.t1.Id, n.t1.Account }).ToPageList<UserDto>(0, 10);

                var result8 = dm.Queryable<User, UserRole>()
                .LeftJoin(n => n.t1.Id == n.t2.UserId)
                .OrderBy(n => n.t1.Id)
                .Select(n => new
                {
                    Id = Guid.NewGuid(),//newid()
                    Account = string.Concat(n.t1.Account, "XXX1"),
                    NickName = string.Concat(n.t1.NickName, "XXX2"),
                    CreateDate = DateTime.Now // getdate()
                })
                .ToPageList<UserDto>(0, 10);

                var result9 = dm.Queryable<User>().Where(i =>
                i.NickName.StartsWith("j") || i.NickName.EndsWith("i"))
               .Take(10)
               .Select(i => new
               {
                   Id = Guid.NewGuid(),//newid()
                   Account = string.Concat(i.Account, "XXX1"),
                   NickName = string.Concat(i.NickName, "XXX2"),
                   CreateDate = DateTime.Now // getdate()
               }).Into<User>(i => new
               {
                   i.Id,
                   i.Account,
                   i.NickName,
                   i.CreateDate
               });

                DateTime beginDate = DateTime.Parse("2021-02-14 23:58:48");
                DateTime endDate = DateTime.Parse("2021-02-14 23:59:59");

                var result10 = dm.Queryable<User>().Where(
                     i => DbFunc.Between(i.CreateDate, beginDate, endDate))
                    .Take(10).ToList();

                var result11 = dm.Queryable<User>().Where(
                 i => DbFunc.Between(i.CreateDate, "2021-02-14 23:58:48", "2021-02-14 23:59:59"))
                .Take(10).ToList();

                var result12 = dm.Queryable<User>().Where(i => true).Count(i => i.Id);

                var result13 = dm.Queryable<User>().Max<int?>(i => i.Flag);

                //max(isnull([Flag],1))
                var result13_2 = dm.Queryable<User>().Max<int?>(i => DbFunc.IsNull(i.Flag, 1));

                var result13_3 = dm.Queryable<User>().Max<int?>(i => DbFunc.IsNull(i.Flag, 1) + 5);

                // datediff(HOUR, CreateDate, GETDATE()) < 24
                var result14 = dm.Queryable<User>()
                    .Where(i => DbFunc.Datediff_Hour(i.CreateDate, beginDate) < 24)
                    .Select(i => new
                    {
                        i.Account,
                        x1 = DbFunc.Count(i.Id)
                    }).GroupBy(i => new
                    {
                        i.Account,
                        i.Id
                    }).OrderBy(i => i.Id).ToList();

                //DateTime.Now = getdate() now()
                var result15 = dm.Queryable<User>()
                   .Where(i => DbFunc.ToDateTime(i.ExpireX2) >= DateTime.Now)
                   .ToList();

                //dm.Updateable<User>().SetColumns("Expire='2021/2/15'").Where(i => true).Execute();

                //var result16 = dm.Queryable<User>()
                //   .Where(i => DbFunc.Concat(i.Expire, " 23:59:59") >= DateTime.Now)
                //   .First();

                var result17 = dm.Queryable<User>().Where("account=@account", new { account = "moon" })
                    .Select("*").First();

                var result19 = dm.Queryable<User, UserRole, Role>()
                     .LeftJoin(n => n.t1.Id == n.t2.UserId)
                     .LeftJoin(n => n.t3.Id == n.t2.RoleId)
                     .OrderBy(n => n.t1.CreateDate)
                     .Take(10)
                     .Where(n => n.t3.RoleCode == "Super"
                     ).Select(n => new
                     {
                         n.t1,//t1.*
                         x1 = n.t3.Id //别名
                     }).Take(10).ToList<UserDto>();

                var result20 = dm.Queryable<User>()
                  .Where(i => i.CreateDate >= DbFunc.ToDateTime("2021-02-14 23:59:59"))
                  .ToList();

                DataTable dataTable = dm.Repository.Query("select * from t_moon_user where account = @account",
                     new { account = "administrator" });
            }
        }

        public void LambdaQueryTest()
        {
            using (DapperMoon dm = DapperMoonFactory.Create())
            {
                UserDto queryDto = new UserDto()
                {
                    Account = "admin",
                    NickName = "junz"
                };

                Expression<Func<User, bool>> expression = t => t.UserType == null;

                expression = expression.And(t => t.Account.Contains(queryDto.Account),
                   !string.IsNullOrWhiteSpace(queryDto.Account));

                expression = expression.And(t => t.NickName.Contains(queryDto.NickName),
                    !string.IsNullOrWhiteSpace(queryDto.NickName));

                expression = expression.Or(i => new[] {
                    "009319aea728439fb249b68c0d2074aa",
                    "010c85bd3e7342fd99fd32e7490d4154",
                    "011c9df0b2b247468e33fae649ed156f" }.Contains(i.Id));

                var result1 = dm.Queryable<User>().Where(expression).First();

                Expression<Func<MoonFunc<User, UserRole, Role>, bool>> expression2 = t => true;

                expression2 = expression2.And(n => n.t1.Account.Contains(queryDto.Account),
                    !string.IsNullOrWhiteSpace(queryDto.Account));

                expression2 = expression2.And(n => n.t1.NickName.Contains(queryDto.NickName),
                    !string.IsNullOrWhiteSpace(queryDto.NickName));

                expression2 = expression2.Or(n => new[] { "a","b","c" }.Contains(n.t1.Id));

                var result2 = dm.Queryable<User, UserRole, Role>()
               .LeftJoin(n => n.t1.Id == n.t2.UserId)
               .LeftJoin(n => n.t3.Id == n.t2.RoleId)
               .OrderBy(n => n.t1.CreateDate)
               .Where(expression2).Select(n => new
               {
                   n.t1,//t1.*
                   n.t3 //t3.*
                }).Take(10).ToList<UserDto>();
            }
        }

        public void DbFirstTest()
        {
            using (DapperMoon dm = DapperMoonFactory.Create())
            {
                string dire = AppDomain.CurrentDomain.BaseDirectory + "Model";
                dm.DbFirst().Builder(new DbFirstOption()
                {
                    Namespace = "model",
                    SaveFolder = dire,
                    Tables = new string[] { "t_moon_user" }
                });
            }
        }

        string MD5To(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return BitConverter.ToString(MD5.Create().ComputeHash(Encoding.Default.GetBytes(s))).Replace("X", "").Replace("-", "");
        }
    }
}
