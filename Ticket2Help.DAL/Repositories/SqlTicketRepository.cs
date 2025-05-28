using Microsoft.Data.SqlClient;
using Ticket2Help.BLL.Models;
using Ticket2Help.DAL.Interfaces;

namespace Ticket2Help.DAL.Repositories
{
    public class SqlTicketRepository : ITicketRepository
    {
        private readonly string _connectionString;

        public SqlTicketRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> InserirTicketAsync(Ticket ticket)
        {
            const string sql = @"
                INSERT INTO Tickets (DataCriacao, DataAtendimento, CodigoUtilizador, EstadoTicket, EstadoAtendimento, Tipo,
                                    Equipamento, Avaria, DescricaoReparacao, Pecas,
                                    Software, Necessidade, DescricaoIntervencao)
                VALUES (@DataCriacao, @DataAtendimento, @CodigoUtilizador, @EstadoTicket, @EstadoAtendimento, @Tipo,
                        @Equipamento, @Avaria, @DescricaoReparacao, @Pecas,
                        @Software, @Necessidade, @DescricaoIntervencao);
                SELECT CAST(SCOPE_IDENTITY() AS int);";

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(sql, connection);
            AdicionarParametrosTicket(command, ticket);

            var result = await command.ExecuteScalarAsync();
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public async Task<bool> AtualizarTicketAsync(Ticket ticket)
        {
            const string sql = @"
                UPDATE Tickets SET 
                    DataAtendimento = @DataAtendimento,
                    EstadoTicket = @EstadoTicket,
                    EstadoAtendimento = @EstadoAtendimento,
                    DescricaoReparacao = @DescricaoReparacao,
                    Pecas = @Pecas,
                    DescricaoIntervencao = @DescricaoIntervencao
                WHERE Id = @Id";

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", ticket.Id);
            command.Parameters.AddWithValue("@DataAtendimento", (object?)ticket.DataAtendimento ?? DBNull.Value);
            command.Parameters.AddWithValue("@EstadoTicket", ticket.EstadoTicket.ToString());
            command.Parameters.AddWithValue("@EstadoAtendimento", ticket.EstadoAtendimento.ToString());

            if (ticket is HardwareTicket hw)
            {
                command.Parameters.AddWithValue("@DescricaoReparacao", (object?)hw.DescricaoReparacao ?? DBNull.Value);
                command.Parameters.AddWithValue("@Pecas", (object?)hw.Pecas ?? DBNull.Value);
                command.Parameters.AddWithValue("@DescricaoIntervencao", DBNull.Value);
            }
            else if (ticket is SoftwareTicket sw)
            {
                command.Parameters.AddWithValue("@DescricaoReparacao", DBNull.Value);
                command.Parameters.AddWithValue("@Pecas", DBNull.Value);
                command.Parameters.AddWithValue("@DescricaoIntervencao", (object?)sw.DescricaoIntervencao ?? DBNull.Value);
            }
            else
            {
                command.Parameters.AddWithValue("@DescricaoReparacao", DBNull.Value);
                command.Parameters.AddWithValue("@Pecas", DBNull.Value);
                command.Parameters.AddWithValue("@DescricaoIntervencao", DBNull.Value);
            }

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<Ticket?> ObterTicketPorIdAsync(int id)
        {
            const string sql = "SELECT * FROM Tickets WHERE Id = @Id";

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);

            await using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapearTicketFromReader(reader);
            }
            return null;
        }

        public async Task<IEnumerable<Ticket>> ObterTodosTicketsAsync()
        {
            const string sql = "SELECT * FROM Tickets ORDER BY DataCriacao DESC";
            var tickets = new List<Ticket>();

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(sql, connection);
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                tickets.Add(MapearTicketFromReader(reader));
            }

            return tickets;
        }

        public async Task<IEnumerable<Ticket>> ObterTicketsPorUtilizadorAsync(string codigoUtilizador)
        {
            const string sql = "SELECT * FROM Tickets WHERE CodigoUtilizador = @CodigoUtilizador ORDER BY DataCriacao DESC";
            var tickets = new List<Ticket>();

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@CodigoUtilizador", codigoUtilizador);

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                tickets.Add(MapearTicketFromReader(reader));
            }

