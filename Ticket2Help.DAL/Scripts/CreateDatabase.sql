-- Criação da Base de Dados Ticket2Help
USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'Ticket2Help')
    DROP DATABASE Ticket2Help;
GO

CREATE DATABASE Ticket2Help;
GO

USE Ticket2Help;
GO

-- Tabela de Utilizadores
CREATE TABLE Utilizadores (
    Codigo NVARCHAR(50) PRIMARY KEY,
    Nome NVARCHAR(200) NOT NULL,
    Email NVARCHAR(200) NOT NULL,
    PasswordHash NVARCHAR(500) NOT NULL,
    EhTecnicoHelpdesk BIT NOT NULL DEFAULT 0,
    Activo BIT NOT NULL DEFAULT 1,
    DataCriacao DATETIME2 DEFAULT GETDATE()
);

-- Tabela de Tickets
CREATE TABLE Tickets (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TipoTicket NVARCHAR(20) NOT NULL CHECK (TipoTicket IN ('Hardware', 'Software')),
    CodigoColaborador NVARCHAR(50) NOT NULL,
    DataHoraCriacao DATETIME2 NOT NULL DEFAULT GETDATE(),
    Estado NVARCHAR(20) NOT NULL DEFAULT 'porAtender' CHECK (Estado IN ('porAtender', 'emAtendimento', 'atendido')),
    DataHoraAtendimento DATETIME2 NULL,
    EstadoAtendimento NVARCHAR(20) NULL CHECK (EstadoAtendimento IN ('aberto', 'resolvido', 'naoResolvido')),
    
    -- Campos específicos de Hardware
    Equipamento NVARCHAR(200) NULL,
    Avaria NVARCHAR(1000) NULL,
    DescricaoReparacao NVARCHAR(2000) NULL,
    Pecas NVARCHAR(1000) NULL,
    
    -- Campos específicos de Software
    Software NVARCHAR(200) NULL,
    DescricaoNecessidade NVARCHAR(1000) NULL,
    DescricaoIntervencao NVARCHAR(2000) NULL,
    
    -- Chaves estrangeiras
    FOREIGN KEY (CodigoColaborador) REFERENCES Utilizadores(Codigo)
);

-- Índices para performance
CREATE INDEX IX_Tickets_Estado ON Tickets(Estado);
CREATE INDEX IX_Tickets_CodigoColaborador ON Tickets(CodigoColaborador);
CREATE INDEX IX_Tickets_DataCriacao ON Tickets(DataHoraCriacao);
CREATE INDEX IX_Tickets_TipoTicket ON Tickets(TipoTicket);

-- View para relatórios completos
CREATE VIEW vw_TicketsCompletos AS
SELECT 
    t.Id,
    t.TipoTicket,
    t.CodigoColaborador,
    u.Nome AS NomeColaborador,
    t.DataHoraCriacao,
    t.Estado,
    t.DataHoraAtendimento,
    t.EstadoAtendimento,
    CASE 
        WHEN t.TipoTicket = 'Hardware' THEN CONCAT(t.Equipamento, ': ', t.Avaria)
        WHEN t.TipoTicket = 'Software' THEN CONCAT(t.Software, ': ', t.DescricaoNecessidade)
    END AS DescricaoCompleta,
    CASE 
        WHEN t.DataHoraAtendimento IS NOT NULL 
        THEN DATEDIFF(HOUR, t.DataHoraCriacao, t.DataHoraAtendimento)
        ELSE NULL
    END AS TempoAtendimentoHoras
FROM Tickets t
INNER JOIN Utilizadores u ON t.CodigoColaborador = u.Codigo;

-- Stored Procedures
GO
CREATE PROCEDURE sp_CriarTicketHardware
    @CodigoColaborador NVARCHAR(50),
    @Equipamento NVARCHAR(200),
    @Avaria NVARCHAR(1000)
AS
BEGIN
    DECLARE @NovoId INT;
    
    INSERT INTO Tickets (TipoTicket, CodigoColaborador, Equipamento, Avaria)
    VALUES ('Hardware', @CodigoColaborador, @Equipamento, @Avaria);
    
    SET @NovoId = SCOPE_IDENTITY();
    SELECT @NovoId AS NovoId;
END;
GO

CREATE PROCEDURE sp_CriarTicketSoftware
    @CodigoColaborador NVARCHAR(50),
    @Software NVARCHAR(200),
    @DescricaoNecessidade NVARCHAR(1000)
AS
BEGIN
    DECLARE @NovoId INT;
    
    INSERT INTO Tickets (TipoTicket, CodigoColaborador, Software, DescricaoNecessidade)
    VALUES ('Software', @CodigoColaborador, @Software, @DescricaoNecessidade);
    
    SET @NovoId = SCOPE_IDENTITY();
    SELECT @NovoId AS NovoId;
