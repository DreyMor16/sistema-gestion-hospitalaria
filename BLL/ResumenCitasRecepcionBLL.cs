using DAL;
using EDL;

namespace BLL
{
    public class ResumenCitasRecepcionBLL
    {
        private readonly ResumenCitasRecepcionDAL resumenDAL =
            new ResumenCitasRecepcionDAL();

        public ResumenCitasRecepcion ObtenerResumen()
        {
            return resumenDAL.ObtenerResumen();
        }
    }
}