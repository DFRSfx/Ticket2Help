using Ticket2Help.BLL.Models;
using Ticket2Help.BLL.Patterns;

namespace Ticket2Help.BLL.Services
{
    public class TicketService
    {
        private readonly ITicketFactory _ticketFactory;
        private readonly TicketNotificationService _notificationService;
        private static int _proximoId = 1;

        public TicketService(ITicketFactory ticketFactory, TicketNotificationService notificationService)
        {
            _ticketFactory = ticketFactory;
            _notificationService = notificationService;
        }

        public Ticket CriarTicket(TipoTicket tipo, Dictionary<string, object> parametros)
        {
            var ticket = _ticketFactory.CriarTicket(tipo, parametros);
            ticket.Id = _proximoId++;
            return ticket;
        }

        public bool PodeAtenderTicket(Ticket ticket)
        {
            return ticket.PodeSerAtendido();
        }

        public void IniciarAtendimento(Ticket ticket)
        {
            if (PodeAtenderTicket(ticket))
            {
                var estadoAnterior = ticket.EstadoTicket;
                ticket.IniciarAtendimento();
                _notificationService.NotificarMudancaEstado(ticket, estadoAnterior);
            }
        }

        public void FinalizarAtendimento(Ticket ticket, EstadoAtendimento estadoAtendimento)
        {
            var estadoAnterior = ticket.EstadoTicket;
            ticket.FinalizarAtendimento(estadoAtendimento);
            _notificationService.NotificarMudancaEstado(ticket, estadoAnterior);
        }

        public IEnumerable<Ticket> ObterTicketsPorUtilizador(string codigoUtilizador, IEnumerable<Ticket> todosTickets)
        {
            return todosTickets.Where(t => t.CodigoUtilizador.Equals(codigoUtilizador, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<Ticket> ObterTicketsPorAtender(IEnumerable<Ticket> todosTickets)
        {
            return todosTickets.Where(t => t.EstadoTicket == EstadoTicket.PorAtender);
        }
    }
}