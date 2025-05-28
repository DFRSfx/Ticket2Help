using Ticket2Help.BLL.Models;

namespace Ticket2Help.BLL.Patterns
{
    public interface ITicketStrategy
    {
        IEnumerable<Ticket> OrdenarTickets(IEnumerable<Ticket> tickets);
        string Nome { get; }
    }
}