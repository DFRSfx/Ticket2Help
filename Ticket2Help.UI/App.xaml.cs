using System.Windows;
using Ticket2Help.UI.Views;

namespace Ticket2Help.UI
{
    /// <summary>
    /// Classe principal da aplicação.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Evento de arranque da aplicação.
        /// </summary>
        /// <param name="e">Argumentos de arranque.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Mostrar janela de login
            var loginWindow = new LoginWindow();
            if (loginWindow.ShowDialog() == true)
            {
                var usuario = loginWindow.UtilizadorAutenticado;
                var mainWindow = new MainWindow(usuario);
                mainWindow.Show();
            }
            else
            {
                Shutdown();
            }
        }
    }
}