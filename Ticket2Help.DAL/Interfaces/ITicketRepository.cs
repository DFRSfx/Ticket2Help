using Ticket2Help.BLL.Models;

namespace Ticket2Help.DAL.Interfaces
{
    public interface ITicketRepository
    {
        Task<int> InserirTicketAsync(Ticket ticket);
        Task<bool> AtualizarTicketAsync(Ticket ticket);
        Task<Ticket> ObterTicketPorIdAsync(int id);
        Task<IEnumerable<Ticket>> ObterTodosTicketsAsync();
        Task<IEnumerable<Ticket>> ObterTicketsPorUtilizadorAsync(string codigoUtilizador);
        Task<IEnumerable<Ticket>> ObterTicketsPorEstadoAsync(EstadoTicket estado);
        Task<bool> EliminarTicketAsync(int id);
    }
}
