using DAL;
using EDL;
using System;

namespace BLL
{
    public class ResumenJornadaMedicoBLL
    {
        private readonly ResumenJornadaMedicoDAL resumenDAL =
            new ResumenJornadaMedicoDAL();

        public ResumenJornadaMedico ObtenerResumen(int idUsuario)
        {
            if (idUsuario <= 0)
            {
                throw new ArgumentException(
                    "El usuario no es válido.");
            }

            return resumenDAL.ObtenerResumen(idUsuario);
        }
    }
}