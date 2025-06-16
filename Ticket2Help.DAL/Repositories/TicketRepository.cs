using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using Ticket2Help.Models;
using Ticket2Help.DAL.Interfaces;
using Ticket2Help.DAL.Connection;

namespace Ticket2Help.DAL.Repositories
{
    /// <summary>
    /// Implementação do repositório para operações de tickets na base de dados.
    /// </summary>
    public class TicketRepository : ITicketRepository
    {
        private readonly IDatabaseConnection _dbConnection;

        /// <summary>
        /// Construtor que utiliza a ligação singleton por defeito.
        /// </summary>
        public TicketRepository()
        {
            _dbConnection = DatabaseConnection.Instance;
        }

        /// <summary>
        /// Construtor para injecção de dependências (útil para testes).
        /// </summary>
        /// <param name="dbConnection">Ligação à base de dados.</param>
        public TicketRepository(IDatabaseConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        /// <summary>
        /// Cria um novo ticket na base de dados.
        /// </summary>
        /// <param name="ticket">Ticket a ser criado.</param>
        public void Criar(Ticket ticket)
        {
            using var connection = _dbConnection.CreateConnection();
            connection.Open();

            if (ticket is HardwareTicket hardwareTicket)
            {
                CriarTicketHardware(connection, hardwareTicket);
            }
            else if (ticket is SoftwareTicket softwareTicket)
            {
                CriarTicketSoftware(connection, softwareTicket);
            }
            else
            {
                throw new ArgumentException("Tipo de ticket não suportado");
            }
        }

        private void CriarTicketHardware(SqlConnection connection, HardwareTicket ticket)
        {
            using var command = new SqlCommand("sp_CriarTicketHardware", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@CodigoColaborador", ticket.CodigoColaborador);
            command.Parameters.AddWithValue("@Equipamento", ticket.Equipamento);
            command.Parameters.AddWithValue("@Avaria", ticket.Avaria);

            var result = command.ExecuteScalar();
            if (result != null && int.TryParse(result.ToString(), out int novoId))
            {
                ticket.Id = novoId;
            }
        }

        private void CriarTicketSoftware(SqlConnection connection, SoftwareTicket ticket)
        {
            using var command = new SqlCommand("sp_CriarTicketSoftware", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@CodigoColaborador", ticket.CodigoColaborador);
            command.Parameters.AddWithValue("@Software", ticket.Software);
            command.Parameters.AddWithValue("@DescricaoNecessidade", ticket.DescricaoNecessidade);

            var result = command.ExecuteScalar();
            if (result != null && int.TryParse(result.ToString(), out int novoId))
            {
                ticket.Id = novoId;
            }
        }

        /// <summary>
        /// Obtém um ticket pelo seu identificador.
        /// </summary>
        /// <param name="id">Identificador do ticket.</param>
        /// <returns>Ticket encontrado ou null.</returns>
        public Ticket ObterPorId(int id)
        {
            using var connection = _dbConnection.CreateConnection();
            connection.Open();

            const string sql = @"
                SELECT Id, TipoTicket, CodigoColaborador, DataHoraCriacao, Estado, 
                       DataHoraAtendimento, EstadoAtendimento,
                       Equipamento, Avaria, DescricaoReparacao, Pecas,
                       Software, DescricaoNecessidade, DescricaoIntervencao
                FROM Tickets 
                WHERE Id = @Id";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return MapearTicketDoReader(reader);
            }

            return null;
        }

        /// <summary>
        /// Obtém todos os tickets do sistema.
        /// </summary>
        /// <returns>Lista de todos os tickets.</returns>
        public IEnumerable<Ticket> ObterTodos()
        {
            using var connection = _dbConnection.CreateConnection();
            connection.Open();

            const string sql = @"
                SELECT Id, TipoTicket, CodigoColaborador, DataHoraCriacao, Estado, 
                       DataHoraAtendimento, EstadoAtendimento,
                       Equipamento, Avaria, DescricaoReparacao, Pecas,
                       Software, DescricaoNecessidade, DescricaoIntervencao
                FROM Tickets 
                ORDER BY DataHoraCriacao DESC";

            using var command = new SqlCommand(sql, connection);
            using var reader = command.ExecuteReader();

            var tickets = new List<Ticket>();
            while (reader.Read())
            {
                tickets.Add(MapearTicketDoReader(reader));
            }

            return tickets;
        }

        /// <summary>
        /// Obtém tickets de um colaborador específico.
        /// </summary>
        /// <param name="codigoColaborador">Código do colaborador.</param>
        /// <returns>Lista de tickets do colaborador.</returns>
        public IEnumerable<Ticket> ObterPorColaborador(string codigoColaborador)
        {
            using var connection = _dbConnection.CreateConnection();
            connection.Open();

            const string sql = @"
                SELECT Id, TipoTicket, CodigoColaborador, DataHoraCriacao, Estado, 
                       DataHoraAtendimento, EstadoAtendimento,
                       Equipamento, Avaria, DescricaoReparacao, Pecas,
                       Software, DescricaoNecessidade, DescricaoIntervencao
                FROM Tickets 
                WHERE CodigoColaborador = @CodigoColaborador
                ORDER BY DataHoraCriacao DESC";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@CodigoColaborador", codigoColaborador);

            using var reader = command.ExecuteReader();

            var tickets = new List<Ticket>();
            while (reader.Read())
            {
                tickets.Add(MapearTicketDoReader(reader));
            }

            return tickets;
        }

        /// <summary>
        /// Obtém tickets por estado.
        /// </summary>
        /// <param name="estado">Estado dos tickets a obter.</param>
        /// <returns>Lista de tickets no estado especificado.</returns>
        public IEnumerable<Ticket> ObterPorEstado(EstadoTicket estado)
        {
            using var connection = _dbConnection.CreateConnection();
            connection.Open();

            const string sql = @"
                SELECT Id, TipoTicket, CodigoColaborador, DataHoraCriacao, Estado, 
                       DataHoraAtendimento, EstadoAtendimento,
                       Equipamento, Avaria, DescricaoReparacao, Pecas,
                       Software, DescricaoNecessidade, DescricaoIntervencao
                FROM Tickets 
                WHERE Estado = @Estado
                ORDER BY DataHoraCriacao ASC";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Estado", estado.ToString());

            using var reader = command.ExecuteReader();

            var tickets = new List<Ticket>();
            while (reader.Read())
            {
                tickets.Add(MapearTicketDoReader(reader));
            }

            return tickets;
        }

