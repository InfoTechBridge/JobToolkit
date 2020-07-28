using AnyCache.Redis;
using AnyCache.Serialization;
using JobToolkit.AnyCache;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobToolkit.Redis
{
    public class RedisJobRepository : AnyCacheJobRepository
    {
        public RedisJobRepository(string connectionString = null, int dbId = -1, string keyPrefix = "jobToolkit", ISerializer serializer = null)
            : base(new RedisCache(connectionString, dbId, keyPrefix, serializer ?? new BinarySerializer()))
        {
        }
    }
}
