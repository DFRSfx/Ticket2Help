-- ============================================================================
-- DAL/Scripts/DatabaseSetup.sql
-- Script para criação da base de dados Ticket2Help
-- ============================================================================

-- Criar a base de dados
IF NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = 'Ticket2Help')
BEGIN
    CREATE DATABASE Ticket2Help;
END
GO

USE Ticket2Help;
GO

-- Tabela de Utilizadores
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Utilizadores' AND xtype='U')
BEGIN
    CREATE TABLE Utilizadores (
        Codigo NVARCHAR(50) PRIMARY KEY NOT NULL,
        Nome NVARCHAR(100) NOT NULL,
        Tipo NVARCHAR(20) NOT NULL CHECK (Tipo IN ('UtilizadorComum', 'TecnicoHelpdesk')),
        Password NVARCHAR(255) NOT NULL,
        DataCriacao DATETIME DEFAULT GETDATE()
    );
END
GO

-- Tabela de Tickets
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Tickets' AND xtype='U')
BEGIN
    CREATE TABLE Tickets (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        DataCriacao DATETIME NOT NULL DEFAULT GETDATE(),
        DataAtendimento DATETIME NULL,
        CodigoUtilizador NVARCHAR(50) NOT NULL,
        EstadoTicket NVARCHAR(20) NOT NULL CHECK (EstadoTicket IN ('PorAtender', 'EmAtendimento', 'Atendido')),
        EstadoAtendimento NVARCHAR(20) NOT NULL CHECK (EstadoAtendimento IN ('Aberto', 'Resolvido', 'NaoResolvido')),
        Tipo NVARCHAR(20) NOT NULL CHECK (Tipo IN ('Hardware', 'Software')),
        
        -- Campos específicos para Hardware
        Equipamento NVARCHAR(200) NULL,
        Avaria NVARCHAR(500) NULL,
        DescricaoReparacao NVARCHAR(1000) NULL,
        Pecas NVARCHAR(500) NULL,
        
        -- Campos específicos para Software
        Software NVARCHAR(200) NULL,
        Necessidade NVARCHAR(500) NULL,
        DescricaoIntervencao NVARCHAR(1000) NULL,
        
        -- Chave estrangeira
        CONSTRAINT FK_Tickets_Utilizadores FOREIGN KEY (CodigoUtilizador) REFERENCES Utilizadores(Codigo)
    );
END
GO

-- Índices para melhor performance
CREATE NONCLUSTERED INDEX IX_Tickets_CodigoUtilizador ON Tickets(CodigoUtilizador);
CREATE NONCLUSTERED INDEX IX_Tickets_EstadoTicket ON Tickets(EstadoTicket);
CREATE NONCLUSTERED INDEX IX_Tickets_DataCriacao ON Tickets(DataCriacao);
CREATE NONCLUSTERED INDEX IX_Tickets_Tipo ON Tickets(Tipo);
GO

-- Inserir dados de exemplo
-- Utilizadores de teste
IF NOT EXISTS (SELECT * FROM Utilizadores WHERE Codigo = 'admin')
BEGIN
    INSERT INTO Utilizadores (Codigo, Nome, Tipo, Password) VALUES 
    ('admin', 'Administrador Sistema', 'TecnicoHelpdesk', 'admin123'),
    ('tecnico1', 'João Silva', 'TecnicoHelpdesk', 'tecnico123'),
    ('user1', 'Maria Santos', 'UtilizadorComum', 'user123'),
    ('user2', 'Pedro Costa', 'UtilizadorComum', 'user456'),
    ('user3', 'Ana Ferreira', 'UtilizadorComum', 'user789');
END
GO

