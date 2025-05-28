using Microsoft.Data.SqlClient;
using Ticket2Help.BLL.Models;
using Ticket2Help.DAL.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ticket2Help.DAL.Repositories
{
    public class SqlUtilizadorRepository : IUtilizadorRepository
    {
        private readonly string _connectionString = @"Data Source = SOARES\SQLEXPRESS;Initial Catalog = Ticket2Help; Integrated Security = True; Trust Server Certificate=True";
        
        /// <summary>
        /// Obtém um utilizador específico pelo código
        /// </summary>
        public Utilizador ObterUtilizador(string codigo)
        {
            const string sql = "SELECT * FROM Utilizadores WHERE Codigo = @Codigo";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Codigo", codigo);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Utilizador(
                                reader["Codigo"].ToString(),
                                reader["Nome"].ToString(),
                                Enum.Parse<TipoUtilizador>(reader["Tipo"].ToString()),
                                reader["Password"].ToString()
                            );
                        }
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// Obtém todos os utilizadores da base de dados
        /// </summary>
        public IEnumerable<Utilizador> ObterTodosUtilizadores()
        {
            const string sql = "SELECT * FROM Utilizadores ORDER BY Nome";
            var utilizadores = new List<Utilizador>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(sql, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        utilizadores.Add(new Utilizador(
                            reader["Codigo"].ToString(),
                            reader["Nome"].ToString(),
                            Enum.Parse<TipoUtilizador>(reader["Tipo"].ToString()),
                            reader["Password"].ToString()
                        ));
                    }
                }
            }

            return utilizadores;
        }

        /// <summary>
        /// Insere um novo utilizador na base de dados
        /// </summary>
        public bool InserirUtilizador(Utilizador utilizador)
        {
            const string sql = @"
                INSERT INTO Utilizadores (Codigo, Nome, Tipo, Password)
                VALUES (@Codigo, @Nome, @Tipo, @Password)";

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Codigo", utilizador.Codigo);
                        command.Parameters.AddWithValue("@Nome", utilizador.Nome);
                        command.Parameters.AddWithValue("@Tipo", utilizador.Tipo.ToString());
                        command.Parameters.AddWithValue("@Password", utilizador.Password);

                        var rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Erro ao inserir utilizador: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Atualiza os dados de um utilizador existente
        /// </summary>
        public bool AtualizarUtilizador(Utilizador utilizador)
        {
            const string sql = @"
                UPDATE Utilizadores 
                SET Nome = @Nome, 
                    Tipo = @Tipo, 
                    Password = @Password
                WHERE Codigo = @Codigo";

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Codigo", utilizador.Codigo);
                        command.Parameters.AddWithValue("@Nome", utilizador.Nome);
                        command.Parameters.AddWithValue("@Tipo", utilizador.Tipo.ToString());
                        command.Parameters.AddWithValue("@Password", utilizador.Password);

                        var rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Erro ao atualizar utilizador: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Valida as credenciais de login de um utilizador
        /// </summary>
        public bool ValidarCredenciais(string codigo, string password)
        {
            const string sql = "SELECT COUNT(*) FROM Utilizadores WHERE Codigo = @Codigo AND Password = @Password";

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Codigo", codigo);
                        command.Parameters.AddWithValue("@Password", password);

                        var count = (int)command.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Erro ao validar credenciais: {ex.Message}");
                return false;
            }
        }
    }
}
