using System;
using System.Collections.Generic;
using Ticket2Help.BLL.Patterns.Observer;
using Ticket2Help.DAL.Interfaces;
using Ticket2Help.Models;

namespace Ticket2Help.BLL.Services
{
    /// <summary>
    /// Serviço principal para gestão de tickets.
    /// Implementa o padrão Observer como Subject.
    /// </summary>
    public class TicketService : ITicketSubject
    {
        private readonly List<ITicketObserver> _observers;
        private readonly ITicketRepository _ticketRepository;

        /// <summary>
        /// Construtor do serviço de tickets.
        /// </summary>
        /// <param name="ticketRepository">Repositório de tickets.</param>
        public TicketService(ITicketRepository ticketRepository)
        {
            _observers = new List<ITicketObserver>();
            _ticketRepository = ticketRepository;
        }

        /// <summary>
        /// Adiciona um observador para notificações.
        /// </summary>
        /// <param name="observer">Observador a adicionar.</param>
        public void AdicionarObserver(ITicketObserver observer)
        {
            _observers.Add(observer);
        }

        /// <summary>
        /// Remove um observador das notificações.
        /// </summary>
        /// <param name="observer">Observador a remover.</param>
        public void RemoverObserver(ITicketObserver observer)
        {
            _observers.Remove(observer);
        }

        /// <summary>
        /// Notifica todos os observadores sobre mudança de estado.
        /// </summary>
        /// <param name="ticket">Ticket alterado.</param>
        /// <param name="estadoAnterior">Estado anterior.</param>
        public void NotificarObservers(Ticket ticket, EstadoTicket estadoAnterior)
        {
            foreach (var observer in _observers)
            {
                observer.OnTicketEstadoAlterado(ticket, estadoAnterior);
            }
        }

        /// <summary>
        /// Altera o estado de um ticket e notifica observadores.
        /// </summary>
        /// <param name="ticketId">ID do ticket.</param>
        /// <param name="novoEstado">Novo estado.</param>
        public void AlterarEstadoTicket(int ticketId, EstadoTicket novoEstado)
        {
            var ticket = _ticketRepository.ObterPorId(ticketId);
            if (ticket != null)
            {
                var estadoAnterior = ticket.Estado;
                ticket.Estado = novoEstado;

                if (novoEstado == EstadoTicket.emAtendimento || novoEstado == EstadoTicket.atendido)
                {
                    ticket.DataHoraAtendimento = DateTime.Now;
                }

                _ticketRepository.Actualizar(ticket);
                NotificarObservers(ticket, estadoAnterior);
            }
        }

        /// <summary>
        /// Cria um novo ticket.
        /// </summary>
        /// <param name="ticket">Ticket a criar.</param>
        public void CriarTicket(Ticket ticket)
        {
            _ticketRepository.Criar(ticket);
        }

        /// <summary>
        /// Obtém todos os tickets.
        /// </summary>
        /// <returns>Lista de tickets.</returns>
        public IEnumerable<Ticket> ObterTodos()
        {
            return _ticketRepository.ObterTodos();
        }

        /// <summary>
        /// Obtém tickets de um colaborador.
        /// </summary>
        /// <param name="codigoColaborador">Código do colaborador.</param>
        /// <returns>Tickets do colaborador.</returns>
        public IEnumerable<Ticket> ObterTicketsColaborador(string codigoColaborador)
        {
            return _ticketRepository.ObterPorColaborador(codigoColaborador);
        }

        /// <summary>
        /// Obtém tickets por estado.
        /// </summary>
        /// <param name="estado">Estado a filtrar.</param>
        /// <returns>Tickets no estado especificado.</returns>
        public IEnumerable<Ticket> ObterTicketsPorEstado(EstadoTicket estado)
        {
            return _ticketRepository.ObterPorEstado(estado);
        }
    }
}
