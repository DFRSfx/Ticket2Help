-- Script de criação da base de dados Ticket2Help

-- Criar base de dados
CREATE DATABASE Ticket2Help;
GO

USE Ticket2Help;
GO

-- Tabela de Utilizadores
CREATE TABLE Utilizadores (
    Codigo NVARCHAR(20) PRIMARY KEY,
    Nome NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    EhTecnicoHelpdesk BIT NOT NULL DEFAULT 0,
    DataCriacao DATETIME2 NOT NULL DEFAULT GETDATE(),
    Activo BIT NOT NULL DEFAULT 1
);

-- Tabela de Tickets
CREATE TABLE Tickets (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TipoTicket NVARCHAR(20) NOT NULL CHECK (TipoTicket IN ('Hardware', 'Software')),
    CodigoColaborador NVARCHAR(20) NOT NULL,
    DataHoraCriacao DATETIME2 NOT NULL DEFAULT GETDATE(),
    Estado NVARCHAR(20) NOT NULL CHECK (Estado IN ('porAtender', 'emAtendimento', 'atendido')),
    DataHoraAtendimento DATETIME2 NULL,
    EstadoAtendimento NVARCHAR(20) NULL CHECK (EstadoAtendimento IN ('aberto', 'resolvido', 'naoResolvido')),
    
    -- Campos para Hardware
    Equipamento NVARCHAR(100) NULL,
    Avaria NVARCHAR(500) NULL,
    DescricaoReparacao NVARCHAR(1000) NULL,
    Pecas NVARCHAR(500) NULL,
    
    -- Campos para Software
    Software NVARCHAR(100) NULL,
    DescricaoNecessidade NVARCHAR(500) NULL,
    DescricaoIntervencao NVARCHAR(1000) NULL,
    
    CONSTRAINT FK_Tickets_Utilizadores FOREIGN KEY (CodigoColaborador) REFERENCES Utilizadores(Codigo),
    
    CONSTRAINT CK_Hardware_Fields CHECK (
        (TipoTicket = 'Hardware' AND Equipamento IS NOT NULL AND Avaria IS NOT NULL) OR
        (TipoTicket = 'Software')
    ),
    CONSTRAINT CK_Software_Fields CHECK (
        (TipoTicket = 'Software' AND Software IS NOT NULL AND DescricaoNecessidade IS NOT NULL) OR
        (TipoTicket = 'Hardware')
    )
);

-- Tabela de Log de Mudanças
CREATE TABLE LogMudancasEstado (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TicketId INT NOT NULL,
    EstadoAnterior NVARCHAR(20) NOT NULL,
    EstadoNovo NVARCHAR(20) NOT NULL,
    DataHoraMudanca DATETIME2 NOT NULL DEFAULT GETDATE(),
    UsuarioResponsavel NVARCHAR(20) NULL,
    Observacoes NVARCHAR(500) NULL,
    
    CONSTRAINT FK_LogMudancas_Tickets FOREIGN KEY (TicketId) REFERENCES Tickets(Id),
    CONSTRAINT FK_LogMudancas_Usuarios FOREIGN KEY (UsuarioResponsavel) REFERENCES Utilizadores(Codigo)
);

-- Índices
CREATE INDEX IX_Tickets_Estado ON Tickets(Estado);
CREATE INDEX IX_Tickets_CodigoColaborador ON Tickets(CodigoColaborador);
CREATE INDEX IX_Tickets_DataCriacao ON Tickets(DataHoraCriacao);
CREATE INDEX IX_Tickets_TipoTicket ON Tickets(TipoTicket);

-- Dados iniciais
INSERT INTO Utilizadores (Codigo, Nome, Email, PasswordHash, EhTecnicoHelpdesk) VALUES
('COL001', 'João Silva', 'joao.silva@empresa.pt', 'hash_123', 0),
('COL002', 'Maria Santos', 'maria.santos@empresa.pt', 'hash_456', 0),
('TEC001', 'Pedro Oliveira', 'pedro.oliveira@empresa.pt', 'hash_789', 1),
('TEC002', 'Ana Costa', 'ana.costa@empresa.pt', 'hash_321', 1),
('ADMIN', 'Administrador', 'admin@empresa.pt', 'hash_admin', 1);

-- Stored Procedures
CREATE PROCEDURE sp_CriarTicketHardware
    @CodigoColaborador NVARCHAR(20),
    @Equipamento NVARCHAR(100),
    @Avaria NVARCHAR(500)
AS
BEGIN
    INSERT INTO Tickets (TipoTicket, CodigoColaborador, Equipamento, Avaria, Estado)
    VALUES ('Hardware', @CodigoColaborador, @Equipamento, @Avaria, 'porAtender');
    
    SELECT SCOPE_IDENTITY() AS NovoTicketId;
END;
GO

CREATE PROCEDURE sp_CriarTicketSoftware
    @CodigoColaborador NVARCHAR(20),
    @Software NVARCHAR(100),
    @DescricaoNecessidade NVARCHAR(500)
AS
BEGIN
    INSERT INTO Tickets (TipoTicket, CodigoColaborador, Software, DescricaoNecessidade, Estado)
    VALUES ('Software', @CodigoColaborador, @Software, @DescricaoNecessidade, 'porAtender');
    
    SELECT SCOPE_IDENTITY() AS NovoTicketId;
END;
GO

