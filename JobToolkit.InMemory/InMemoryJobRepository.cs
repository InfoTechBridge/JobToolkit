using AnyCache.Core;
using AnyCache.InMemory;
using JobToolkit.AnyCache;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobToolkit.InMemory
{
    public class InMemoryJobRepository : AnyCacheJobRepository
    {
        public InMemoryJobRepository() 
            : base(new InMemoryCache())
        {
        }
    }
}
