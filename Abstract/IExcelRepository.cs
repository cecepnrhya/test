using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mini.Abstract
{
    public interface IExcelRepository<T> where T : class
    {
        Task<IEnumerable<T>> Get(string Path, string HeaderCode, string HeaderName, string HeaderQty, string HeaderPrice, bool SyncAll);
    }
}
