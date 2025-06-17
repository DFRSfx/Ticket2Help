using System;
using System.Collections.Generic;
using System.Linq;
using Ticket2Help.BLL.Managers;
using Ticket2Help.BLL.Patterns.Factory;
using Ticket2Help.BLL.Patterns.Observer;
using Ticket2Help.BLL.Patterns.Strategy;
using Ticket2Help.BLL.Services;
using Ticket2Help.DAL.Repositories;
using Ticket2Help.Models;
using Ticket2Help.UI.ViewModels;

namespace Ticket2Help.UI.Controllers
{
    /// <summary>
    /// Controlador principal para gestão de tickets (padrão MVC).
    /// </summary>
    public class TicketController
    {
        private readonly TicketService _ticketService;
        private readonly ITicketFactory _ticketFactory;
        private readonly GestorAtendimento _gestorAtendimento;
        private readonly RelatorioRepository _relatorioRepository;

        /// <summary>
        /// Construtor do controlador de tickets.
        /// </summary>
        public TicketController()
        {
            var ticketRepository = new TicketRepository();
            _ticketService = new TicketService(ticketRepository);
            _ticketFactory = new TicketFactory();
            _gestorAtendimento = new GestorAtendimento(new FIFOStrategy());
            _relatorioRepository = new RelatorioRepository();

            // Configurar observadores
            _ticketService.AdicionarObserver(new LogObserver());
            var uiObserver = new UINotificationObserver();
            _ticketService.AdicionarObserver(uiObserver);
        }

        /// <summary>
        /// Cria um novo ticket.
        /// </summary>
        /// <param name="tipo">Tipo de ticket.</param>
        /// <param name="dados">Dados do ticket.</param>
        /// <returns>True se criado com sucesso.</returns>
        public bool CriarTicket(TipoTicket tipo, Dictionary<string, object> dados)
        {
            try
            {
                var ticket = _ticketFactory.CriarTicket(tipo, dados);
                _ticketService.CriarTicket(ticket);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Obtém tickets de um colaborador.
        /// </summary>
        /// <param name="codigoColaborador">Código do colaborador.</param>
        /// <returns>Lista de ViewModels dos tickets.</returns>
        public List<TicketViewModel> ObterTicketsDoColaborador(string codigoColaborador)
        {
            var tickets = _ticketService.ObterTicketsColaborador(codigoColaborador);
            return tickets.Select(TicketViewModel.FromTicket).ToList();
        }

        /// <summary>
        /// Obtém tickets para atendimento.
        /// </summary>
        /// <returns>Lista ordenada de tickets para atendimento.</returns>
        public List<TicketViewModel> ObterTicketsParaAtendimento()
        {
            var todosTickets = _ticketService.ObterTodos();
            var ticketsParaAtendimento = _gestorAtendimento.ObterTicketsParaAtendimento(todosTickets);
            return ticketsParaAtendimento.Select(TicketViewModel.FromTicket).ToList();
        }

        /// <summary>
        /// Inicia o atendimento de um ticket (coloca em "emAtendimento").
        /// </summary>
        /// <param name="ticketId">ID do ticket.</param>
        /// <param name="usuarioResponsavel">Usuário que irá atender.</param>
        /// <returns>True se iniciado com sucesso.</returns>
        public bool IniciarAtendimento(int ticketId, string usuarioResponsavel)
        {
            return _ticketService.IniciarAtendimento(ticketId, usuarioResponsavel);
        }

        /// <summary>
        /// Finaliza o atendimento de um ticket.
        /// </summary>
        /// <param name="ticketId">ID do ticket.</param>
        /// <param name="dadosAtendimento">Dados do atendimento.</param>
        /// <returns>True se finalizado com sucesso.</returns>
        public bool FinalizarAtendimento(int ticketId, Dictionary<string, object> dadosAtendimento)
        {
            try
            {
                // Extrair estado do atendimento
                if (!dadosAtendimento.ContainsKey("estadoAtendimento"))
                    return false;

                var estadoAtendimento = (EstadoAtendimento)Enum.Parse(typeof(EstadoAtendimento),
                    dadosAtendimento["estadoAtendimento"].ToString());

                return _ticketService.FinalizarAtendimento(ticketId, estadoAtendimento, dadosAtendimento);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Método legacy mantido para compatibilidade - agora chama FinalizarAtendimento.
        /// </summary>
        /// <param name="ticketId">ID do ticket.</param>
        /// <param name="dadosAtendimento">Dados do atendimento.</param>
        /// <returns>True se atendido com sucesso.</returns>
        [Obsolete("Use IniciarAtendimento seguido de FinalizarAtendimento para melhor controle")]
        public bool AtenderTicket(int ticketId, Dictionary<string, object> dadosAtendimento)
        {
            return FinalizarAtendimento(ticketId, dadosAtendimento);
        }

        /// <summary>
        /// Altera a estratégia de atendimento.
        /// </summary>
        /// <param name="nomeEstrategia">Nome da estratégia.</param>
        public void AlterarEstrategiaAtendimento(string nomeEstrategia)
        {
            IAtendimentoStrategy novaEstrategia = nomeEstrategia switch
            {
                "FIFO" => new FIFOStrategy(),
                "PrioridadeHardware" => new PrioridadeHardwareStrategy(),
                _ => new FIFOStrategy()
            };

            _gestorAtendimento.DefinirEstrategia(novaEstrategia);
        }

        /// <summary>
        /// Obtém dados para o dashboard.
        /// </summary>
        /// <param name="dataInicio">Data de início.</param>
        /// <param name="dataFim">Data de fim.</param>
        /// <returns>ViewModel com dados do dashboard.</returns>
        public DashboardViewModel ObterDadosDashboard(DateTime dataInicio, DateTime dataFim)
        {
            var dados = _relatorioRepository.ObterEstatisticasDashboard(dataInicio, dataFim);

            return new DashboardViewModel
            {
                PercentagemTicketsAtendidos = dados.PercentagemTicketsAtendidos,
                PercentagemTicketsResolvidos = dados.PercentagemTicketsResolvidos,
                PercentagemTicketsNaoResolvidos = dados.PercentagemTicketsNaoResolvidos,
                MediaTempoAtendimentoHardware = dados.MediaTempoAtendimentoHardware,
                MediaTempoAtendimentoSoftware = dados.MediaTempoAtendimentoSoftware,
                TotalTicketsHoje = dados.TotalTicketsHoje,
                TicketsPendentes = dados.TicketsPendentes,
                TicketsEmAtendimento = dados.TicketsEmAtendimento
            };
        }

        /// <summary>
        /// Obtém o nome da estratégia actual.
        /// </summary>
        /// <returns>Nome da estratégia.</returns>
        public string ObterEstrategiaActual()
        {
            return _gestorAtendimento.GetEstrategiaActual();
        }
    }
}