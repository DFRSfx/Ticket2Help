namespace Ticket2Help.BLL.Models
{
    public class SoftwareTicket : Ticket
    {
        public string Software { get; set; }
        public string Necessidade { get; set; }
        public string DescricaoIntervencao { get; set; }

        public override TipoTicket Tipo => TipoTicket.Software;
        public override string Descricao => $"{Software} - {Necessidade}";

        public SoftwareTicket(string software, string necessidade, string codigoUtilizador)
        {
            Software = software;
            Necessidade = necessidade;
            CodigoUtilizador = codigoUtilizador;
        }
    }
}