            return tickets;
        }

        public async Task<IEnumerable<Ticket>> ObterTicketsPorEstadoAsync(EstadoTicket estado)
        {
            const string sql = "SELECT * FROM Tickets WHERE EstadoTicket = @EstadoTicket ORDER BY DataCriacao";
            var tickets = new List<Ticket>();

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EstadoTicket", estado.ToString());

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                tickets.Add(MapearTicketFromReader(reader));
            }

            return tickets;
        }

        public async Task<bool> EliminarTicketAsync(int id)
        {
            const string sql = "DELETE FROM Tickets WHERE Id = @Id";

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        private void AdicionarParametrosTicket(SqlCommand command, Ticket ticket)
        {
            command.Parameters.AddWithValue("@DataCriacao", ticket.DataCriacao);
            command.Parameters.AddWithValue("@DataAtendimento", (object?)ticket.DataAtendimento ?? DBNull.Value);
            command.Parameters.AddWithValue("@CodigoUtilizador", ticket.CodigoUtilizador);
            command.Parameters.AddWithValue("@EstadoTicket", ticket.EstadoTicket.ToString());
            command.Parameters.AddWithValue("@EstadoAtendimento", ticket.EstadoAtendimento.ToString());
            command.Parameters.AddWithValue("@Tipo", ticket.Tipo.ToString());

            if (ticket is HardwareTicket hw)
            {
                command.Parameters.AddWithValue("@Equipamento", hw.Equipamento);
                command.Parameters.AddWithValue("@Avaria", hw.Avaria);
                command.Parameters.AddWithValue("@DescricaoReparacao", (object?)hw.DescricaoReparacao ?? DBNull.Value);
                command.Parameters.AddWithValue("@Pecas", (object?)hw.Pecas ?? DBNull.Value);
                command.Parameters.AddWithValue("@Software", DBNull.Value);
                command.Parameters.AddWithValue("@Necessidade", DBNull.Value);
                command.Parameters.AddWithValue("@DescricaoIntervencao", DBNull.Value);
            }
            else if (ticket is SoftwareTicket sw)
            {
                command.Parameters.AddWithValue("@Equipamento", DBNull.Value);
                command.Parameters.AddWithValue("@Avaria", DBNull.Value);
                command.Parameters.AddWithValue("@DescricaoReparacao", DBNull.Value);
                command.Parameters.AddWithValue("@Pecas", DBNull.Value);
                command.Parameters.AddWithValue("@Software", sw.Software);
                command.Parameters.AddWithValue("@Necessidade", sw.Necessidade);
                command.Parameters.AddWithValue("@DescricaoIntervencao", (object?)sw.DescricaoIntervencao ?? DBNull.Value);
            }
            else
            {
                command.Parameters.AddWithValue("@Equipamento", DBNull.Value);
                command.Parameters.AddWithValue("@Avaria", DBNull.Value);
                command.Parameters.AddWithValue("@DescricaoReparacao", DBNull.Value);
                command.Parameters.AddWithValue("@Pecas", DBNull.Value);
                command.Parameters.AddWithValue("@Software", DBNull.Value);
                command.Parameters.AddWithValue("@Necessidade", DBNull.Value);
                command.Parameters.AddWithValue("@DescricaoIntervencao", DBNull.Value);
            }
        }

        private Ticket MapearTicketFromReader(SqlDataReader reader)
        {
            var tipo = Enum.Parse<TipoTicket>(reader["Tipo"].ToString() ?? throw new InvalidOperationException("Tipo is null"));

            Ticket ticket;
            if (tipo == TipoTicket.Hardware)
            {
                var hw = new HardwareTicket(
                    reader["Equipamento"]?.ToString() ?? string.Empty,
                    reader["Avaria"]?.ToString() ?? string.Empty,
                    reader["CodigoUtilizador"].ToString() ?? string.Empty
                )
                {
                    DescricaoReparacao = reader["DescricaoReparacao"]?.ToString(),
                    Pecas = reader["Pecas"]?.ToString()
                };
                ticket = hw;
            }
            else
            {
                var sw = new SoftwareTicket(
                    reader["Software"]?.ToString() ?? string.Empty,
                    reader["Necessidade"]?.ToString() ?? string.Empty,
                    reader["CodigoUtilizador"].ToString() ?? string.Empty
                )
                {
                    DescricaoIntervencao = reader["DescricaoIntervencao"]?.ToString()
                };
                ticket = sw;
            }

            ticket.Id = Convert.ToInt32(reader["Id"]);
            ticket.DataCriacao = Convert.ToDateTime(reader["DataCriacao"]);
            ticket.DataAtendimento = reader["DataAtendimento"] as DateTime?;
            ticket.EstadoTicket = Enum.Parse<EstadoTicket>(reader["EstadoTicket"].ToString() ?? throw new InvalidOperationException("EstadoTicket is null"));
            ticket.EstadoAtendimento = Enum.Parse<EstadoAtendimento>(reader["EstadoAtendimento"].ToString() ?? throw new InvalidOperationException("EstadoAtendimento is null"));

            return ticket;
        }
    }
}