namespace Ticket2Help.UI.ViewModels
{
    /// <summary>
    /// ViewModel para dados do dashboard.
    /// </summary>
    public class DashboardViewModel
    {
        public double PercentagemTicketsAtendidos { get; set; }
        public double PercentagemTicketsResolvidos { get; set; }
        public double PercentagemTicketsNaoResolvidos { get; set; }
        public double MediaTempoAtendimentoHardware { get; set; }
        public double MediaTempoAtendimentoSoftware { get; set; }
        public int TotalTicketsHoje { get; set; }
        public int TicketsPendentes { get; set; }
        public int TicketsEmAtendimento { get; set; }
    }
}