using DAL;
using EDL;

namespace BLL
{
    public class DashboardBLL
    {
        private readonly DashboardDAL dashboardDAL =
            new DashboardDAL();

        public ResumenAdministrador ObtenerResumen()
        {
            return dashboardDAL.ObtenerResumen();
        }
    }
}