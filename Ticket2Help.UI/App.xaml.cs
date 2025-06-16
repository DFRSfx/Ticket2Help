using System.Configuration;
using System.Data;
using System.Windows;
using Ticket2Help.UI.Views;

namespace Ticket2Help.UI
{
    /// <summary>
    /// Lógica de aplicação para App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Configurar cultura portuguesa
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-PT");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("pt-PT");

            try
            {
                // Mostrar janela de login
                var loginWindow = new LoginWindow();

                if (loginWindow.ShowDialog() == true)
                {
                    // Login bem-sucedido, mostrar janela principal
                    var mainWindow = new MainWindow();
                    mainWindow.DefinirUtilizador(loginWindow.UtilizadorAutenticado);
                    mainWindow.Show();
                }
                else
                {
                    // Login cancelado, fechar aplicação
                    Shutdown();
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Erro ao inicializar a aplicação: {ex.Message}",
                    "Erro Crítico", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }
    }
}