﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mini.Abstract
{
    public interface IPsqlRepository<T> where T : class
    {
        Task<IEnumerable<T>> Get(string ConString, string Query, bool SyncAll);
    }
}
