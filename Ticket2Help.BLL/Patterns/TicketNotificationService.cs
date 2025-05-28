using Ticket2Help.BLL.Models;

namespace Ticket2Help.BLL.Patterns
{
    public interface ITicketObserver
    {
        void OnTicketEstadoChanged(Ticket ticket, EstadoTicket estadoAnterior);
    }

    public class TicketNotificationService
    {
        private readonly List<ITicketObserver> _observers = new List<ITicketObserver>();

        public void AdicionarObserver(ITicketObserver observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);
        }

        public void RemoverObserver(ITicketObserver observer)
        {
            _observers.Remove(observer);
        }

        public void NotificarMudancaEstado(Ticket ticket, EstadoTicket estadoAnterior)
        {
            foreach (var observer in _observers)
            {
                observer.OnTicketEstadoChanged(ticket, estadoAnterior);
            }
        }
    }
}