using Core.Abstract;
using Mini.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mini.Abstract
{
    public interface IProductStore<T> where T: class
    {
        IQueryable<T> Products { get; }
        Task<IEnumerable<T>> UpdateAsync(IEnumerable<T> entities, bool SyncAll);
    }
}
