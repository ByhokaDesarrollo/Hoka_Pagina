
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

using hoka.Permisos;
using hoka.Models;
using System.Web.WebPages;
using System.Reflection;

namespace hoka.Controllers
{
    public class MainController : Controller
    {

        private static string DBHoka = ConfigurationManager.ConnectionStrings["CadenaConexionHokaCompuadmo"].ToString();
        private static string DBJoy = ConfigurationManager.ConnectionStrings["CadenaConexionHokaJoyeria"].ToString();

        private static string DBHokaTEST = ConfigurationManager.ConnectionStrings["CadenaConexionPruebaHokaCompuadmo"].ToString();
        private static string DBJoyTEST = ConfigurationManager.ConnectionStrings["CadenaConexionPruebaHokaJoyeria"].ToString();

        private static bool conexionDBDEV = ConfigurationManager.AppSettings["ENV_DB_DEV"].AsBool();

        private static string conexionDBHoka;
        private static string conexionDBJoy;

        public MainController()
        {
            if (conexionDBDEV)
            {
                conexionDBHoka = DBHokaTEST;
                conexionDBJoy = DBJoyTEST;
            }
            else
            {
                conexionDBHoka = DBHoka;
                conexionDBJoy = DBJoy;
            }
        }

        [HttpPost]
        public void updateStatusForSales()
        {

            int Estado = 0;
            int IdRegistroVentas = 0;
            foreach (string key in Request.Form.AllKeys)
            {
                if (key.StartsWith("Estado"))
                {
                    if (Request.Form[key] != "")
                        Estado = Convert.ToInt32(Request.Form[key]);
                }

                if (key.StartsWith("IdRegistroVentas"))
                {
                    if (Request.Form[key] != "")
                        IdRegistroVentas = Convert.ToInt32(Request.Form[key]);
                }

            }

            using (SqlConnection con = new SqlConnection(conexionDBHoka))
            {
                string baseQuery = @"UPDATE ingresos.dbo.RegistroVentasCategorias  SET Estado = @Estado Where Id = @IdRegistroVentas";
                con.Open();
                SqlCommand cmd = new SqlCommand(baseQuery, con);

                cmd.Parameters.AddWithValue("@Estado", Estado);
                cmd.Parameters.AddWithValue("@IdRegistroVentas", IdRegistroVentas);
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

            }


        }

        [HttpPost]
        public void updateStatusCoin()
        {

            int Estado = 0;
            int IdRegistroValoresEntregados = 0;
            foreach (string key in Request.Form.AllKeys)
            {
                if (key.StartsWith("Estado"))
                {
                    if (Request.Form[key] != "")
                        Estado = Convert.ToInt32(Request.Form[key]);
                }

                if (key.StartsWith("IdRegistroValoresEntregados"))
                {
                    if (Request.Form[key] != "")
                        IdRegistroValoresEntregados = Convert.ToInt32(Request.Form[key]);
                }

            }

            using (SqlConnection con = new SqlConnection(conexionDBHoka))
            {
                string baseQuery = @"UPDATE ingresos.dbo.RegistroValoresEntregados  SET Estado = @Estado Where Id = @IdRegistroValoresEntregados";
                con.Open();
                SqlCommand cmd = new SqlCommand(baseQuery, con);

                cmd.Parameters.AddWithValue("@Estado", Estado);
                cmd.Parameters.AddWithValue("@IdRegistroValoresEntregados", IdRegistroValoresEntregados);
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

            }

        }

        [HttpPost]
        public void updateStatusVoucher()
        {

            int Estado = 0;
            int IdRegistroDiaVoucher = 0;
            foreach (string key in Request.Form.AllKeys)
            {
                if (key.StartsWith("Estado"))
                {
                    if (Request.Form[key] != "")
                        Estado = Convert.ToInt32(Request.Form[key]);
                }

                if (key.StartsWith("IdRegistroDiaVoucher"))
                {
                    if (Request.Form[key] != "")
                        IdRegistroDiaVoucher = Convert.ToInt32(Request.Form[key]);
                }

            }

            using (SqlConnection con = new SqlConnection(conexionDBHoka))
            {
                string baseQuery = @"UPDATE ingresos.dbo.RegistroDiaVoucher  SET Estado = @Estado Where Id = @IdRegistroDiaVoucher";
                con.Open();
                SqlCommand cmd = new SqlCommand(baseQuery, con);

                cmd.Parameters.AddWithValue("@Estado", Estado);
                cmd.Parameters.AddWithValue("@IdRegistroDiaVoucher", IdRegistroDiaVoucher);
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

            }

        }



