using EDL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class MedicamentoDAL
    {
        private readonly string cadenaConexion =
            ConfigurationManager.ConnectionStrings["cnn"].ConnectionString;

        public List<Medicamento> Listar()
        {
            List<Medicamento> medicamentos = new List<Medicamento>();

            string consulta = @"
                SELECT id_medicamento, nombre, descripcion, costo_unitario
                FROM Medicamento
                ORDER BY nombre;";

            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                conexion.Open();

                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        medicamentos.Add(MapearMedicamento(lector));
                    }
                }
            }

            return medicamentos;
        }

        public List<Medicamento> Buscar(string filtro)
        {
            List<Medicamento> medicamentos = new List<Medicamento>();

            string consulta = @"
                SELECT id_medicamento, nombre, descripcion, costo_unitario
                FROM Medicamento
                WHERE nombre LIKE @patron
                   OR ISNULL(descripcion, '') LIKE @patron
                ORDER BY nombre;";

            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add("@patron", SqlDbType.VarChar, 202).Value =
                    "%" + (filtro ?? "").Trim() + "%";

                conexion.Open();

                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        medicamentos.Add(MapearMedicamento(lector));
                    }
                }
            }

            return medicamentos;
        }

        public Medicamento ObtenerPorId(int idMedicamento)
        {
            Medicamento medicamento = null;

            string consulta = @"
                SELECT id_medicamento, nombre, descripcion, costo_unitario
                FROM Medicamento
                WHERE id_medicamento = @idMedicamento;";

            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add("@idMedicamento", SqlDbType.Int).Value =
                    idMedicamento;

                conexion.Open();

                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    if (lector.Read())
                    {
                        medicamento = MapearMedicamento(lector);
                    }
                }
            }

            return medicamento;
        }

        public void Insertar(Medicamento medicamento)
        {
            string consulta = @"
                INSERT INTO Medicamento
                (nombre, descripcion, costo_unitario)
                VALUES
                (@nombre, @descripcion, @costoUnitario);";

            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add("@nombre", SqlDbType.VarChar, 100).Value =
                    medicamento.Nombre;

                comando.Parameters.Add("@descripcion", SqlDbType.VarChar, 300).Value =
                    string.IsNullOrWhiteSpace(medicamento.Descripcion)
                    ? (object)DBNull.Value
                    : medicamento.Descripcion;

                SqlParameter parametroCosto =
                    comando.Parameters.Add("@costoUnitario", SqlDbType.Decimal);

                parametroCosto.Precision = 10;
                parametroCosto.Scale = 2;
                parametroCosto.Value = medicamento.CostoUnitario;

                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        public void Actualizar(Medicamento medicamento)
        {
            string consulta = @"
                UPDATE Medicamento
                SET nombre = @nombre,
                    descripcion = @descripcion,
                    costo_unitario = @costoUnitario
                WHERE id_medicamento = @idMedicamento;";

            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add("@idMedicamento", SqlDbType.Int).Value =
                    medicamento.IdMedicamento;

                comando.Parameters.Add("@nombre", SqlDbType.VarChar, 100).Value =
                    medicamento.Nombre;

                comando.Parameters.Add("@descripcion", SqlDbType.VarChar, 300).Value =
                    string.IsNullOrWhiteSpace(medicamento.Descripcion)
                    ? (object)DBNull.Value
                    : medicamento.Descripcion;

                SqlParameter parametroCosto =
                    comando.Parameters.Add("@costoUnitario", SqlDbType.Decimal);

                parametroCosto.Precision = 10;
                parametroCosto.Scale = 2;
                parametroCosto.Value = medicamento.CostoUnitario;

                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        public void Eliminar(int idMedicamento)
        {
            string consulta = @"
                DELETE FROM Medicamento
                WHERE id_medicamento = @idMedicamento;";

            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add("@idMedicamento", SqlDbType.Int).Value =
                    idMedicamento;

                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        public void AgregarStock(int idHospital, int idMedicamento, int cantidad)
        {
            const string sql = @"
        UPDATE Inventario_Hospital WITH (UPDLOCK, SERIALIZABLE)
        SET cantidad_stock = cantidad_stock + @Cantidad
        WHERE id_hospital = @IdHospital
          AND id_medicamento = @IdMedicamento;

        IF @@ROWCOUNT = 0
        BEGIN
            INSERT INTO Inventario_Hospital
                (id_hospital, id_medicamento, cantidad_stock)
            VALUES
                (@IdHospital, @IdMedicamento, @Cantidad);
        END";

            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            using (SqlCommand comando = new SqlCommand(sql, conexion))
            {
                comando.Parameters.AddWithValue("@IdHospital", idHospital);
                comando.Parameters.AddWithValue("@IdMedicamento", idMedicamento);
                comando.Parameters.AddWithValue("@Cantidad", cantidad);

                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        public List<InventarioMedicamentoDetalle>
            ObtenerInventarioUltimos30Dias(int idHospital)
        {
            List<InventarioMedicamentoDetalle> inventario =
                new List<InventarioMedicamentoDetalle>();

            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            using (SqlCommand comando = new SqlCommand(
                "sp_InventarioYPrescripcionesUltimos30Dias",
                conexion))
            {
                comando.CommandType = CommandType.StoredProcedure;

                comando.Parameters.Add("@IdHospital", SqlDbType.Int).Value =
                    idHospital;

                conexion.Open();

                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        inventario.Add(new InventarioMedicamentoDetalle
                        {
                            IdMedicamento =
                                Convert.ToInt32(lector["id_medicamento"]),

                            Medicamento =
                                lector["medicamento"].ToString(),

                            StockActual =
                                Convert.ToInt32(lector["stock_actual"]),

                            TotalPrescrito30Dias =
                                Convert.ToInt32(
                                    lector["total_prescrito_30_dias"])
                        });
                    }
                }
            }

            return inventario;
        }

        private Medicamento MapearMedicamento(SqlDataReader lector)
        {
            return new Medicamento
            {
                IdMedicamento =
                    Convert.ToInt32(lector["id_medicamento"]),

                Nombre =
                    lector["nombre"].ToString(),

                Descripcion =
                    lector["descripcion"].ToString(),

                CostoUnitario =
                    Convert.ToDecimal(lector["costo_unitario"])
            };
        }
        public List<MedicamentoHospitalDetalle> BuscarPorHospital(
     int idHospital,
     string filtro)
        {
            List<MedicamentoHospitalDetalle> medicamentos =
                new List<MedicamentoHospitalDetalle>();

            string consulta = @"
                SELECT
                    m.id_medicamento,
                    m.nombre,
                    m.descripcion,
                    m.costo_unitario,
                    ISNULL(
                        STRING_AGG(h.nombre, ', '),
                        'Sin asignar'
                    ) AS nombre_hospital,
                    ISNULL(SUM(ih.cantidad_stock), 0) AS cantidad_stock
                FROM Medicamento m
                LEFT JOIN Inventario_Hospital ih
                    ON ih.id_medicamento = m.id_medicamento
                LEFT JOIN Hospital h
                    ON h.id_hospital = ih.id_hospital
                WHERE (@idHospital = 0 OR ih.id_hospital = @idHospital)
                  AND (
                        m.nombre LIKE @patron
                        OR ISNULL(m.descripcion, '') LIKE @patron
                  )
                GROUP BY
                    m.id_medicamento,
                    m.nombre,
                    m.descripcion,
                    m.costo_unitario
                ORDER BY m.nombre;";

            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add("@idHospital", SqlDbType.Int).Value =
                    idHospital;

                comando.Parameters.Add("@patron", SqlDbType.VarChar, 302).Value =
                    "%" + (filtro ?? "").Trim() + "%";

                conexion.Open();

                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        medicamentos.Add(new MedicamentoHospitalDetalle
                        {
                            IdMedicamento =
                                Convert.ToInt32(lector["id_medicamento"]),

                            Nombre = lector["nombre"].ToString(),

                            Descripcion = lector["descripcion"].ToString(),

                            CostoUnitario =
                                Convert.ToDecimal(lector["costo_unitario"]),

                            NombreHospital =
                                lector["nombre_hospital"].ToString(),

                            CantidadStock =
                                Convert.ToInt32(lector["cantidad_stock"])
                        });
                    }
                }
            }

            return medicamentos;
        }
    }
}