/*! \mainpage
# 🎫 Ticket2Help
## Sistema de Gestão de Tickets de Helpdesk

[![.NET](https://img.shields.io/badge/.NET-8.0-blue)](https://dotnet.microsoft.com/)
[![WPF](https://img.shields.io/badge/WPF-Windows%20Presentation%20Foundation-lightblue)](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-Database-red)](https://www.microsoft.com/en-us/sql-server)
[![C#](https://img.shields.io/badge/C%23-Programming%20Language-green)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Status](https://img.shields.io/badge/Status-Production%20Ready-brightgreen)]()

---

## 📋 **Índice**
- [📖 Sobre o Projeto](#-sobre-o-projeto)
- [🏗️ Arquitetura](#️-arquitetura)
- [🚀 Funcionalidades](#-funcionalidades)
- [💻 Tecnologias](#-tecnologias)
- [🎯 Padrões de Design](#-padrões-de-design)
- [⚙️ Instalação](#️-instalação)
- [🎮 Como Usar](#-como-usar)
- [🧪 Testes](#-testes)
- [📊 Base de Dados](#-base-de-dados)
- [🔧 Troubleshooting](#-troubleshooting)
- [👥 Equipa](#-equipa)

---

## 📖 **Sobre o Projeto**

O **Ticket2Help** é um sistema completo de gestão de tickets de helpdesk desenvolvido em C# com WPF, implementando uma arquitetura robusta em 3 camadas com múltiplos padrões de design para a unidade curricular de **Programação Avançada** do **ISLA Gaia**.

### 🎯 **Objetivos Atingidos**
- ✅ Sistema completo de gestão de tickets (Hardware e Software)
- ✅ Arquitetura em 3 camadas bem estruturada (UI, BLL, DAL)
- ✅ Implementação de 5+ padrões de design
- ✅ Interface moderna e responsiva em WPF
- ✅ Base de dados SQL Server otimizada
- ✅ Sistema de autenticação e autorização
- ✅ Dashboard com estatísticas em tempo real
- ✅ Relatórios e exportação de dados
- ✅ Testes unitários abrangentes

### 🏢 **Contexto Empresarial**
Sistema interno para gestão eficiente de solicitações de TI, permitindo fluxo completo desde criação até resolução de tickets, com diferentes níveis de acesso e funcionalidades avançadas de relatórios.

---

## 🏗️ **Arquitetura**

### 📐 **Arquitetura em 3 Camadas + MVC**

```
🎨 Presentation Layer (Ticket2Help.UI)
├── 🖼️ Views/ (XAML Windows)
│   ├── LoginWindow - Autenticação segura
│   ├── MainWindow - Interface principal
│   ├── CriarTicketWindow - Criação de tickets
│   ├── AtenderTicketWindow - Atendimento
│   └── RelatoriosWindow - Relatórios avançados
├── 🎮 Controllers/ (MVC Pattern)
│   ├── LoginController - Gestão de autenticação
│   └── TicketController - Lógica de apresentação
├── 📊 ViewModels/ (MVVM Support)
│   ├── TicketViewModel - Apresentação de tickets
│   └── DashboardViewModel - Métricas do dashboard
└── 🎨 Resources/ - Estilos e recursos visuais

🧠 Business Logic Layer (Ticket2Help.BLL)
├── 🔧 Services/
│   ├── TicketService - Lógica principal de tickets
│   └── UtilizadorService - Gestão de utilizadores
├── 👔 Managers/
│   └── GestorAtendimento - Estratégias de atendimento
└── 🎯 Patterns/ - Implementação de Design Patterns
    ├── Factory/ - Criação de tickets
    ├── Strategy/ - Estratégias de atendimento
    ├── Observer/ - Notificações de mudanças

💾 Data Access Layer (Ticket2Help.DAL)
├── 🔌 Interfaces/ - Contratos de repositórios
├── 🗃️ Repositories/ - Implementações ADO.NET
│   ├── TicketRepository - CRUD de tickets
│   ├── UtilizadorRepository - Gestão de utilizadores
│   └── RelatorioRepository - Queries de relatórios
└── 🔗 Connection/ - Gestão de ligações BD

📦 Models Layer (Ticket2Help.Models)
├── 🏗️ Entities/
│   ├── Ticket (Abstract) - Classe base
│   ├── HardwareTicket - Tickets de hardware
│   ├── SoftwareTicket - Tickets de software
│   └── Utilizador - Utilizadores do sistema
└── 📋 Enums/ - Enumerações do domínio
```

---

## 🚀 **Funcionalidades Implementadas**

### 👤 **Colaboradores (Utilizadores Base)**
- ✅ **Autenticação segura** com validação em BD
- ✅ **Criação de tickets Hardware** (equipamento + avaria)
- ✅ **Criação de tickets Software** (aplicação + necessidade)
- ✅ **Visualização histórico** dos próprios tickets
- ✅ **Acompanhamento de estados** em tempo real
- ✅ **Interface intuitiva** com design moderno

### 🔧 **Técnicos de Helpdesk (Privilégios Avançados)**
- ✅ **Todas as funcionalidades** de colaborador
- ✅ **Gestão de atendimento** com múltiplas estratégias:
  - 🔄 FIFO (First In, First Out)
  - ⚡ Prioridade Hardware
- ✅ **Atendimento completo** com fluxo bi-modal:
  - 🚀 Iniciar atendimento (marca como "emAtendimento")
  - ✅ Finalizar atendimento (registo detalhado + resolução)
- ✅ **Dashboard executivo** com métricas em tempo real
- ✅ **Sistema de relatórios** avançado com filtros
- ✅ **Exportação CSV** com estatísticas integradas

### 📊 **Dashboard e Analytics**
- 📈 **Métricas em tempo real**:
  - Tickets criados hoje
  - Tickets pendentes e em atendimento
  - Percentagem de tickets atendidos
  - Taxa de resolução vs. não resolução
  - Tempo médio de atendimento (Hardware vs. Software)
- 📊 **Indicadores visuais** com barras de progresso
- 🔄 **Atualização automática** a cada 2 minutos
- 📋 **Relatórios filtráveis** por período
- 💾 **Exportação completa** com metadados

---

## 💻 **Stack Tecnológico**

| Camada | Tecnologia | Versão | Justificação |
|--------|------------|--------|--------------|
| **Frontend** | WPF + XAML | .NET 8.0 | Interface nativa Windows rica |
| **Backend** | C# | .NET 8.0 | Performance e ecosistema robusto |
| **Base de Dados** | SQL Server | 2019+ | Escalabilidade e confiabilidade empresarial |
| **Data Access** | ADO.NET | Nativo | Controle total sobre queries e performance |
| **Testes** | MSTest | 3.0+ | Framework oficial Microsoft |
| **Arquitetura** | 3-Layer + MVC | Custom | Separação clara de responsabilidades |

---

## 🎯 **Padrões de Design Implementados**

### 🏭 **Factory Pattern** - Criação de Tickets
```csharp
// Fábrica polimórfica para diferentes tipos de tickets
var factory = new TicketFactory();
var dadosHardware = new Dictionary<string, object> {
    ["codigoColaborador"] = "COL001",
    ["equipamento"] = "Dell OptiPlex 7090",
    ["avaria"] = "Computador não liga após queda de energia"
};
Ticket ticket = factory.CriarTicket(TipoTicket.Hardware, dadosHardware);
```

### 📈 **Strategy Pattern** - Estratégias de Atendimento
```csharp
// Sistema flexível de priorização de tickets
var gestor = new GestorAtendimento(new FIFOStrategy());
var ticketsOrdenados = gestor.ObterTicketsParaAtendimento(todosTickets);

// Mudança dinâmica de estratégia
gestor.DefinirEstrategia(new PrioridadeHardwareStrategy());
```

### 👀 **Observer Pattern** - Notificações de Estado
```csharp
// Sistema de notificações reativo
ticketService.AdicionarObserver(new LogObserver());
ticketService.AdicionarObserver(new UINotificationObserver());

// Mudanças automáticas notificam todos os observadores
ticketService.AlterarEstadoTicket(ticketId, EstadoTicket.emAtendimento);
```


### 🎛️ **MVC Pattern** - Separação de Responsabilidades
- **Models**: Entidades de domínio com lógica de negócio
- **Views**: Interface WPF com binding e estilos modernos
- **Controllers**: Mediação e fluxo de dados

---

## ⚙️ **Instalação e Configuração**

### 📋 **Pré-requisitos**
- ✅ Windows 10/11 (64-bit)
- ✅ .NET 8.0 SDK ou Runtime
- ✅ SQL Server 2019+ / SQL Server Express / LocalDB
- ✅ Visual Studio 2022 ou VS Code (opcional)

### 🛠️ **Instalação Passo-a-Passo**

#### 1️⃣ **Clonar Repositório**
```bash
git clone https://github.com/[usuario]/Ticket2Help.git
cd Ticket2Help
```

#### 2️⃣ **Configurar Base de Dados**
```sql
-- 1. Abrir SQL Server Management Studio (SSMS)
-- 2. Executar o script completo: paste.txt
-- 3. Verificar criação da BD "Ticket2Help"
-- 4. Confirmar dados de teste inseridos
```

#### 3️⃣ **Verificar Connection String**
O sistema tenta automaticamente várias connection strings:
- `SOARES\SQLEXPRESS` (padrão do projeto)
- `.\SQLEXPRESS`
- `(localdb)\MSSQLLocalDB`
- `localhost\SQLEXPRESS`

#### 4️⃣ **Compilar e Executar**
```bash
# Restaurar dependências
dotnet restore

# Compilar solução
dotnet build

# Executar aplicação
dotnet run --project Ticket2Help.UI
```

---

## 🎮 **Guia de Utilização**

### 🔐 **Credenciais de Demonstração**

| Utilizador | Código | Password | Tipo | Permissões |
|------------|--------|----------|------|------------|
| **Administrador** | `ADMIN` | `admin` | Técnico | Dashboard + Atendimento + Relatórios |
| **Técnico** | `TEC001` | `123` | Técnico | Dashboard + Atendimento + Relatórios |
| **Colaborador** | `COL001` | `123` | Standard | Criar e ver próprios tickets |

### 📝 **Fluxo de Utilização**

#### **Para Colaboradores:**
1. **Login** → Inserir código e password
2. **Criar Ticket** → Escolher tipo (Hardware/Software)
3. **Preencher Dados** → Equipamento/Software + Descrição
4. **Acompanhar** → Ver estado na listagem

#### **Para Técnicos:**
1. **Tab Atendimento** → Ver tickets pendentes
2. **Selecionar Estratégia** → FIFO ou Prioridade Hardware
3. **Iniciar Atendimento** → Duplo-clique ou botão "Atender"
4. **Registar Resolução** → Descrição + Estado final
5. **Dashboard** → Monitorizar métricas
6. **Relatórios** → Análise detalhada + Export

### 🔄 **Fluxo de Estados do Ticket**
```
📝 porAtender → 🔧 emAtendimento → ✅ atendido
                                      ├── resolvido
                                      └── naoResolvido
```

---

## 🧪 **Sistema de Testes**

### 🎯 **Executar Testes Unitários**
```bash
# Todos os testes (25 testes implementados)
dotnet test

# Com detalhes de cobertura
dotnet test --collect:"XPlat Code Coverage" --logger trx

# Testes específicos por categoria
dotnet test --filter "TestCategory=Factory"
dotnet test --filter "TestCategory=Strategy"
```

### 📊 **Cobertura de Testes Atual**
- ✅ **Entities**: Classes `Ticket`, `HardwareTicket`, `SoftwareTicket`
- ✅ **Factory Pattern**: Criação polimórfica de tickets
- ✅ **Strategy Pattern**: Algoritmos de ordenação
- ✅ **Validações**: Campos obrigatórios e regras de negócio
- ✅ **Estados**: Transições e fluxos válidos
- ✅ **Performance**: Testes de carga (1000+ tickets)

### 🧪 **Exemplos de Testes Implementados**
```csharp
[TestMethod]
public void HardwareTicket_GetTipoTicket_RetornaHardware()
[TestMethod]
public void TicketFactory_CriarTicketInvalido_DeveLancarExcecao()
[TestMethod]
public void Ticket_FluxoDeEstados_DeveSequenciarCorrectamente()
```

---

## 📊 **Estrutura da Base de Dados**

### 🗃️ **Tabelas Principais**

#### **Utilizadores**
```sql
CREATE TABLE Utilizadores (
    Codigo NVARCHAR(50) PRIMARY KEY,
    Nome NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255),
    PasswordHash NVARCHAR(500) NOT NULL,  -- Nota: Sistema usa password simples para demo
    EhTecnicoHelpdesk BIT NOT NULL DEFAULT 0,
    Activo BIT NOT NULL DEFAULT 1,
    DataCriacao DATETIME2 NOT NULL DEFAULT GETDATE()
)
```

#### **Tickets** (Polimórfica)
```sql
CREATE TABLE Tickets (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TipoTicket NVARCHAR(50) NOT NULL, -- 'Hardware' ou 'Software'
    CodigoColaborador NVARCHAR(50) NOT NULL,
    DataHoraCriacao DATETIME2 NOT NULL DEFAULT GETDATE(),
    Estado NVARCHAR(50) NOT NULL DEFAULT 'porAtender',
    DataHoraAtendimento DATETIME2 NULL,
    EstadoAtendimento NVARCHAR(50) NULL,
    UsuarioResponsavel NVARCHAR(50) NULL,
    
    -- Campos específicos Hardware
    Equipamento NVARCHAR(500) NULL,
    Avaria NTEXT NULL,
    DescricaoReparacao NTEXT NULL,
    Pecas NTEXT NULL,
    
    -- Campos específicos Software
    Software NVARCHAR(500) NULL,
    DescricaoNecessidade NTEXT NULL,
    DescricaoIntervencao NTEXT NULL,
    
    FOREIGN KEY (CodigoColaborador) REFERENCES Utilizadores(Codigo),
    FOREIGN KEY (UsuarioResponsavel) REFERENCES Utilizadores(Codigo)
)
```

### 🔧 **Stored Procedures Otimizadas**
- `sp_CriarTicketHardware` - Criação optimizada de tickets hardware
- `sp_CriarTicketSoftware` - Criação optimizada de tickets software  
- `sp_ActualizarEstadoTicket` - Atualização com transações seguras
- `sp_ObterEstatisticasDashboard` - Queries complexas para métricas

### 📊 **Views para Relatórios**
- `vw_TicketsCompletos` - Junção otimizada com dados de utilizadores
- Campos calculados automáticos (tempo de atendimento, descrições completas)

---

## 🔧 **Troubleshooting e Soluções**

### ❌ **Problemas Comuns e Soluções**

#### **🔌 Erro de Ligação à Base de Dados**
```
Erro: "Cannot open database 'Ticket2Help'" 
```
**Diagnóstico Automático**: O sistema testa múltiplas connection strings
**Soluções**:
1. Verificar se SQL Server está em execução
2. Executar script `paste.txt` completo
3. Verificar instância SQL Server (`SQLEXPRESS`, `LocalDB`)

#### **🔑 Credenciais Inválidas**
```
Erro: "Código ou palavra-passe incorretos"
```
**Soluções**:
1. Usar credenciais de demo (ver tabela acima)
2. Códigos são case-sensitive: usar maiúsculas
3. Verificar se dados de teste foram inseridos na BD

#### **⚡ Performance Lenta**
**Otimizações implementadas**:
- Connection pooling automático
- Stored procedures otimizadas  
- Indexes automáticos na BD
- Timer de atualização configurável (2min padrão)

#### **🖼️ Problemas de Interface**
- **Resolução mínima**: 1000x700
- **Temas**: Sistema adapta automaticamente ao tema Windows
- **DPI**: Suporte automático para High-DPI displays

### 🛠️ **Ferramentas de Diagnóstico**
- **Debug Console**: Logs detalhados em tempo real
- **Connection Tester**: Teste automático de várias connection strings
- **Health Check**: Verificação de integridade na inicialização

---

## 📈 **Funcionalidades Avançadas**

### 🔄 **Sistema de Atualização Automática**
- Timer configurável (padrão: 2 minutos)
- Atualização inteligente (apenas dados alterados)
- Indicadores visuais de última atualização

### 💾 **Sistema de Exportação**
- **CSV com encoding UTF-8** (suporte caracteres especiais)
- **Metadados incluídos** (estatísticas, data geração)
- **Abertura automática** da pasta de destino

### 🎨 **Interface Moderna**
- **Design System** consistente com cores e tipografia
- **Responsive Layout** que adapta a diferentes resoluções
- **Micro-animações** para melhor UX
- **Dark/Light Mode** automático baseado no sistema

### 🔐 **Segurança e Autorização**
- **Controle de acesso** baseado em roles
- **Sessões seguras** com logout automático
- **Validação input** em todas as camadas
- **SQL Injection protection** via parametrização

---

## 🚀 **Roadmap e Extensões Futuras**

### 🔧 **Melhorias Técnicas**
- [ ] **Notificações Push** em tempo real
- [ ] **Sistema de Comentários** nos tickets
- [ ] **Melhorias nos mapas estatísticos** no dashboard

---

## 📁 **Estrutura Detalhada do Projeto**

```
Ticket2Help/ (Solução .NET)
├── 📁 Ticket2Help.Models/               # 🏗️ Camada de Domínio
│   ├── Entities/
│   │   ├── Ticket.cs                    # Classe abstrata base
│   │   ├── HardwareTicket.cs           # Especialização para hardware
│   │   ├── SoftwareTicket.cs           # Especialização para software
│   │   └── Utilizador.cs               # Entidade de utilizadores
│   └── Enums/
│       ├── TipoTicket.cs               # Hardware | Software
│       ├── EstadoTicket.cs             # porAtender | emAtendimento | atendido
│       └── EstadoAtendimento.cs        # aberto | resolvido | naoResolvido
├── 📁 Ticket2Help.DAL/                 # 💾 Camada de Dados
│   ├── Interfaces/
│   │   ├── IDatabaseConnection.cs      # Interface de conexão
│   │   ├── ITicketRepository.cs        # CRUD de tickets
│   │   └── IUtilizadorRepository.cs    # CRUD de utilizadores
│   └── Repositories/
│      ├── TicketRepository.cs         # Implementação ADO.NET tickets
│       ├── UtilizadorRepository.cs     # Implementação ADO.NET users
│       └── RelatorioRepository.cs      # Queries complexas relatórios
│   
├── 📁 Ticket2Help.BLL/                 # 🧠 Camada de Negócio
│   ├── Services/
│   │   ├── TicketService.cs            # Orquestração + Observer Subject
│   │   └── UtilizadorService.cs        # Lógica de autenticação
│   ├── Managers/
│   │   └── GestorAtendimento.cs        # Context para Strategy Pattern
│   └── Patterns/
│       ├── Factory/
│       │   ├── ITicketFactory.cs       # Interface Factory
│       │   └── TicketFactory.cs        # Implementação Factory
│       ├── Strategy/
│       │   ├── IAtendimentoStrategy.cs # Interface Strategy
│       │   ├── FIFOStrategy.cs         # First In First Out
│       │   └── PrioridadeHardwareStrategy.cs # Hardware priority
│       └── Observer/
│           ├── ITicketObserver.cs      # Interface Observer
│           ├── ITicketSubject.cs       # Interface Subject
│           ├── LogObserver.cs          # Observer para logs
│           └── UINotificationObserver.cs # Observer para UI
│       
│        
├── 📁 Ticket2Help.UI/                  # 🎨 Camada de Apresentação
│   ├── Views/
│   │   ├── LoginWindow.xaml/.cs        # Tela de autenticação
│   │   ├── MainWindow.xaml/.cs         # Interface principal
│   │   ├── CriarTicketWindow.xaml/.cs  # Criação de tickets
│   │   ├── AtenderTicketWindow.xaml/.cs # Atendimento de tickets
│   │   └── RelatoriosWindow.xaml/.cs   # Relatórios e exportação
│   ├── Controllers/
│   │   ├── LoginController.cs          # MVC Controller auth
│   │   └── TicketController.cs         # MVC Controller tickets
│   ├── ViewModels/
│   │   ├── TicketViewModel.cs          # ViewModel para tickets
│   │   └── DashboardViewModel.cs       # ViewModel para métricas
│   ├── Resources/
│   │   └── Styles.xaml                 # Design system XAML
│   ├── App.xaml/.cs                    # Configuração da aplicação
│   └── Ticket2Help.UI.csproj          # Configuração do projeto
├── 📁 Ticket2Help.Tests/               # 🧪 Testes Unitários
│   └── UnitTest1.cs                    # 25 testes implementados
├── 📁 Database/               # 🛢 SQL Server
│   └── CreateDatabase.sql                    # Script completo SQL Server
├── 📁 Docs               # 📇 Doxygen
├── 📄 Ticket2Help.sln                  # Solução Visual Studio
└── 📄 README.md                        # Documentação principal
```


## 📜 **Licença e Direitos**

Este projeto foi desenvolvido exclusivamente para fins **académicos e educacionais** no âmbito da disciplina de Programação Avançada do ISLA Gaia.

**⚠️ Importante**: 
- Código disponível para consulta e aprendizagem
- Não destinado a uso comercial
- Credenciais e dados são fictícios
- Respeitar direitos de propriedade intelectual

---

## 🌟 **Agradecimentos**

### 👨‍🏫 **Corpo Docente**
- **Prof. Helder Rodrigo Pinto** - Orientação técnica e pedagógica
- **ISLA Gaia** - Infraestrutura e recursos educacionais

### 🛠️ **Tecnologias e Ferramentas**
- **Microsoft** - Plataforma .NET e ferramentas de desenvolvimento
- **GitHub** - Hospedagem e controlo de versões
- **SQL Server** - Sistema de gestão de base de dados


*/