        /// <summary>
        /// Actualiza um ticket existente.
        /// </summary>
        /// <param name="ticket">Ticket com dados actualizados.</param>
        public void Actualizar(Ticket ticket)
        {
            using var connection = _dbConnection.CreateConnection();
            connection.Open();

            using var command = new SqlCommand("sp_ActualizarEstadoTicket", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@TicketId", ticket.Id);
            command.Parameters.AddWithValue("@NovoEstado", ticket.Estado.ToString());
            command.Parameters.AddWithValue("@EstadoAtendimento",
                ticket.EstadoAtendimento?.ToString() ?? (object)DBNull.Value);

            if (ticket is HardwareTicket hw)
            {
                command.Parameters.AddWithValue("@DescricaoReparacao",
                    hw.DescricaoReparacao ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Pecas",
                    hw.Pecas ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@DescricaoIntervencao", DBNull.Value);
            }
            else if (ticket is SoftwareTicket sw)
            {
                command.Parameters.AddWithValue("@DescricaoIntervencao",
                    sw.DescricaoIntervencao ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@DescricaoReparacao", DBNull.Value);
                command.Parameters.AddWithValue("@Pecas", DBNull.Value);
            }

            command.Parameters.AddWithValue("@UsuarioResponsavel", DBNull.Value);

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Elimina um ticket da base de dados.
        /// </summary>
        /// <param name="id">Identificador do ticket a eliminar.</param>
        public void Eliminar(int id)
        {
            using var connection = _dbConnection.CreateConnection();
            connection.Open();

            const string sql = "DELETE FROM Tickets WHERE Id = @Id";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);

            command.ExecuteNonQuery();
        }

        private Ticket MapearTicketDoReader(SqlDataReader reader)
        {
            var tipoTicket = reader.GetString("TipoTicket");

            Ticket ticket = tipoTicket switch
            {
                "Hardware" => new HardwareTicket
                {
                    Equipamento = reader.IsDBNull("Equipamento") ? null : reader.GetString("Equipamento"),
                    Avaria = reader.IsDBNull("Avaria") ? null : reader.GetString("Avaria"),
                    DescricaoReparacao = reader.IsDBNull("DescricaoReparacao") ? null : reader.GetString("DescricaoReparacao"),
                    Pecas = reader.IsDBNull("Pecas") ? null : reader.GetString("Pecas")
                },
                "Software" => new SoftwareTicket
                {
                    Software = reader.IsDBNull("Software") ? null : reader.GetString("Software"),
                    DescricaoNecessidade = reader.IsDBNull("DescricaoNecessidade") ? null : reader.GetString("DescricaoNecessidade"),
                    DescricaoIntervencao = reader.IsDBNull("DescricaoIntervencao") ? null : reader.GetString("DescricaoIntervencao")
                },
                _ => throw new ArgumentException($"Tipo de ticket não suportado: {tipoTicket}")
            };

            // Mapear propriedades comuns
            ticket.Id = reader.GetInt32("Id");
            ticket.CodigoColaborador = reader.GetString("CodigoColaborador");
            ticket.DataHoraCriacao = reader.GetDateTime("DataHoraCriacao");
            ticket.Estado = Enum.Parse<EstadoTicket>(reader.GetString("Estado"));
            ticket.DataHoraAtendimento = reader.IsDBNull("DataHoraAtendimento")
                ? null : reader.GetDateTime("DataHoraAtendimento");

            if (!reader.IsDBNull("EstadoAtendimento"))
            {
                ticket.EstadoAtendimento = Enum.Parse<EstadoAtendimento>(reader.GetString("EstadoAtendimento"));
            }

            return ticket;
        }
    }
}
