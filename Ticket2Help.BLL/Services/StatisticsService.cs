using Ticket2Help.BLL.Models;

namespace Ticket2Help.BLL.Services
{
    public class StatisticsService
    {
        public Dictionary<string, int> ObterEstatisticasPorTipo(IEnumerable<Ticket> tickets)
        {
            return tickets.GroupBy(t => t.Tipo)
                         .ToDictionary(g => g.Key.ToString(), g => g.Count());
        }

        public Dictionary<string, int> ObterEstatisticasPorEstado(IEnumerable<Ticket> tickets)
        {
            return tickets.GroupBy(t => t.EstadoTicket)
                         .ToDictionary(g => g.Key.ToString(), g => g.Count());
        }

        public Dictionary<string, int> ObterEstatisticasPorMes(IEnumerable<Ticket> tickets)
        {
            return tickets.GroupBy(t => t.DataCriacao.ToString("yyyy-MM"))
                         .ToDictionary(g => g.Key, g => g.Count());
        }

        public int ObterTicketsResolvidos(IEnumerable<Ticket> tickets)
        {
            return tickets.Count(t => t.EstadoAtendimento == EstadoAtendimento.Resolvido);
        }

        public double ObterTaxaResolucao(IEnumerable<Ticket> tickets)
        {
            var ticketsAtendidos = tickets.Where(t => t.EstadoTicket == EstadoTicket.Atendido);
            if (!ticketsAtendidos.Any()) return 0;

            var resolvidos = ticketsAtendidos.Count(t => t.EstadoAtendimento == EstadoAtendimento.Resolvido);
            return (double)resolvidos / ticketsAtendidos.Count() * 100;
        }
    }
}