using Ticket2Help.BLL.Models;

namespace Ticket2Help.BLL.Patterns
{
    public class AuthenticationManager
    {
        private static AuthenticationManager _instance;
        private static readonly object _lock = new object();
        private Utilizador _utilizadorAtual;

        private AuthenticationManager() { }

        public static AuthenticationManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new AuthenticationManager();
                    }
                }
                return _instance;
            }
        }

        public Utilizador UtilizadorAtual => _utilizadorAtual;
        public bool EstaAutenticado => _utilizadorAtual != null;

        public bool Autenticar(string codigo, string password, List<Utilizador> utilizadores)
        {

            var utilizador = utilizadores.FirstOrDefault(u =>
                u.Codigo.Equals(codigo, StringComparison.OrdinalIgnoreCase) &&
                u.Password == password);

            if (utilizador != null)
            {
                _utilizadorAtual = utilizador;
                return true;
            }

            return false;
        }

        public void Logout()
        {
            _utilizadorAtual = null;
        }
    }
}