using System.Web.Mvc;

namespace CPMS.Admin.Delivery.Adapters.App_Start
{
    public static class ValueProviderFactoriesConfig
    {
        public static void Register()
        {
            ValueProviderFactories.Factories.Add(new JsonValueProviderFactory());
        }
    }
}