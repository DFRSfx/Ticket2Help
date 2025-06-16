using System;
using Ticket2Help.Models;

namespace Ticket2Help.UI.ViewModels
{
    /// <summary>
    /// ViewModel para apresentação de tickets na interface.
    /// </summary>
    public class TicketViewModel
    {
        public int Id { get; set; }
        public string? Descricao { get; set; }
        public string? CodigoColaborador { get; set; }
        public DateTime DataCriacao { get; set; }
        public string? Estado { get; set; }
        public string? Tipo { get; set; }
        public DateTime? DataAtendimento { get; set; }
        public string? EstadoAtendimento { get; set; }
        public string? TempoEspera { get; set; } // Nova propriedade

        // Campos específicos para Hardware
        public string? Equipamento { get; set; }
        public string? Avaria { get; set; }
        public string? DescricaoReparacao { get; set; }
        public string? Pecas { get; set; }

        // Campos específicos para Software
        public string? Software { get; set; }
        public string? DescricaoNecessidade { get; set; }
        public string? DescricaoIntervencao { get; set; }

        /// <summary>
        /// Converte um Ticket para TicketViewModel.
        /// </summary>
        /// <param name="ticket">Ticket a converter.</param>
        /// <returns>ViewModel correspondente.</returns>
        public static TicketViewModel FromTicket(Ticket ticket)
        {
            var viewModel = new TicketViewModel
            {
                Id = ticket.Id,
                CodigoColaborador = ticket.CodigoColaborador,
                DataCriacao = ticket.DataHoraCriacao,
                Estado = ticket.Estado.ToString(),
                DataAtendimento = ticket.DataHoraAtendimento,
                EstadoAtendimento = ticket.EstadoAtendimento?.ToString(),
                Tipo = ticket.GetTipoTicket().ToString(),
                Descricao = ticket.GetDescricaoCompleta()
            };

            if (ticket is HardwareTicket hw)
            {
                viewModel.Equipamento = hw.Equipamento;
                viewModel.Avaria = hw.Avaria;
                viewModel.DescricaoReparacao = hw.DescricaoReparacao;
                viewModel.Pecas = hw.Pecas;
            }
            else if (ticket is SoftwareTicket sw)
            {
                viewModel.Software = sw.Software;
                viewModel.DescricaoNecessidade = sw.DescricaoNecessidade;
                viewModel.DescricaoIntervencao = sw.DescricaoIntervencao;
            }

            return viewModel;
        }
    }
}