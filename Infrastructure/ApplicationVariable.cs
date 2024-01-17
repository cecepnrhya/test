using System.Collections.Generic;

namespace Mini.Infrastructure
{
    public static class ApplicationVariable
    {
        static string _MerchantCode;
        public static string MerchantCode
        {
            get
            {
                return _MerchantCode;
            }
            set
            {
                _MerchantCode = value;
            }
        }
    }
}
