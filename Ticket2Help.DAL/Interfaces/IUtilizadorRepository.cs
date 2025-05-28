using Ticket2Help.BLL.Models;

namespace Ticket2Help.DAL.Interfaces
{
    public interface IUtilizadorRepository
    {
        Utilizador ObterUtilizador(string codigo);
        IEnumerable<Utilizador> ObterTodosUtilizadores();
        bool InserirUtilizador(Utilizador utilizador);
        bool AtualizarUtilizador(Utilizador utilizador);
        bool ValidarCredenciais(string codigo, string password);
    }
}