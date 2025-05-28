using Ticket2Help.BLL.Models;

namespace Ticket2Help.BLL.Patterns
{
    public class FIFOStrategy : ITicketStrategy
    {
        public string Nome => "FIFO (Primeiro a entrar, primeiro a sair)";

        public IEnumerable<Ticket> OrdenarTickets(IEnumerable<Ticket> tickets)
        {
            return tickets.OrderBy(t => t.DataCriacao);
        }
    }

    public class PrioridadeHardwareStrategy : ITicketStrategy
    {
        public string Nome => "Prioridade Hardware";

        public IEnumerable<Ticket> OrdenarTickets(IEnumerable<Ticket> tickets)
        {
            return tickets.OrderBy(t => t.Tipo == TipoTicket.Hardware ? 0 : 1)
                         .ThenBy(t => t.DataCriacao);
        }
    }

    public class PrioridadeSoftwareStrategy : ITicketStrategy
    {
        public string Nome => "Prioridade Software";

        public IEnumerable<Ticket> OrdenarTickets(IEnumerable<Ticket> tickets)
        {
            return tickets.OrderBy(t => t.Tipo == TipoTicket.Software ? 0 : 1)
                         .ThenBy(t => t.DataCriacao);
        }
    }
}