using System.Security.Cryptography;
using System.Text;

namespace hoka_cli.Models.Utileria.Encriptar
{
    public static class EncriptarSha256
    {
        public static string Encriptar(string texto)
        {
            StringBuilder Cadena = new StringBuilder();
            using (SHA256 Formato = SHA256Managed.Create())
            {
                Encoding Encoding = Encoding.UTF8;
                byte[] Resultado = Formato.ComputeHash(Encoding.GetBytes(texto));
                foreach (byte textoParcial in Resultado)
                    Cadena.Append(textoParcial.ToString("X2"));
            }
            return Cadena.ToString();
        }
    }
}
