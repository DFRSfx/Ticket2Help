using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using Ticket2Help.DAL.Connection;
using Ticket2Help.DAL.Interfaces;
using Ticket2Help.Models;

namespace Ticket2Help.DAL.Repositories
{
    /// <summary>
    /// Implementação do repositório para operações de utilizadores com autenticação simples.
    /// </summary>
    public class UtilizadorRepository : IUtilizadorRepository
    {
        private readonly IDatabaseConnection _dbConnection;
        private const int CommandTimeoutSeconds = 30;

        /// <summary>
        /// Construtor que utiliza a ligação singleton por defeito.
        /// </summary>
        public UtilizadorRepository()
        {
            _dbConnection = DatabaseConnection.Instance;
        }

        /// <summary>
        /// Autentica um utilizador verificando código e password na base de dados.
        /// </summary>
        /// <param name="codigo">Código do utilizador.</param>
        /// <param name="password">Password do utilizador.</param>
        /// <returns>Utilizador autenticado ou null se credenciais inválidas.</returns>
        public Utilizador AutenticarUtilizador(string codigo, string password)
        {
            if (string.IsNullOrWhiteSpace(codigo) || string.IsNullOrWhiteSpace(password))
                return null;

            SqlConnection connection = null;
            try
            {
                connection = _dbConnection.CreateConnection();
                connection.Open();

                const string sql = @"
                    SELECT Codigo, Nome, Email, EhTecnicoHelpdesk
                    FROM Utilizadores 
                    WHERE Codigo = @Codigo 
                      AND Password = @Password 
                      AND Activo = 1";

                using var command = new SqlCommand(sql, connection);
                command.CommandTimeout = CommandTimeoutSeconds;
                command.Parameters.AddWithValue("@Codigo", codigo);
                command.Parameters.AddWithValue("@Password", password);

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return new Utilizador
                    {
                        Codigo = reader.GetString("Codigo"),
                        Nome = reader.GetString("Nome"),
                        Email = reader.GetString("Email"),
                        EhTecnicoHelpdesk = reader.GetBoolean("EhTecnicoHelpdesk")
                    };
                }

                // Se chegou aqui, as credenciais estão incorretas
                return null;
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Erro na base de dados: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao autenticar utilizador: {ex.Message}", ex);
            }
            finally
            {
                connection?.Close();
                connection?.Dispose();
            }
        }

        /// <summary>
        /// Obtém um utilizador pelo código (sem verificar password).
        /// </summary>
        /// <param name="codigo">Código do utilizador.</param>
        /// <returns>Utilizador encontrado ou null.</returns>
        public Utilizador ObterPorCodigo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return null;

            SqlConnection connection = null;
            try
            {
                connection = _dbConnection.CreateConnection();
                connection.Open();

                const string sql = @"
                    SELECT Codigo, Nome, Email, EhTecnicoHelpdesk
                    FROM Utilizadores 
                    WHERE Codigo = @Codigo AND Activo = 1";

                using var command = new SqlCommand(sql, connection);
                command.CommandTimeout = CommandTimeoutSeconds;
                command.Parameters.AddWithValue("@Codigo", codigo);

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return new Utilizador
                    {
                        Codigo = reader.GetString("Codigo"),
                        Nome = reader.GetString("Nome"),
                        Email = reader.GetString("Email"),
                        EhTecnicoHelpdesk = reader.GetBoolean("EhTecnicoHelpdesk")
                    };
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Erro na base de dados: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter utilizador: {ex.Message}", ex);
            }
            finally
            {
                connection?.Close();
                connection?.Dispose();
            }

