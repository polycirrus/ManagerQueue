using System.Web;
using System.Web.Mvc;

namespace BSUIR.ManagerQueue.Service
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
