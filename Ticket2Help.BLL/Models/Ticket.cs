namespace Ticket2Help.BLL.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtendimento { get; set; }
        public string CodigoUtilizador { get; set; }
        public EstadoTicket EstadoTicket { get; set; }
        public EstadoAtendimento EstadoAtendimento { get; set; }

        public virtual TipoTicket Tipo { get; }
        public virtual string Descricao { get; }

        public Ticket()
        {
            DataCriacao = DateTime.Now;
            EstadoTicket = EstadoTicket.PorAtender;
            EstadoAtendimento = EstadoAtendimento.Aberto;
        }

        public bool PodeSerAtendido()
        {
            return EstadoTicket == EstadoTicket.PorAtender;
        }

        public void IniciarAtendimento()
        {
            if (PodeSerAtendido())
            {
                EstadoTicket = EstadoTicket.EmAtendimento;
                DataAtendimento = DateTime.Now;
            }
        }

        public void FinalizarAtendimento(EstadoAtendimento novoEstado)
        {
            EstadoTicket = EstadoTicket.Atendido;
            EstadoAtendimento = novoEstado;
        }
    }
}
