using Core.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Mini.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string product_code { get; set; }
        public string product_name { get; set; }
        public string qty { get; set; }
        public string price { get; set; }
        public string merchant_code { get; set; }
    }
}
