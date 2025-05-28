namespace Ticket2Help.BLL.Models
{
    public class HardwareTicket : Ticket
    {
        public string Equipamento { get; set; }
        public string Avaria { get; set; }
        public string DescricaoReparacao { get; set; }
        public string Pecas { get; set; }

        public override TipoTicket Tipo => TipoTicket.Hardware;
        public override string Descricao => $"{Equipamento} - {Avaria}";

        public HardwareTicket(string equipamento, string avaria, string codigoUtilizador)
        {
            Equipamento = equipamento;
            Avaria = avaria;
            CodigoUtilizador = codigoUtilizador;
        }
    }
}
