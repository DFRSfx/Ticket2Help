using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Ticket2Help.Models;
using Ticket2Help.DAL.Interfaces;
using Ticket2Help.DAL.Connection;
using System.Data;

namespace Ticket2Help.DAL.Repositories
{
    /// <summary>
    /// Implementação do repositório para operações de utilizadores.
    /// </summary>
    public class UtilizadorRepository : IUtilizadorRepository
    {
        private readonly IDatabaseConnection _dbConnection;

        /// <summary>
        /// Construtor que utiliza a ligação singleton por defeito.
        /// </summary>
        public UtilizadorRepository()
        {
            _dbConnection = DatabaseConnection.Instance;
        }

        /// <summary>
        /// Obtém um utilizador pelo código.
        /// </summary>
        /// <param name="codigo">Código do utilizador.</param>
        /// <returns>Utilizador encontrado ou null.</returns>
        public Utilizador ObterPorCodigo(string codigo)
        {
            using var connection = _dbConnection.CreateConnection();
            connection.Open();

            const string sql = @"
                SELECT Codigo, Nome, Email, EhTecnicoHelpdesk, Activo
                FROM Utilizadores 
                WHERE Codigo = @Codigo AND Activo = 1";

            using var command = new SqlCommand(sql, connection);
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

            return null;
        }

        /// <summary>
        /// Cria um novo utilizador.
        /// </summary>
        /// <param name="Utilizador">Utilizador a criar.</param>
        public void Criar(Utilizador Utilizador)
        {
            using var connection = _dbConnection.CreateConnection();
            connection.Open();

            const string sql = @"
                INSERT INTO Utilizadores (Codigo, Nome, Email, PasswordHash, EhTecnicoHelpdesk)
                VALUES (@Codigo, @Nome, @Email, @PasswordHash, @EhTecnicoHelpdesk)";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Codigo", Utilizador.Codigo);
            command.Parameters.AddWithValue("@Nome", Utilizador.Nome);
            command.Parameters.AddWithValue("@Email", Utilizador.Email);
            command.Parameters.AddWithValue("@PasswordHash", "hash_temporario"); // Implementar hash real
            command.Parameters.AddWithValue("@EhTecnicoHelpdesk", Utilizador.EhTecnicoHelpdesk);

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Actualiza dados de um utilizador.
        /// </summary>
        /// <param name="Utilizador">Utilizador com dados actualizados.</param>
        public void Actualizar(Utilizador utilizador)
        {
            using var connection = _dbConnection.CreateConnection();
            connection.Open();

            const string sql = @"
                UPDATE Utilizadores 
                SET Nome = @Nome, Email = @Email, EhTecnicoHelpdesk = @EhTecnicoHelpdesk
                WHERE Codigo = @Codigo";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Codigo", utilizador.Codigo);
            command.Parameters.AddWithValue("@Nome", utilizador.Nome);
            command.Parameters.AddWithValue("@Email", utilizador.Email);
            command.Parameters.AddWithValue("@EhTecnicoHelpdesk", utilizador.EhTecnicoHelpdesk);

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Obtém todos os utilizadores activos.
        /// </summary>
        /// <returns>Lista de utilizadores.</returns>
        public IEnumerable<Utilizador> ObterTodos()
        {
            using var connection = _dbConnection.CreateConnection();
            connection.Open();

            const string sql = @"
                SELECT Codigo, Nome, Email, EhTecnicoHelpdesk
                FROM Utilizadores 
                WHERE Activo = 1
                ORDER BY Nome";

            using var command = new SqlCommand(sql, connection);
            using var reader = command.ExecuteReader();

            var utilizadores = new List<Utilizador>();
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

            return utilizadores;
        }

        /// <summary>
        /// Valida as credenciais de um utilizador.
        /// </summary>
        /// <param name="codigo">Código do utilizador.</param>
        /// <param name="senha">Senha do utilizador.</param>
        /// <returns>True se as credenciais forem válidas.</returns>
        public bool ValidarCredenciais(string codigo, string senha)
        {
            using var connection = _dbConnection.CreateConnection();
            connection.Open();

            const string sql = @"
                SELECT COUNT(*) 
                FROM Utilizadores 
                WHERE Codigo = @Codigo AND PasswordHash = @PasswordHash AND Activo = 1";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Codigo", codigo);
            command.Parameters.AddWithValue("@PasswordHash", GerarHashSenha(senha));

            var count = (int)command.ExecuteScalar();
            return count > 0;
        }

        private string GerarHashSenha(string senha)
        {
            // Implementação simplificada - usar BCrypt ou similar em produção
            return $"hash_{senha}";
        }
    }
}