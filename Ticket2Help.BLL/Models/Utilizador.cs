namespace Ticket2Help.BLL.Models
{
    public class Utilizador
    {
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public TipoUtilizador Tipo { get; set; }
        public string Password { get; set; }

        public Utilizador(string codigo, string nome, TipoUtilizador tipo, string password = "")
        {
            Codigo = codigo;
            Nome = nome;
            Tipo = tipo;
            Password = password;
        }
    }
}