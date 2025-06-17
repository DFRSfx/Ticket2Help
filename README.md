# 🎫 Ticket2Help
## Sistema de Gestão de Tickets de Helpdesk

[![.NET](https://img.shields.io/badge/.NET-8.0-blue)](https://dotnet.microsoft.com/)
[![WPF](https://img.shields.io/badge/WPF-Windows%20Presentation%20Foundation-lightblue)](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-Database-red)](https://www.microsoft.com/en-us/sql-server)
[![C#](https://img.shields.io/badge/C%23-Programming%20Language-green)](https://docs.microsoft.com/en-us/dotnet/csharp/)

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
- [📊 Screenshots](#-screenshots)
- [👥 Equipa](#-equipa)
- [📜 Licença](#-licença)

---

## 📖 **Sobre o Projeto**

O **Ticket2Help** é um sistema completo de gestão de tickets de helpdesk desenvolvido em C# com WPF para a unidade curricular de **Programação Avançada** do curso de **Licenciatura em Engenharia Informática** no **ISLA Gaia**.

### 🎯 **Objetivos**
- Gerir tickets de suporte técnico (Hardware e Software)
- Implementar arquitetura em 3 camadas (UI, BLL, DAL)
- Aplicar padrões de design de software
- Seguir metodologias de documentação e testes
- Utilizar controlo de versões com GitHub

### 🏢 **Contexto**
A empresa fictícia **Ticket2Help** necessita de uma plataforma interna para gerir solicitações de serviços de TI, permitindo aos colaboradores criar tickets e aos técnicos atendê-los de forma eficiente.

---

## 🏗️ **Arquitetura**

O sistema segue uma **arquitetura em 3 camadas** com padrão **MVC**:

```
📱 UI Layer (Presentation)
├── Views (WPF Windows)
├── Controllers (MVC)
└── ViewModels

🧠 BLL Layer (Business Logic)
├── Services
├── Managers
└── Design Patterns

💾 DAL Layer (Data Access)
├── Repositories
├── Interfaces
└── Database Connection

📦 Models Layer
├── Entities
├── Enums
└── DTOs
```

---

## 🚀 **Funcionalidades**

### 👤 **Para Colaboradores**
- ✅ Login seguro no sistema
- ✅ Criar tickets de Hardware (equipamento, avaria)
- ✅ Criar tickets de Software (aplicação, necessidade)
- ✅ Visualizar histórico dos seus tickets
- ✅ Acompanhar estado dos tickets

### 🔧 **Para Técnicos de Helpdesk**
- ✅ Todas as funcionalidades de colaborador
- ✅ Ver lista de tickets pendentes
- ✅ Atender tickets com diferentes estratégias
- ✅ Registar reparações e intervenções
- ✅ Dashboard com estatísticas em tempo real
- ✅ Gerar relatórios detalhados
- ✅ Exportar dados para CSV

### 📊 **Dashboard e Relatórios**
- ✅ Percentagem de tickets atendidos
- ✅ Taxa de resolução de problemas
- ✅ Tempo médio de atendimento por tipo
- ✅ Estatísticas visuais em tempo real
- ✅ Relatórios filtrados por período
- ✅ Exportação de dados

---

## 💻 **Tecnologias**

| Camada | Tecnologia | Versão |
|--------|------------|--------|
| **Frontend** | WPF (Windows Presentation Foundation) | .NET 8.0 |
| **Backend** | C# | .NET 8.0 |
| **Base de Dados** | SQL Server | 2019+ |
| **ORM** | ADO.NET | Nativo |
| **Testes** | MSTest | 3.0+ |
| **Documentação** | Doxygen | 1.9+ |
| **Controlo Versões** | Git + GitHub | - |

---

## 🎯 **Padrões de Design**

### 🏭 **Factory Pattern**
```csharp
ITicketFactory factory = new TicketFactory();
Ticket ticket = factory.CriarTicket(TipoTicket.Hardware, dados);
```

### 📈 **Strategy Pattern**
```csharp
GestorAtendimento gestor = new GestorAtendimento(new FIFOStrategy());
gestor.DefinirEstrategia(new PrioridadeHardwareStrategy());
```

### 👀 **Observer Pattern**
```csharp
ticketService.AdicionarObserver(new LogObserver());
ticketService.AdicionarObserver(new UINotificationObserver());
```

### 🔒 **Singleton Pattern**
```csharp
var config = ConfiguracaoSistema.Instance;
var connection = DatabaseConnection.Instance;
```

### 🎛️ **MVC Pattern**
- **Model**: Entidades e lógica de negócio
- **View**: Interface WPF
- **Controller**: Mediação entre View e Model

---

## ⚙️ **Instalação**

### 📋 **Pré-requisitos**
- Windows 10/11
- .NET 8.0 SDK
- SQL Server 2019+ ou SQL Server Express
- Visual Studio 2022 (recomendado)

### 🛠️ **Passos de Instalação**

1. **Clonar o repositório**
```bash
git clone https://github.com/[usuario]/Ticket2Help.git
cd Ticket2Help
```

2. **Configurar Base de Dados**
```sql
-- Executar script SQL no SQL Server Management Studio
-- Localização: /Database/CreateDatabase.sql
```

3. **Configurar Connection String**
```csharp
// Editar em DatabaseConnection.cs se necessário
ConnectionString = "Data Source=SERVIDOR\\SQLEXPRESS;Initial Catalog=Ticket2Help;Integrated Security=True;Trust Server Certificate=True";
```

4. **Restaurar Dependências**
```bash
dotnet restore
```

5. **Compilar Solução**
```bash
dotnet build
```

6. **Executar Aplicação**
```bash
dotnet run --project Ticket2Help.UI
```

---

## 🎮 **Como Usar**

### 🔐 **Login**
Utilize uma das contas de demonstração:

| Tipo | Código | Password | Permissões |
|------|--------|----------|------------|
| **Administrador** | ADMIN | admin | Técnico Helpdesk |
| **Técnico** | TEC001 | 123 | Técnico Helpdesk |
| **Colaborador** | COL001 | 123 | Utilizador Normal |

### 📝 **Criar Ticket**
1. Na tab "📋 Os Meus Tickets"
2. Clicar em "🔧 Criar Ticket Hardware" ou "💻 Criar Ticket Software"
3. Preencher informações obrigatórias
4. Confirmar criação

### 🛠️ **Atender Ticket** (Apenas Técnicos)
1. Na tab "🛠️ Atendimento"
2. Seleccionar estratégia de atendimento
3. Clicar "✅ Atender" ou duplo-clique no ticket
4. Preencher detalhes da resolução
5. Definir estado final

### 📊 **Ver Estatísticas** (Apenas Técnicos)
1. Na tab "📊 Dashboard"
2. Visualizar métricas em tempo real
3. Clicar "📈 Relatórios Detalhados" para análise avançada

---

## 🧪 **Testes**

### 🎯 **Executar Testes**
```bash
# Todos os testes
dotnet test

# Com relatório de cobertura
dotnet test --collect:"XPlat Code Coverage"

# Testes específicos
dotnet test --filter "TestCategory=UnitTest"
```

### 📊 **Cobertura de Testes**
- ✅ Classe `Ticket` e derivadas
- ✅ Factory Pattern
- ✅ Strategy Pattern
- ✅ Gestão de estados
- ✅ Validações de dados

---

## 📊 **Screenshots**

### 🔐 **Tela de Login**
![Login](docs/screenshots/login.png)

### 🏠 **Dashboard Principal**
![Dashboard](docs/screenshots/dashboard.png)

### 📝 **Criação de Ticket**
![Criar Ticket](docs/screenshots/criar-ticket.png)

### 🛠️ **Atendimento**
![Atender Ticket](docs/screenshots/atender-ticket.png)

### 📊 **Relatórios**
![Relatórios](docs/screenshots/relatorios.png)

---

## 📁 **Estrutura do Projeto**

```
Ticket2Help/
├── 📁 Ticket2Help.Models/          # Entidades e Enums
│   ├── Entities/
│   ├── Enums/
│   └── DTOs/
├── 📁 Ticket2Help.DAL/             # Data Access Layer
│   ├── Interfaces/
│   ├── Repositories/
│   └── Connection/
├── 📁 Ticket2Help.BLL/             # Business Logic Layer
│   ├── Services/
│   ├── Managers/
│   └── Patterns/
├── 📁 Ticket2Help.UI/              # User Interface
│   ├── Views/
│   ├── Controllers/
│   ├── ViewModels/
│   └── Resources/
├── 📁 Ticket2Help.Tests/           # Testes Unitários
├── 📁 Database/                    # Scripts SQL
├── 📁 docs/                       # Documentação
└── 📄 README.md
```

---

## 🐛 **Resolução de Problemas**

### ❌ **Erro de Ligação à BD**
```
Erro: "Cannot open database 'Ticket2Help'"
```
**Solução**: Verificar se o SQL Server está a correr e executar script de criação da BD.

### ❌ **Erro de Compilação**
```
Erro: "Target framework not found"
```
**Solução**: Instalar .NET 8.0 SDK.

### ❌ **Credenciais Inválidas**
```
Erro: "Login failed"
```
**Solução**: Utilizar utilizadores de demonstração listados na secção "Como Usar".

---

## 👥 **Equipa**

| Nome | Função | GitHub |
|------|--------|--------|
| [Nome 1] | Desenvolvedor Full-Stack | [@username1](https://github.com/username1) |
| [Nome 2] | Desenvolvedor Backend | [@username2](https://github.com/username2) |

### 📊 **Distribuição de Tarefas**
- **[Nome 1]**: UI/UX, Controllers, Testes
- **[Nome 2]**: Base de Dados, Repositories, Padrões

---

## 📈 **Roadmap Futuro**

### 🚀 **Versão 2.0**
- [ ] Notificações por email
- [ ] API REST para integração
- [ ] Aplicação móvel
- [ ] Chat em tempo real
- [ ] Sistema de aprovações
- [ ] Integração com Active Directory

### 🔧 **Melhorias Técnicas**
- [ ] Entity Framework Core
- [ ] Containerização com Docker
- [ ] CI/CD com GitHub Actions
- [ ] Logs estruturados
- [ ] Cache distribuído

---

## 📚 **Documentação Adicional**

- 📖 [Manual do Utilizador](docs/manual-utilizador.md)
- 🏗️ [Documentação da Arquitetura](docs/arquitetura.md)
- 🎨 [Guia de Estilo](docs/guia-estilo.md)
- 🔧 [Configuração Avançada](docs/configuracao.md)
- 📊 [Relatório Técnico](docs/relatorio-tecnico.pdf)

---

## 📜 **Licença**

Este projeto foi desenvolvido para fins académicos na unidade curricular de **Programação Avançada** do **ISLA Gaia**.

**Docente**: Helder Rodrigo Pinto  
**Instituição**: Instituto Politécnico de Gestão e Tecnologia  
**Ano Letivo**: 2024/2025

---

## 🤝 **Contribuição**

Para contribuir com o projeto:

1. Fork do repositório
2. Criar branch para feature (`git checkout -b feature/nova-funcionalidade`)
3. Commit das alterações (`git commit -m 'Adicionar nova funcionalidade'`)
4. Push para branch (`git push origin feature/nova-funcionalidade`)
5. Criar Pull Request

---

## 📞 **Contacto**

Para questões académicas ou técnicas:
- 📧 Email: [email@aluno.islagaia.pt]
- 🐙 GitHub: [https://github.com/[usuario]/Ticket2Help]
- 🎓 ISLA Gaia: [https://www.islagaia.pt]

---

**⭐ Se gostou do projeto, deixe uma estrela no GitHub!**

---

*Desenvolvido com ❤️ por estudantes do ISLA Gaia*