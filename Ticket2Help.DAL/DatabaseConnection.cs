using Microsoft.Data.SqlClient;
using System.Configuration;

namespace Ticket2Help.DAL
{
    public static class DatabaseConnection
    {
        /// <summary>
        /// Testa a conexão com a base de dados
        /// </summary>
        public static bool TestarConexao()
        {
            try
            {
                using (var connection = new SqlConnection(@"Data Source = SOARES\SQLEXPRESS;Initial Catalog = Ticket2Help; Integrated Security = True; Trust Server Certificate=True"))
                {
                    connection.Open();
                    return connection.State == System.Data.ConnectionState.Open;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}