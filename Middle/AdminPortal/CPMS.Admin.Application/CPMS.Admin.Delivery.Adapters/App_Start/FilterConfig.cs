﻿using System.Web;
using System.Web.Mvc;

namespace CPMS.Admin.Delivery.Adapters
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
