using Microsoft.Data.SqlClient;
using Ticket2Help.DAL.Interfaces;
using System;

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
        private static DatabaseConnection? _instance = null;
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
            // Tentar diferentes configurações de ligação
            ConnectionString = ObterConnectionString();
            System.Diagnostics.Debug.WriteLine($"DatabaseConnection inicializada com: {MascararConnectionString(ConnectionString)}");
        }

        private string ObterConnectionString()
        {
            // Lista de possíveis connection strings para testar
            var connectionStrings = new[]
            {
                // Original
                "Data Source=SOARES\\SQLEXPRESS;Initial Catalog=Ticket2Help;Integrated Security=True;Trust Server Certificate=True",
                
                // Alternativas comuns
                "Data Source=.\\SQLEXPRESS;Initial Catalog=Ticket2Help;Integrated Security=True;Trust Server Certificate=True",
                "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Ticket2Help;Integrated Security=True;Trust Server Certificate=True",
                "Data Source=localhost\\SQLEXPRESS;Initial Catalog=Ticket2Help;Integrated Security=True;Trust Server Certificate=True",
                "Data Source=.;Initial Catalog=Ticket2Help;Integrated Security=True;Trust Server Certificate=True",
                
                // Com timeout mais longo
                "Data Source=SOARES\\SQLEXPRESS;Initial Catalog=Ticket2Help;Integrated Security=True;Trust Server Certificate=True;Connection Timeout=30",
            };

            foreach (var connectionString in connectionStrings)
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine($"Testando connection string: {MascararConnectionString(connectionString)}");

                    using var connection = new SqlConnection(connectionString);
                    connection.Open();

                    // Testar uma query simples
                    using var command = new SqlCommand("SELECT 1", connection);
                    command.ExecuteScalar();

                    System.Diagnostics.Debug.WriteLine("✅ Connection string funcionou!");
                    return connectionString;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Falhou: {ex.Message}");
                }
            }

            // Se nenhuma funcionou, retornar a original e deixar o erro aparecer
            System.Diagnostics.Debug.WriteLine("⚠️ Nenhuma connection string funcionou, usando a original");
            return connectionStrings[0];
        }

        private string MascararConnectionString(string connectionString)
        {
            // Mascarar informações sensíveis para logging
            return connectionString.Replace("Password=", "Password=***").Replace("pwd=", "pwd=***");
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
            try
            {
                var connection = new SqlConnection(ConnectionString);
                System.Diagnostics.Debug.WriteLine("Nova conexão SQL criada");
                return connection;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERRO ao criar conexão SQL: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Testa a ligação à base de dados.
        /// </summary>
        /// <returns>True se a ligação estiver disponível.</returns>
        public bool TestarLigacao()
        {
            try
            {
                using var connection = CreateConnection();
                connection.Open();

                using var command = new SqlCommand("SELECT 1", connection);
                command.ExecuteScalar();

                System.Diagnostics.Debug.WriteLine("✅ Teste de ligação bem-sucedido");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Teste de ligação falhou: {ex.Message}");
                return false;
            }
        }
    }
}