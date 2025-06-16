using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Ticket2Help.DAL.Interfaces;
using Ticket2Help.DAL.Connection;

namespace Ticket2Help.DAL.Repositories
{
    /// <summary>
    /// Repositório para geração de relatórios e estatísticas.
    /// </summary>
    public class RelatorioRepository
    {
        private readonly IDatabaseConnection _dbConnection;

        /// <summary>
        /// Construtor que utiliza a ligação singleton por defeito.
        /// </summary>
        public RelatorioRepository()
        {
            _dbConnection = DatabaseConnection.Instance;
        }

        /// <summary>
        /// Obtém estatísticas para o dashboard.
        /// </summary>
        /// <param name="dataInicio">Data de início do período.</param>
        /// <param name="dataFim">Data de fim do período.</param>
        /// <returns>Dados estatísticos do dashboard.</returns>
        public DashboardDto ObterEstatisticasDashboard(DateTime dataInicio, DateTime dataFim)
        {
            using var connection = _dbConnection.CreateConnection();
            connection.Open();

            using var command = new SqlCommand("sp_ObterEstatisticasDashboard", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@DataInicio", dataInicio);
            command.Parameters.AddWithValue("@DataFim", dataFim);

            var dashboard = new DashboardDto();

            using var reader = command.ExecuteReader();

            // Primeiro resultado: estatísticas gerais
            if (reader.Read())
            {
                dashboard.PercentagemTicketsAtendidos = reader.GetDouble("PercentagemAtendidos");
                dashboard.PercentagemTicketsResolvidos = reader.GetDouble("PercentagemResolvidos");
                dashboard.PercentagemTicketsNaoResolvidos = reader.GetDouble("PercentagemNaoResolvidos");
            }

            // Segundo resultado: médias por tipo
            if (reader.NextResult())
            {
                while (reader.Read())
                {
                    var tipoTicket = reader.GetString("TipoTicket");
                    var media = reader.GetDouble("MediaTempoAtendimentoHoras");

                    if (tipoTicket == "Hardware")
                        dashboard.MediaTempoAtendimentoHardware = media;
                    else if (tipoTicket == "Software")
                        dashboard.MediaTempoAtendimentoSoftware = media;
                }
            }

            // Terceiro resultado: contadores
            if (reader.NextResult() && reader.Read())
            {
                dashboard.TotalTicketsHoje = reader.GetInt32("TotalTicketsHoje");
                dashboard.TicketsPendentes = reader.GetInt32("TicketsPendentes");
                dashboard.TicketsEmAtendimento = reader.GetInt32("TicketsEmAtendimento");
            }

            return dashboard;
        }

        /// <summary>
        /// Obtém relatório detalhado de tickets.
        /// </summary>
        /// <param name="dataInicio">Data de início.</param>
        /// <param name="dataFim">Data de fim.</param>
        /// <returns>Lista de dados para relatório.</returns>
        public IEnumerable<TicketRelatorioDto> ObterRelatorioDetalhado(DateTime dataInicio, DateTime dataFim)
        {
            using var connection = _dbConnection.CreateConnection();
            connection.Open();

            const string sql = @"
                SELECT * FROM vw_TicketsCompletos 
                WHERE DataHoraCriacao BETWEEN @DataInicio AND @DataFim
                ORDER BY DataHoraCriacao DESC";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@DataInicio", dataInicio);
            command.Parameters.AddWithValue("@DataFim", dataFim);

            using var reader = command.ExecuteReader();

            var relatorio = new List<TicketRelatorioDto>();
            while (reader.Read())
            {
                relatorio.Add(new TicketRelatorioDto
                {
                    Id = reader.GetInt32("Id"),
                    TipoTicket = reader.GetString("TipoTicket"),
                    CodigoColaborador = reader.GetString("CodigoColaborador"),
                    NomeColaborador = reader.GetString("NomeColaborador"),
                    DataHoraCriacao = reader.GetDateTime("DataHoraCriacao"),
                    Estado = reader.GetString("Estado"),
                    DataHoraAtendimento = reader.IsDBNull("DataHoraAtendimento")
                        ? null : reader.GetDateTime("DataHoraAtendimento"),
                    EstadoAtendimento = reader.IsDBNull("EstadoAtendimento")
                        ? null : reader.GetString("EstadoAtendimento"),
                    DescricaoCompleta = reader.GetString("DescricaoCompleta"),
                    TempoAtendimentoHoras = reader.IsDBNull("TempoAtendimentoHoras")
                        ? null : reader.GetInt32("TempoAtendimentoHoras")
                });
            }

            return relatorio;
        }
    }

    /// <summary>
    /// DTO para dados do dashboard.
    /// </summary>
    public class DashboardDto
    {
        public double PercentagemTicketsAtendidos { get; set; }
        public double PercentagemTicketsResolvidos { get; set; }
        public double PercentagemTicketsNaoResolvidos { get; set; }
        public double MediaTempoAtendimentoHardware { get; set; }
        public double MediaTempoAtendimentoSoftware { get; set; }
        public int TotalTicketsHoje { get; set; }
        public int TicketsPendentes { get; set; }
        public int TicketsEmAtendimento { get; set; }
    }

    /// <summary>
    /// DTO para relatórios detalhados.
    /// </summary>
    public class TicketRelatorioDto
    {
        public int Id { get; set; }
        public string TipoTicket { get; set; }
        public string CodigoColaborador { get; set; }
        public string NomeColaborador { get; set; }
        public DateTime DataHoraCriacao { get; set; }
        public string Estado { get; set; }
        public DateTime? DataHoraAtendimento { get; set; }
        public string EstadoAtendimento { get; set; }
        public string DescricaoCompleta { get; set; }
        public int? TempoAtendimentoHoras { get; set; }
    }
}