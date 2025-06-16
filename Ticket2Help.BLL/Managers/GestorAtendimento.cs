using System.Collections.Generic;
using System.Linq;
using Ticket2Help.BLL.Patterns.Strategy;
using Ticket2Help.Models;

namespace Ticket2Help.BLL.Managers
{
    /// <summary>
    /// Gestor para atendimento de tickets utilizando padrão Strategy.
    /// </summary>
    public class GestorAtendimento
    {
        private IAtendimentoStrategy _estrategia;

        /// <summary>
        /// Construtor que define a estratégia inicial.
        /// </summary>
        /// <param name="estrategia">Estratégia de atendimento.</param>
        public GestorAtendimento(IAtendimentoStrategy estrategia)
        {
            _estrategia = estrategia;
        }

        /// <summary>
        /// Define uma nova estratégia de atendimento.
        /// </summary>
        /// <param name="novaEstrategia">Nova estratégia.</param>
        public void DefinirEstrategia(IAtendimentoStrategy novaEstrategia)
        {
            _estrategia = novaEstrategia;
        }

        /// <summary>
        /// Obtém tickets ordenados para atendimento.
        /// </summary>
        /// <param name="tickets">Lista de tickets.</param>
        /// <returns>Tickets ordenados segundo a estratégia.</returns>
        public IEnumerable<Ticket> ObterTicketsParaAtendimento(IEnumerable<Ticket> tickets)
        {
            return _estrategia.OrdenarTicketsParaAtendimento(tickets);
        }

        /// <summary>
        /// Obtém o nome da estratégia actual.
        /// </summary>
        /// <returns>Nome da estratégia.</returns>
        public string GetEstrategiaActual()
        {
            return _estrategia.GetNomeEstrategia();
        }
    }
}