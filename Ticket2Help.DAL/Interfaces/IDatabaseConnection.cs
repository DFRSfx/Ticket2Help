using Microsoft.Data.SqlClient;

namespace Ticket2Help.DAL.Interfaces
{
    /// <summary>
    /// Interface para gestão de ligações à base de dados.
    /// </summary>
    public interface IDatabaseConnection
    {
        /// <summary>
        /// String de ligação à base de dados.
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// Cria uma nova ligação SQL.
        /// </summary>
        /// <returns>Instância de SqlConnection configurada.</returns>
        SqlConnection CreateConnection();
    }
}