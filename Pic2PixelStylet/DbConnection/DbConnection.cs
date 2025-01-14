using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace Pic2PixelStylet.DbConnection
{
    internal class DbConnection
    {
        private const string connectionString = "datasource=pic2pixels.db;";
        public static SqlSugarClient Db;

        static DbConnection()
        {
            Db = new SqlSugarClient(
                new ConnectionConfig()
                {
                    ConnectionString = connectionString,
                    DbType = DbType.Sqlite,
                    IsAutoCloseConnection = true,
                },
                db =>
                {
                    db.Aop.OnLogExecuting = (sql, pars) =>
                    {
                        Console.WriteLine(UtilMethods.GetNativeSql(sql, pars));
                    };
                }
            );
        }
    }
}
