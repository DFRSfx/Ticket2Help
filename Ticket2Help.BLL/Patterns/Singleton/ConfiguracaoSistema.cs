namespace Ticket2Help.BLL.Patterns.Singleton
{
    /// <summary>
    /// Implementação do padrão Singleton para configurações do sistema.
    /// </summary>
    /// <remarks>
    /// Garante que existe apenas uma instância das configurações
    /// em toda a aplicação.
    /// 
    /// <b>Padrão Utilizado:</b> Singleton
    /// <b>Objectivo:</b> Controlar acesso a configurações globais
    /// <b>Benefícios:</b>
    /// - Controlo de acesso
    /// - Ponto único de configuração
    /// - Thread-safe
    /// </remarks>
    public sealed class ConfiguracaoSistema
    {
        private static ConfiguracaoSistema _instance = null;
        private static readonly object _lock = new object();

        /// <summary>
        /// String de ligação à base de dados.
        /// </summary>
        public string ConnectionString { get; private set; }

        /// <summary>
        /// Tempo limite para atendimento de tickets (em horas).
        /// </summary>
        public int TempoLimiteAtendimento { get; private set; }

        /// <summary>
        /// Indica se o sistema está em modo de depuração.
        /// </summary>
        public bool ModoDebug { get; private set; }

        /// <summary>
        /// Construtor privado para implementar Singleton.
        /// </summary>
        private ConfiguracaoSistema()
        {
            ConnectionString = "Server=localhost;Database=Ticket2Help;Trusted_Connection=true;TrustServerCertificate=true;";
            TempoLimiteAtendimento = 24;
            ModoDebug = true;
        }

        /// <summary>
        /// Obtém a instância única da classe (Singleton).
        /// </summary>
        public static ConfiguracaoSistema Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new ConfiguracaoSistema();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Actualiza as configurações do sistema.
        /// </summary>
        /// <param name="novaConnectionString">Nova string de ligação.</param>
        /// <param name="novoTempoLimite">Novo tempo limite em horas.</param>
        public void ActualizarConfiguracao(string novaConnectionString, int novoTempoLimite)
        {
            ConnectionString = novaConnectionString;
            TempoLimiteAtendimento = novoTempoLimite;
        }
    }
}