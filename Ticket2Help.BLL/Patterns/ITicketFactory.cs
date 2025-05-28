using Ticket2Help.BLL.Models;

namespace Ticket2Help.BLL.Patterns
{
    public interface ITicketFactory
    {
        Ticket CriarTicket(TipoTicket tipo, Dictionary<string, object> parametros);
    }
}