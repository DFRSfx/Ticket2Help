-- Script de criação da base de dados Ticket2Help
-- Execute este script no SQL Server para criar a estrutura necessária

-- Criar base de dados
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'Ticket2Help')
BEGIN
    CREATE DATABASE Ticket2Help
END
GO

USE Ticket2Help
GO

-- Tabela de Utilizadores
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Utilizadores' AND xtype='U')
BEGIN
    CREATE TABLE Utilizadores (
        Codigo NVARCHAR(50) PRIMARY KEY,
        Nome NVARCHAR(255) NOT NULL,
        Email NVARCHAR(255),
        PasswordHash NVARCHAR(500) NOT NULL,
        EhTecnicoHelpdesk BIT NOT NULL DEFAULT 0,
        Activo BIT NOT NULL DEFAULT 1,
        DataCriacao DATETIME2 NOT NULL DEFAULT GETDATE()
    )
END
GO

-- Tabela de Tickets
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Tickets' AND xtype='U')
BEGIN
    CREATE TABLE Tickets (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        TipoTicket NVARCHAR(50) NOT NULL, -- 'Hardware' ou 'Software'
        CodigoColaborador NVARCHAR(50) NOT NULL,
        DataHoraCriacao DATETIME2 NOT NULL DEFAULT GETDATE(),
        Estado NVARCHAR(50) NOT NULL DEFAULT 'porAtender', -- 'porAtender', 'emAtendimento', 'atendido'
        DataHoraAtendimento DATETIME2 NULL,
        EstadoAtendimento NVARCHAR(50) NULL, -- 'aberto', 'resolvido', 'naoResolvido'
        UsuarioResponsavel NVARCHAR(50) NULL,
        
        -- Campos específicos para Hardware
        Equipamento NVARCHAR(500) NULL,
        Avaria NTEXT NULL,
        DescricaoReparacao NTEXT NULL,
        Pecas NTEXT NULL,
        
        -- Campos específicos para Software
        Software NVARCHAR(500) NULL,
        DescricaoNecessidade NTEXT NULL,
        DescricaoIntervencao NTEXT NULL,
        
        FOREIGN KEY (CodigoColaborador) REFERENCES Utilizadores(Codigo),
        FOREIGN KEY (UsuarioResponsavel) REFERENCES Utilizadores(Codigo)
    )
END
GO

-- Stored Procedures

-- SP para criar ticket de hardware
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_CriarTicketHardware')
    DROP PROCEDURE sp_CriarTicketHardware
GO

CREATE PROCEDURE sp_CriarTicketHardware
    @CodigoColaborador NVARCHAR(50),
    @Equipamento NVARCHAR(500),
    @Avaria NTEXT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @NovoId INT;
    
    INSERT INTO Tickets (TipoTicket, CodigoColaborador, Equipamento, Avaria)
    VALUES ('Hardware', @CodigoColaborador, @Equipamento, @Avaria);
    
    SET @NovoId = SCOPE_IDENTITY();
    SELECT @NovoId AS NovoId;
END
GO

-- SP para criar ticket de software
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_CriarTicketSoftware')
    DROP PROCEDURE sp_CriarTicketSoftware
GO

CREATE PROCEDURE sp_CriarTicketSoftware
    @CodigoColaborador NVARCHAR(50),
    @Software NVARCHAR(500),
    @DescricaoNecessidade NTEXT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @NovoId INT;
    
    INSERT INTO Tickets (TipoTicket, CodigoColaborador, Software, DescricaoNecessidade)
    VALUES ('Software', @CodigoColaborador, @Software, @DescricaoNecessidade);
    
    SET @NovoId = SCOPE_IDENTITY();
    SELECT @NovoId AS NovoId;
END
GO

-- SP para actualizar estado do ticket
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_ActualizarEstadoTicket')
    DROP PROCEDURE sp_ActualizarEstadoTicket
GO

CREATE PROCEDURE sp_ActualizarEstadoTicket
    @TicketId INT,
    @NovoEstado NVARCHAR(50),
    @EstadoAtendimento NVARCHAR(50) = NULL,
    @DescricaoReparacao NTEXT = NULL,
    @Pecas NTEXT = NULL,
    @DescricaoIntervencao NTEXT = NULL,
    @UsuarioResponsavel NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Tickets 
    SET Estado = @NovoEstado,
        DataHoraAtendimento = CASE WHEN @NovoEstado IN ('emAtendimento', 'atendido') 
                                   THEN GETDATE() 
                                   ELSE DataHoraAtendimento END,
        EstadoAtendimento = @EstadoAtendimento,
        DescricaoReparacao = ISNULL(@DescricaoReparacao, DescricaoReparacao),
        Pecas = ISNULL(@Pecas, Pecas),
        DescricaoIntervencao = ISNULL(@DescricaoIntervencao, DescricaoIntervencao),
        UsuarioResponsavel = ISNULL(@UsuarioResponsavel, UsuarioResponsavel)
    WHERE Id = @TicketId;
END
GO

-- SP para obter estatísticas do dashboard
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_ObterEstatisticasDashboard')
    DROP PROCEDURE sp_ObterEstatisticasDashboard
GO