        [HttpPost]
        public void updateDataForSales()
        {
            float Comisiones = 0;
            float VentaTienda = 0;
            float VentaSistema = 0;
            float NetoVenta = 0;
            int Id = 0;

            if (Request.Form["Id"] != "")
            {
                Id = Convert.ToInt32(Request.Form["Id"]);
            }
            if (Request.Form["Comisiones"] != "")
            {
                Comisiones = Convert.ToSingle(Request.Form["Comisiones"]);
            }
            if (Request.Form["VentaSistema"] != "")
            {
                VentaSistema = Convert.ToSingle(Request.Form["VentaSistema"]);
            }
            if (Request.Form["VentaTienda"] != "")
            {
                VentaTienda = Convert.ToSingle(Request.Form["VentaTienda"]);
            }

            NetoVenta = VentaSistema - Comisiones;

            using (SqlConnection con = new SqlConnection(conexionDBHoka))
            {

                string baseQuery = @"UPDATE ingresos.dbo.VentasCategorias SET VentaTienda = @VentaTienda,Comisiones = @Comisiones,NetoVenta = @NetoVenta Where Id = @Id";
                con.Open();

                SqlCommand cmd = new SqlCommand(baseQuery, con);

                cmd.Parameters.AddWithValue("@Comisiones", Comisiones);
                cmd.Parameters.AddWithValue("@VentaTienda", VentaTienda);
                cmd.Parameters.AddWithValue("@NetoVenta", NetoVenta);
                cmd.Parameters.AddWithValue("@Id", Id);
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

            }

        }

        //
        [HttpPost]
        public void actualizarValoresMonedas()
        {

            //List<Tuple<string, string,string>> data = new List<Tuple<string, string, string>>();
            List<string> denominacionList = new List<string>();
            List<string> cantidadList = new List<string>();
            List<string> importeList = new List<string>();
            List<string> valoresEntregadosList = new List<string>();

            int index = 0;

            double TipoCambio = 0;
            int IdMoneda = 0;
            int IdValoresEntregados = 0;

            string FechaParam = "";
            int IdAlmacenParam = 0;
            string IdMonedaParam = "";

            foreach (string key in Request.Form.AllKeys)
            {
                string value = "";
                if (key.StartsWith("denominacion"))
                {
                    value = Request.Form[key];
                    denominacionList.Add(value);
                    index++;
                }
                if (key.StartsWith("cantidad"))
                {
                    value = Request.Form[key];
                    cantidadList.Add(value);
                }
                if (key.StartsWith("importe"))
                {
                    value = Request.Form[key];
                    importeList.Add(value);
                }
                if (key.StartsWith("TipoCambio"))
                {
                    TipoCambio = Convert.ToDouble(Request.Form[key]);
                }
                if (key.StartsWith("IdMoneda"))
                {
                    IdMoneda = Convert.ToInt32((Request.Form[key]));
                }
                if (key.StartsWith("IdValoresEntregados"))
                {
                    //IdValoresEntregados = Convert.ToInt32((Request.Form[key]));
                    valoresEntregadosList.Add(Request.Form[key]);
                    //IdValoresEntregados = Request.Form[key] != DBNull.Value ? Convert.ToInt32(Request.Form[key]) : 0;

                }
                if (key.StartsWith("FechaParam"))
                {
                    FechaParam = Request.Form[key];
                }
                if (key.StartsWith("IdAlmacenParam"))
                {
                    IdAlmacenParam = Convert.ToInt32(Request.Form[key]);
                }
                if (key.StartsWith("IdMonedaParam"))
                {
                    IdMonedaParam = Request.Form[key];
                }
            }

            //Response.Write("index" + index);
            Response.Write(TipoCambio);


            bool IsValid = true;



            if (IsValid)
            {
                Response.Write("actualizar");

                for (int i = 0; i < index; i++)
                {

                    Response.Write("for");

                    if (TipoCambio > 0 && IdMoneda > 0 && Convert.ToInt32(valoresEntregadosList[0]) > 0)
                    {

                        //verificar primero que no haya datos existentes sql
                        //sql

                        using (SqlConnection cn = new SqlConnection(conexionDBHoka))
                        {
                            cn.Open();

                            //calcular el importe aqui en el backend
                            float importe = 0;

                            SqlCommand cmd = new SqlCommand("ingresos.dbo.sp_ModificarEfectivo", cn);
                            // editar,
                            cmd.Parameters.AddWithValue("Cantidad", cantidadList[i]);
                            cmd.Parameters.AddWithValue("Importe", importeList[i]);
                            cmd.Parameters.AddWithValue("IdMonedaDenominacion", denominacionList[i]);
                            cmd.Parameters.AddWithValue("TipoCambio", TipoCambio);
                            cmd.Parameters.AddWithValue("IdAlmacen", IdAlmacenParam);
                            cmd.Parameters.AddWithValue("IdMoneda", IdMoneda);
                            cmd.Parameters.AddWithValue("Fecha", FechaParam);
                            cmd.Parameters.AddWithValue("IdValoresEntregados", valoresEntregadosList[i]);

                            cmd.Parameters.Add("Actualizado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                            cmd.Parameters.Add("Mensaje", SqlDbType.NVarChar, 100).Direction = ParameterDirection.Output;
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.ExecuteNonQuery();


                        }
                    }



                }

            }

            

        }
        //

    }
}