-- Tickets de exemplo
IF NOT EXISTS (SELECT * FROM Tickets WHERE Id = 1)
BEGIN
    -- Tickets de Hardware
    INSERT INTO Tickets (CodigoUtilizador, EstadoTicket, EstadoAtendimento, Tipo, Equipamento, Avaria, DataCriacao) VALUES
    ('user1', 'PorAtender', 'Aberto', 'Hardware', 'Computador Dell OptiPlex', 'Não liga', DATEADD(day, -5, GETDATE())),
    ('user2', 'PorAtender', 'Aberto', 'Hardware', 'Impressora HP LaserJet', 'Papel encravado frequentemente', DATEADD(day, -3, GETDATE())),
    ('user3', 'EmAtendimento', 'Aberto', 'Hardware', 'Monitor Samsung 24"', 'Ecrã com riscos', DATEADD(day, -2, GETDATE()));

    -- Tickets de Software
    INSERT INTO Tickets (CodigoUtilizador, EstadoTicket, EstadoAtendimento, Tipo, Software, Necessidade, DataCriacao) VALUES
    ('user1', 'PorAtender', 'Aberto', 'Software', 'Microsoft Office', 'Instalação do Excel em novo computador', DATEADD(day, -4, GETDATE())),
    ('user2', 'Atendido', 'Resolvido', 'Software', 'Antivírus', 'Atualização da licença', DATEADD(day, -7, GETDATE())),
    ('user3', 'PorAtender', 'Aberto', 'Software', 'Adobe Acrobat', 'Erro ao abrir ficheiros PDF', DATEADD(day, -1, GETDATE()));
END
GO

-- Atualizar alguns tickets com informações de atendimento
UPDATE Tickets 
SET DataAtendimento = DATEADD(hour, -2, GETDATE()),
    EstadoTicket = 'EmAtendimento'
WHERE EstadoTicket = 'EmAtendimento';

UPDATE Tickets 
SET DataAtendimento = DATEADD(day, -2, GETDATE()),
    DescricaoIntervencao = 'Licença renovada com sucesso. Sistema atualizado.'
WHERE EstadoTicket = 'Atendido' AND Tipo = 'Software';

GO

-- Views úteis para relatórios
CREATE VIEW vw_TicketsResumo AS
SELECT 
    t.Id,
    t.DataCriacao,
    t.DataAtendimento,
    u.Nome as NomeUtilizador,
    t.EstadoTicket,
    t.EstadoAtendimento,
    t.Tipo,
    CASE 
        WHEN t.Tipo = 'Hardware' THEN t.Equipamento + ' - ' + t.Avaria
        ELSE t.Software + ' - ' + t.Necessidade
    END as Descricao,
    CASE 
        WHEN t.DataAtendimento IS NOT NULL 
        THEN DATEDIFF(hour, t.DataCriacao, t.DataAtendimento)
        ELSE NULL
    END as TempoAtendimentoHoras
FROM Tickets t
INNER JOIN Utilizadores u ON t.CodigoUtilizador = u.Codigo;
GO

-- Stored Procedures úteis
CREATE PROCEDURE sp_ObterEstatisticasGerais
AS
BEGIN
    SELECT 
        COUNT(*) as TotalTickets,
        SUM(CASE WHEN EstadoTicket = 'PorAtender' THEN 1 ELSE 0 END) as TicketsPorAtender,
        SUM(CASE WHEN EstadoTicket = 'EmAtendimento' THEN 1 ELSE 0 END) as TicketsEmAtendimento,
        SUM(CASE WHEN EstadoTicket = 'Atendido' THEN 1 ELSE 0 END) as TicketsAtendidos,
        SUM(CASE WHEN EstadoAtendimento = 'Resolvido' THEN 1 ELSE 0 END) as TicketsResolvidos,
        AVG(CASE 
            WHEN DataAtendimento IS NOT NULL 
            THEN CAST(DATEDIFF(hour, DataCriacao, DataAtendimento) AS FLOAT)
            ELSE NULL
        END) as MediaTempoAtendimentoHoras
    FROM Tickets;
END
GO

CREATE PROCEDURE sp_ObterEstatisticasPorTipo
AS
BEGIN
    SELECT 
        Tipo,
        COUNT(*) as Total,
        SUM(CASE WHEN EstadoTicket = 'Atendido' THEN 1 ELSE 0 END) as Atendidos,
        SUM(CASE WHEN EstadoAtendimento = 'Resolvido' THEN 1 ELSE 0 END) as Resolvidos,
        AVG(CASE 
            WHEN DataAtendimento IS NOT NULL 
            THEN CAST(DATEDIFF(hour, DataCriacao, DataAtendimento) AS FLOAT)
            ELSE NULL
        END) as MediaTempoAtendimentoHoras
    FROM Tickets
    GROUP BY Tipo;
END
GO

PRINT 'Base de dados Ticket2Help criada com sucesso!';
PRINT 'Utilizadores de teste:';
PRINT '  admin/admin123 (Técnico)';
PRINT '  tecnico1/tecnico123 (Técnico)';
PRINT '  user1/user123 (Utilizador)';
PRINT '  user2/user456 (Utilizador)';
PRINT '  user3/user789 (Utilizador)';