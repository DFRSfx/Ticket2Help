using Ticket2Help.BLL.Models;

namespace Ticket2Help.BLL.Patterns
{
    public class TicketFactory : ITicketFactory
    {
        public Ticket CriarTicket(TipoTicket tipo, Dictionary<string, object> parametros)
        {
            switch (tipo)
            {
                case TipoTicket.Hardware:
                    return new HardwareTicket(
                        parametros["equipamento"].ToString(),
                        parametros["avaria"].ToString(),
                        parametros["codigoUtilizador"].ToString()
                    );

                case TipoTicket.Software:
                    return new SoftwareTicket(
                        parametros["software"].ToString(),
                        parametros["necessidade"].ToString(),
                        parametros["codigoUtilizador"].ToString()
                    );

                default:
                    throw new ArgumentException($"Tipo de ticket não suportado: {tipo}");
            }
        }
    }
}