            return null;
        }

        /// <summary>
        /// Cria um novo utilizador.
        /// </summary>
        /// <param name="utilizador">Utilizador a criar.</param>
        public void Criar(Utilizador utilizador)
        {
            SqlConnection connection = null;
            try
            {
                connection = _dbConnection.CreateConnection();
                connection.Open();

                const string sql = @"
                    INSERT INTO Utilizadores (Codigo, Nome, Email, EhTecnicoHelpdesk, Activo)
                    VALUES (@Codigo, @Nome, @Email, @EhTecnicoHelpdesk, 1)";

                using var command = new SqlCommand(sql, connection);
                command.CommandTimeout = CommandTimeoutSeconds;
                command.Parameters.AddWithValue("@Codigo", utilizador.Codigo);
                command.Parameters.AddWithValue("@Nome", utilizador.Nome);
                command.Parameters.AddWithValue("@Email", utilizador.Email);
                command.Parameters.AddWithValue("@EhTecnicoHelpdesk", utilizador.EhTecnicoHelpdesk);

                command.ExecuteNonQuery();
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Erro na base de dados: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao criar utilizador: {ex.Message}", ex);
            }
            finally
            {
                connection?.Close();
                connection?.Dispose();
            }
        }

        /// <summary>
        /// Actualiza dados de um utilizador.
        /// </summary>
        /// <param name="utilizador">Utilizador com dados actualizados.</param>
        public void Actualizar(Utilizador utilizador)
        {
            SqlConnection connection = null;
            try
            {
                connection = _dbConnection.CreateConnection();
                connection.Open();

                const string sql = @"
                    UPDATE Utilizadores 
                    SET Nome = @Nome, Email = @Email, EhTecnicoHelpdesk = @EhTecnicoHelpdesk
                    WHERE Codigo = @Codigo";

                using var command = new SqlCommand(sql, connection);
                command.CommandTimeout = CommandTimeoutSeconds;
                command.Parameters.AddWithValue("@Codigo", utilizador.Codigo);
                command.Parameters.AddWithValue("@Nome", utilizador.Nome);
                command.Parameters.AddWithValue("@Email", utilizador.Email);
                command.Parameters.AddWithValue("@EhTecnicoHelpdesk", utilizador.EhTecnicoHelpdesk);

                command.ExecuteNonQuery();
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Erro na base de dados: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao actualizar utilizador: {ex.Message}", ex);
            }
            finally
            {
                connection?.Close();
                connection?.Dispose();
            }
        }

        /// <summary>
        /// Obtém todos os utilizadores activos.
        /// </summary>
        /// <returns>Lista de utilizadores.</returns>
        public IEnumerable<Utilizador> ObterTodos()
        {
            var utilizadores = new List<Utilizador>();
            SqlConnection connection = null;

            try
            {
                connection = _dbConnection.CreateConnection();
                connection.Open();

                const string sql = @"
                    SELECT Codigo, Nome, Email, EhTecnicoHelpdesk
                    FROM Utilizadores 
                    WHERE Activo = 1
                    ORDER BY Nome";

                using var command = new SqlCommand(sql, connection);
                command.CommandTimeout = CommandTimeoutSeconds;
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    utilizadores.Add(new Utilizador
                    {
                        Codigo = reader.GetString("Codigo"),
                        Nome = reader.GetString("Nome"),
                        Email = reader.GetString("Email"),
                        EhTecnicoHelpdesk = reader.GetBoolean("EhTecnicoHelpdesk")
                    });
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Erro na base de dados: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter utilizadores: {ex.Message}", ex);
            }
            finally
            {
                connection?.Close();
                connection?.Dispose();
            }

            return utilizadores;
        }

        /// <summary>
        /// Valida as credenciais de um utilizador (método mantido por compatibilidade).
        /// </summary>
        /// <param name="codigo">Código do utilizador.</param>
        /// <param name="senha">Senha do utilizador.</param>
        /// <returns>True se as credenciais forem válidas.</returns>
        public bool ValidarCredenciais(string codigo, string senha)
        {
            var utilizador = AutenticarUtilizador(codigo, senha);
            return utilizador != null;
        }

        /// <summary>
        /// Testa a ligação à base de dados.
        /// </summary>
        /// <returns>True se a ligação for bem-sucedida.</returns>
        public bool TestarLigacao()
        {
            SqlConnection connection = null;
            try
            {
                connection = _dbConnection.CreateConnection();
                connection.Open();

                using var command = new SqlCommand("SELECT 1", connection);
                command.CommandTimeout = 10;
                command.ExecuteScalar();

                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                connection?.Close();
                connection?.Dispose();
            }
        }

        /// <summary>
        /// Verifica se existe um utilizador com determinado código na base de dados.
        /// </summary>
        /// <param name="codigo">Código a verificar.</param>
        /// <returns>True se o utilizador existir.</returns>
        public bool UtilizadorExiste(string codigo)
        {
            SqlConnection connection = null;
            try
            {
                connection = _dbConnection.CreateConnection();
                connection.Open();

                const string sql = "SELECT COUNT(*) FROM Utilizadores WHERE Codigo = @Codigo";

                using var command = new SqlCommand(sql, connection);
                command.CommandTimeout = CommandTimeoutSeconds;
                command.Parameters.AddWithValue("@Codigo", codigo);

                var count = (int)command.ExecuteScalar();
                return count > 0;
            }
            catch
            {
                return false;
            }
            finally
            {
                connection?.Close();
                connection?.Dispose();
            }
        }
    }
}