CREATE PROCEDURE sp_ActualizarEstadoTicket
    @TicketId INT,
    @NovoEstado NVARCHAR(20),
    @EstadoAtendimento NVARCHAR(20) = NULL,
    @DescricaoReparacao NVARCHAR(1000) = NULL,
    @Pecas NVARCHAR(500) = NULL,
    @DescricaoIntervencao NVARCHAR(1000) = NULL,
    @UsuarioResponsavel NVARCHAR(20) = NULL
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;
        
        DECLARE @EstadoAnterior NVARCHAR(20);
        SELECT @EstadoAnterior = Estado FROM Tickets WHERE Id = @TicketId;
        
        UPDATE Tickets 
        SET Estado = @NovoEstado,
            EstadoAtendimento = @EstadoAtendimento,
            DataHoraAtendimento = CASE WHEN @NovoEstado IN ('emAtendimento', 'atendido') THEN GETDATE() ELSE DataHoraAtendimento END,
            DescricaoReparacao = COALESCE(@DescricaoReparacao, DescricaoReparacao),
            Pecas = COALESCE(@Pecas, Pecas),
            DescricaoIntervencao = COALESCE(@DescricaoIntervencao, DescricaoIntervencao)
        WHERE Id = @TicketId;
        
        INSERT INTO LogMudancasEstado (TicketId, EstadoAnterior, EstadoNovo, UsuarioResponsavel)
        VALUES (@TicketId, @EstadoAnterior, @NovoEstado, @UsuarioResponsavel);
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

CREATE PROCEDURE sp_ObterEstatisticasDashboard
    @DataInicio DATETIME2,
    @DataFim DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @TotalTickets INT = (SELECT COUNT(*) FROM Tickets WHERE DataHoraCriacao BETWEEN @DataInicio AND @DataFim);
    DECLARE @TicketsAtendidos INT = (SELECT COUNT(*) FROM Tickets WHERE Estado = 'atendido' AND DataHoraCriacao BETWEEN @DataInicio AND @DataFim);
    DECLARE @TicketsResolvidos INT = (SELECT COUNT(*) FROM Tickets WHERE EstadoAtendimento = 'resolvido' AND DataHoraCriacao BETWEEN @DataInicio AND @DataFim);
    DECLARE @TicketsNaoResolvidos INT = (SELECT COUNT(*) FROM Tickets WHERE EstadoAtendimento = 'naoResolvido' AND DataHoraCriacao BETWEEN @DataInicio AND @DataFim);
    
    SELECT 
        @TotalTickets AS TotalTickets,
        @TicketsAtendidos AS TicketsAtendidos,
        @TicketsResolvidos AS TicketsResolvidos,
        @TicketsNaoResolvidos AS TicketsNaoResolvidos,
        CASE WHEN @TotalTickets > 0 THEN CAST(@TicketsAtendidos AS FLOAT) / @TotalTickets * 100 ELSE 0 END AS PercentagemAtendidos,
        CASE WHEN @TicketsAtendidos > 0 THEN CAST(@TicketsResolvidos AS FLOAT) / @TicketsAtendidos * 100 ELSE 0 END AS PercentagemResolvidos,
        CASE WHEN @TicketsAtendidos > 0 THEN CAST(@TicketsNaoResolvidos AS FLOAT) / @TicketsAtendidos * 100 ELSE 0 END AS PercentagemNaoResolvidos;
        
    SELECT 
        TipoTicket,
        AVG(CAST(DATEDIFF(HOUR, DataHoraCriacao, DataHoraAtendimento) AS FLOAT)) AS MediaTempoAtendimentoHoras
    FROM Tickets 
    WHERE DataHoraAtendimento IS NOT NULL 
      AND DataHoraCriacao BETWEEN @DataInicio AND @DataFim
    GROUP BY TipoTicket;
    
    SELECT 
        COUNT(CASE WHEN CAST(DataHoraCriacao AS DATE) = CAST(GETDATE() AS DATE) THEN 1 END) AS TotalTicketsHoje,
        COUNT(CASE WHEN Estado = 'porAtender' THEN 1 END) AS TicketsPendentes,
        COUNT(CASE WHEN Estado = 'emAtendimento' THEN 1 END) AS TicketsEmAtendimento
    FROM Tickets;
END;
GO

-- Views
CREATE VIEW vw_TicketsParaAtendimento AS
SELECT 
    Id, TipoTicket, CodigoColaborador, DataHoraCriacao,
    CASE 
        WHEN TipoTicket = 'Hardware' THEN Equipamento + ': ' + Avaria
        WHEN TipoTicket = 'Software' THEN Software + ': ' + DescricaoNecessidade
    END AS DescricaoCompleta,
    Equipamento, Avaria, Software, DescricaoNecessidade
FROM Tickets
WHERE Estado = 'porAtender';
GO

CREATE VIEW vw_TicketsCompletos AS
SELECT 
    t.Id, t.TipoTicket, t.CodigoColaborador,
    u.Nome AS NomeColaborador, u.Email as EmailColaborador,
    t.DataHoraCriacao, t.Estado, t.DataHoraAtendimento, t.EstadoAtendimento,
    t.Equipamento, t.Avaria, t.DescricaoReparacao, t.Pecas,
    t.Software, t.DescricaoNecessidade, t.DescricaoIntervencao,
    CASE 
        WHEN t.TipoTicket = 'Hardware' THEN t.Equipamento + ': ' + t.Avaria
        WHEN t.TipoTicket = 'Software' THEN t.Software + ': ' + t.DescricaoNecessidade
    END AS DescricaoCompleta,
    CASE 
        WHEN t.DataHoraAtendimento IS NOT NULL 
        THEN DATEDIFF(HOUR, t.DataHoraCriacao, t.DataHoraAtendimento)
        ELSE NULL 
    END AS TempoAtendimentoHoras
FROM Tickets t
INNER JOIN Utilizadores u ON t.CodigoColaborador = u.Codigo;
GO