CREATE OR ALTER PROCEDURE sp_ObterEstatisticasDashboard
    @DataInicio DATETIME,
    @DataFim DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @TotalTickets INT
    DECLARE @TicketsAtendidos INT
    DECLARE @TicketsResolvidos INT
    DECLARE @TicketsNaoResolvidos INT
    
    -- Obter totais
    SELECT @TotalTickets = COUNT(*) 
    FROM Tickets 
    WHERE DataHoraCriacao BETWEEN @DataInicio AND @DataFim
    
    SELECT @TicketsAtendidos = COUNT(*) 
    FROM Tickets 
    WHERE DataHoraCriacao BETWEEN @DataInicio AND @DataFim 
    AND Estado = 'atendido'
    
    SELECT @TicketsResolvidos = COUNT(*) 
    FROM Tickets 
    WHERE DataHoraCriacao BETWEEN @DataInicio AND @DataFim 
    AND EstadoAtendimento = 'resolvido'
    
    SELECT @TicketsNaoResolvidos = COUNT(*) 
    FROM Tickets 
    WHERE DataHoraCriacao BETWEEN @DataInicio AND @DataFim 
    AND EstadoAtendimento = 'naoResolvido'
    
    -- Primeira consulta: estatísticas gerais
    SELECT 
        CASE 
            WHEN @TotalTickets = 0 THEN 0.0
            ELSE CAST(@TicketsAtendidos AS FLOAT) / @TotalTickets * 100
        END AS PercentagemAtendidos,
        CASE 
            WHEN @TicketsAtendidos = 0 THEN 0.0
            ELSE CAST(@TicketsResolvidos AS FLOAT) / @TicketsAtendidos * 100
        END AS PercentagemResolvidos,
        CASE 
            WHEN @TicketsAtendidos = 0 THEN 0.0
            ELSE CAST(@TicketsNaoResolvidos AS FLOAT) / @TicketsAtendidos * 100
        END AS PercentagemNaoResolvidos
    
    -- Segunda consulta: médias por tipo
    SELECT 
        TipoTicket,
        AVG(CAST(DATEDIFF(HOUR, DataHoraCriacao, ISNULL(DataHoraAtendimento, GETDATE())) AS FLOAT)) AS MediaTempoAtendimentoHoras
    FROM Tickets 
    WHERE DataHoraCriacao BETWEEN @DataInicio AND @DataFim
    AND Estado = 'atendido'
    GROUP BY TipoTicket
    
    -- Terceira consulta: contadores
    SELECT 
        (SELECT COUNT(*) FROM Tickets WHERE CAST(DataHoraCriacao AS DATE) = CAST(GETDATE() AS DATE)) AS TotalTicketsHoje,
        (SELECT COUNT(*) FROM Tickets WHERE Estado = 'porAtender') AS TicketsPendentes,
        (SELECT COUNT(*) FROM Tickets WHERE Estado = 'emAtendimento') AS TicketsEmAtendimento
END
GO

-- View para relatórios completos
IF EXISTS (SELECT * FROM sys.views WHERE name = 'vw_TicketsCompletos')
    DROP VIEW vw_TicketsCompletos
GO

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
        ELSE 'Descrição não disponível'
    END AS DescricaoCompleta,
    CASE 
        WHEN t.DataHoraAtendimento IS NOT NULL 
        THEN DATEDIFF(HOUR, t.DataHoraCriacao, t.DataHoraAtendimento)
        ELSE NULL
    END AS TempoAtendimentoHoras
FROM Tickets t
LEFT JOIN Utilizadores u ON t.CodigoColaborador = u.Codigo
GO

-- Dados de teste
INSERT INTO Utilizadores (Codigo, Nome, Email, PasswordHash, EhTecnicoHelpdesk) VALUES 
('ADMIN', 'Administrador', 'admin@ticket2help.pt', 'admin', 1),
('TEC001', 'João Silva', 'joao.silva@ticket2help.pt', '123', 1),
('COL001', 'Rui Oliveira', 'rui.oliveira@empresa.pt', '123', 0)
GO

-- Tickets de exemplo
INSERT INTO Tickets (TipoTicket, CodigoColaborador, Equipamento, Avaria) VALUES 
('Hardware', 'COL001', 'Computador Dell OptiPlex 7090', 'Computador não liga após queda de energia'),
('Hardware', 'COL001', 'Impressora HP LaserJet Pro', 'Papel encravado e luzes a piscar'),
('Hardware', 'COL001', 'Monitor Samsung 24"', 'Ecrã com riscas verticais')
GO

INSERT INTO Tickets (TipoTicket, CodigoColaborador, Software, DescricaoNecessidade) VALUES 
('Software', 'COL001', 'Microsoft Office 365', 'Instalação e configuração no novo computador'),
('Software', 'COL001', 'Adobe Acrobat Pro', 'Problema ao assinar documentos PDF'),
('Software', 'COL001', 'Windows 11', 'Actualizações automáticas a falhar')
GO

PRINT 'Base de dados Ticket2Help criada com sucesso!'
PRINT 'Utilizadores criados:'
PRINT '- ADMIN (Técnico Helpdesk): admin / admin'
PRINT '- TEC001 (Técnico): TEC001 / 123'
PRINT '- COL001 (Colaborador): COL001 / 123'
PRINT 'Execute a aplicação e faça login com um dos utilizadores acima.'