END;
GO

CREATE PROCEDURE sp_ActualizarEstadoTicket
    @TicketId INT,
    @NovoEstado NVARCHAR(20),
    @EstadoAtendimento NVARCHAR(20) = NULL,
    @DescricaoReparacao NVARCHAR(2000) = NULL,
    @Pecas NVARCHAR(1000) = NULL,
    @DescricaoIntervencao NVARCHAR(2000) = NULL,
    @UsuarioResponsavel NVARCHAR(50) = NULL
AS
BEGIN
    UPDATE Tickets
    SET Estado = @NovoEstado,
        DataHoraAtendimento = CASE WHEN @NovoEstado IN ('emAtendimento', 'atendido') THEN GETDATE() ELSE DataHoraAtendimento END,
        EstadoAtendimento = @EstadoAtendimento,
        DescricaoReparacao = COALESCE(@DescricaoReparacao, DescricaoReparacao),
        Pecas = COALESCE(@Pecas, Pecas),
        DescricaoIntervencao = COALESCE(@DescricaoIntervencao, DescricaoIntervencao)
    WHERE Id = @TicketId;
END;
GO

CREATE PROCEDURE sp_ObterEstatisticasDashboard
    @DataInicio DATETIME2,
    @DataFim DATETIME2
AS
BEGIN
    -- Estatísticas gerais
    SELECT 
        CAST(COUNT(CASE WHEN Estado = 'atendido' THEN 1 END) * 100.0 / COUNT(*) AS DECIMAL(5,2)) AS PercentagemAtendidos,
        CAST(COUNT(CASE WHEN EstadoAtendimento = 'resolvido' THEN 1 END) * 100.0 / NULLIF(COUNT(CASE WHEN Estado = 'atendido' THEN 1 END), 0) AS DECIMAL(5,2)) AS PercentagemResolvidos,
        CAST(COUNT(CASE WHEN EstadoAtendimento = 'naoResolvido' THEN 1 END) * 100.0 / NULLIF(COUNT(CASE WHEN Estado = 'atendido' THEN 1 END), 0) AS DECIMAL(5,2)) AS PercentagemNaoResolvidos
    FROM Tickets
    WHERE DataHoraCriacao BETWEEN @DataInicio AND @DataFim;
    
    -- Médias por tipo
    SELECT 
        TipoTicket,
        AVG(CAST(DATEDIFF(MINUTE, DataHoraCriacao, DataHoraAtendimento) AS FLOAT) / 60.0) AS MediaTempoAtendimentoHoras
    FROM Tickets
    WHERE DataHoraAtendimento IS NOT NULL
        AND DataHoraCriacao BETWEEN @DataInicio AND @DataFim
    GROUP BY TipoTicket;
    
    -- Contadores para hoje
    SELECT 
        COUNT(CASE WHEN CAST(DataHoraCriacao AS DATE) = CAST(GETDATE() AS DATE) THEN 1 END) AS TotalTicketsHoje,
        COUNT(CASE WHEN Estado = 'porAtender' THEN 1 END) AS TicketsPendentes,
        COUNT(CASE WHEN Estado = 'emAtendimento' THEN 1 END) AS TicketsEmAtendimento
    FROM Tickets;
END;
GO

-- Dados de teste
INSERT INTO Utilizadores (Codigo, Nome, Email, PasswordHash, EhTecnicoHelpdesk) VALUES
('ADMIN', 'Administrador Sistema', 'admin@ticket2help.pt', 'hash_admin', 1),
('TEC001', 'João Silva', 'joao.silva@empresa.pt', 'hash_123', 1),
('TEC002', 'Maria Santos', 'maria.santos@empresa.pt', 'hash_123', 1),
('COL001', 'Pedro Costa', 'pedro.costa@empresa.pt', 'hash_123', 0),
('COL002', 'Ana Ferreira', 'ana.ferreira@empresa.pt', 'hash_123', 0),
('COL003', 'Carlos Oliveira', 'carlos.oliveira@empresa.pt', 'hash_123', 0);

-- Tickets de exemplo
INSERT INTO Tickets (TipoTicket, CodigoColaborador, Equipamento, Avaria) VALUES
('Hardware', 'COL001', 'Computador Dell OptiPlex', 'Computador não liga após actualização'),
('Hardware', 'COL002', 'Impressora HP LaserJet', 'Encravamento de papel constante');

INSERT INTO Tickets (TipoTicket, CodigoColaborador, Software, DescricaoNecessidade) VALUES
('Software', 'COL003', 'Microsoft Office 365', 'Instalação e configuração para novo colaborador'),
('Software', 'COL001', 'Adobe Acrobat', 'Licença expirada, necessária renovação');

PRINT 'Base de dados Ticket2Help criada com sucesso!';