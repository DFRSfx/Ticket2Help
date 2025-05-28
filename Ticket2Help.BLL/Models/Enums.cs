namespace Ticket2Help.BLL.Models
{
    public enum EstadoTicket
    {
        PorAtender,
        EmAtendimento,
        Atendido
    }

    public enum EstadoAtendimento
    {
        Aberto,
        Resolvido,
        NaoResolvido
    }

    public enum TipoTicket
    {
        Hardware,
        Software
    }

    public enum TipoUtilizador
    {
        UtilizadorComum,
        TecnicoHelpdesk
    }
}