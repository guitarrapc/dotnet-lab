using BlazorAppEF.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BlazorAppEF
{
    public static class DbConnectionMonitor
    {
        private static readonly Type contextType = typeof(DbContext);
        private static readonly FieldInfo _dbContextPoolFiled = contextType.GetField("_dbContextPool", BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly Type poolType = typeof(Microsoft.EntityFrameworkCore.Internal.DbContextPool<BloggingContext>);
        private static readonly FieldInfo _poolCountFiled = poolType.GetField("_count", BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance);

        public static (bool isPoolContext, int poolCount) GetConnectionPoolCount(DbContext instance)
        {
            var poolInstance = _dbContextPoolFiled.GetValue(instance);
            if (poolInstance == null)
            {
                return (true, 0);
            }
            else
            {
                var pool = _poolCountFiled.GetValue(poolInstance);
                return (true, (int)pool);
            }
        }
    }
}
