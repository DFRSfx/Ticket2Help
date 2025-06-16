using System;
using System.Windows;
using Ticket2Help.UI.Views;

namespace Ticket2Help.UI
{
    /// <summary>
    /// Lógica de interação para App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // Mostrar janela de login
                var loginWindow = new LoginWindow();
                var resultado = loginWindow.ShowDialog();

                if (resultado == true && loginWindow.UtilizadorAutenticado != null)
                {
                    // Login bem-sucedido, mostrar janela principal
                    var mainWindow = new MainWindow();
                    mainWindow.DefinirUtilizador(loginWindow.UtilizadorAutenticado);
                    mainWindow.Show();
                }
                else
                {
                    // Login cancelado ou falhou, fechar aplicação
                    Shutdown();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao iniciar a aplicação: {ex.Message}", "Erro Fatal",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        private void Application_DispatcherUnhandledException(object sender,
            System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"Erro não tratado: {e.Exception.Message}", "Erro",
                MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
    }
}