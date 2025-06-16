using Microsoft.Data.SqlClient;
using Ticket2Help.DAL.Interfaces;

namespace Ticket2Help.DAL.Connection
{
    /// <summary>
    /// Implementação Singleton para gestão de ligações à base de dados.
    /// </summary>
    /// <remarks>
    /// Garante que existe apenas uma instância de configuração de ligação
    /// em toda a aplicação, implementando o padrão Singleton.
    /// </remarks>
    public sealed class DatabaseConnection : IDatabaseConnection
    {
        private static DatabaseConnection _instance = null;
        private static readonly object _lock = new object();

        /// <summary>
        /// String de ligação à base de dados.
        /// </summary>
        public string ConnectionString { get; private set; }

        /// <summary>
        /// Construtor privado para implementar Singleton.
        /// </summary>
        private DatabaseConnection()
        {
            // Configuração da string de ligação
            ConnectionString = "Data Source=SOARES\\SQLEXPRESS;Initial Catalog=Ticket2Help;Integrated Security=True;Trust Server Certificate=True";
        }

        /// <summary>
        /// Obtém a instância única da classe (Singleton).
        /// </summary>
        public static DatabaseConnection Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new DatabaseConnection();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Cria uma nova ligação SQL configurada.
        /// </summary>
        /// <returns>Nova instância de SqlConnection.</returns>
        public SqlConnection CreateConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}
