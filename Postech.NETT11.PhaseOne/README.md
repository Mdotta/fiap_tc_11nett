# Postech NETT11 - Phase One

Sistema de gerenciamento de jogos desenvolvido como Tech Challenge da Pós-Graduação FIAP - Turma NETT11.

## Equipe

- **Carol Biasi** - carolbiasi - RM 370124
- **Jino Hong** - jinoho_ - RM 370097
- **Mateus Dotta** - dotta - RM 369824
- **Paulo Henrique** - paul0x4f - RM 369760

---

## Links do Projeto

- **Documentação**: [Miro Board](https://miro.com/app/board/uXjVJwHC17w=/?share_link_id=400299830879)
- **Repositório**: [GitHub](https://github.com/Mdotta/fiap_tc_11nett)
- **Vídeo Apresentação**: [Vimeo](https://vimeo.com/1156252139?share=copy&fl=sv&fe=ci)

---

## Sobre o Projeto

Sistema de gerenciamento de jogos digitais que permite o cadastro, consulta, atualização e controle de status de jogos, além de autenticação e autorização de usuários com diferentes níveis de acesso.

### Funcionalidades Principais

- **Gestão de Jogos**: CRUD completo de jogos com informações como título, descrição, desenvolvedor, editora, preço e status
- **Autenticação JWT**: Sistema de autenticação seguro baseado em tokens JWT
- **Autorização por Roles**: Controle de acesso com diferentes níveis (Admin e Client)
- **Logging Centralizado**: Integração com ELK Stack (Elasticsearch + Kibana) para monitoramento e análise de logs
- **Rastreabilidade**: Sistema de Correlation ID para rastreamento de requisições
- **Tratamento de Exceções**: Middleware global para tratamento de erros

## Arquitetura

O projeto segue os princípios de **Clean Architecture** e **Domain-Driven Design (DDD)**:

```
├── Domain/              # Entidades, regras de negócio e interfaces de repositórios
├── Application/         # Serviços, DTOs e lógica de aplicação
├── Infrastructure/      # EF Core, repositórios e acesso a dados
├── WebApp/              # API (Minimal APIs), endpoints e middlewares
├── Tests/               # Testes unitários
```

## Tecnologias Utilizadas

- **.NET 9.0**: Framework principal
- **ASP.NET Core**: Web API com Minimal APIs
- **Entity Framework Core**: ORM para acesso a dados
- **SQL Server 2022**: Banco de dados relacional
- **JWT (JSON Web Tokens)**: Autenticação e autorização
- **Serilog**: Framework de logging estruturado
- **Elasticsearch**: Armazenamento e indexação de logs
- **Kibana**: Visualização e análise de logs
- **Docker & Docker Compose**: Containerização e orquestração
- **xUnit**: Framework de testes

## Pré-requisitos

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Git](https://git-scm.com/)

---

## Como Executar o Projeto

### 1. Clonar o Repositório

```bash
git clone https://github.com/Mdotta/fiap_tc_11nett.git
cd fiap_tc_11nett/Postech.NETT11.PhaseOne
```

### 2. Configurar Variáveis de Ambiente

Copie o arquivo `.env.example` para `.env` e configure as variáveis:

```bash
cp .env.example .env
```

Edite o arquivo `.env` e configure:

```env
# JWT Key - Gere um GUID único
JWT_KEY=seu-guid-aqui

# Senha do SQL Server (mínimo 8 caracteres com maiúsculas, minúsculas, números e símbolos)
SA_PASSWORD=Str0ngP@ssword!
```

### 3. Executar com Docker Compose

```bash
docker-compose up -d
```

Isso iniciará todos os serviços:
- **API**: http://localhost:8080
- **SQL Server**: localhost:2022
- **Elasticsearch**: http://localhost:9200
- **Kibana**: http://localhost:5601

### 4. Verificar os Serviços

Aguarde alguns segundos para os serviços iniciarem completamente. Você pode verificar os logs:

```bash
docker-compose logs -f api
```

A API estará disponível com documentação OpenAPI em: http://localhost:8080/scalar/v1

---

## Desenvolvimento Local (Sem Docker)

### 1. Configurar SQL Server

Certifique-se de ter um SQL Server rodando e configure a connection string no `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=postech;User Id=sa;Password=SuaSenha;TrustServerCertificate=True;"
  }
}
```

### 2. Configurar User Secrets

Para desenvolvimento local, configure o JWT Key usando user secrets:

```bash
cd Postech.NETT11.PhaseOne.WebApp
dotnet user-secrets set "Jwt:Key" "seu-guid-aqui"
```

### 3. Aplicar Migrações

```bash
cd Postech.NETT11.PhaseOne.WebApp
dotnet ef database update
```

### 4. Executar a Aplicação

```bash
cd Postech.NETT11.PhaseOne.WebApp
dotnet run
```

## Executar Testes

### Testes Unitários

```bash
cd Postech.NETT11.PhaseOne.Tdd
dotnet test
```

---

## Endpoints da API

### Autenticação

- `POST /auth/login` - Realizar login e obter token JWT
- `POST /auth/register` - Registrar novo usuário

### Usuários

- `GET /users` - Listar todos os usuários (Admin)
- `GET /users/{id}` - Obter usuário por ID
- `PUT /users/{id}` - Atualizar usuário
- `DELETE /users/{id}` - Excluir usuário (Admin)

### Jogos

- `GET /games` - Listar todos os jogos
- `GET /games/{id}` - Obter jogo por ID
- `POST /games` - Criar novo jogo (Admin)
- `PUT /games/{id}` - Atualizar jogo (Admin)
- `DELETE /games/{id}` - Excluir jogo (Admin)
- `PATCH /games/{id}/status` - Atualizar status do jogo (Admin)

---

## Autenticação e Autorização

O sistema utiliza JWT (JSON Web Tokens) para autenticação. Para acessar endpoints protegidos:

1. Faça login via `/auth/login` para obter o token
2. Adicione o token no header das requisições:
   ```
   Authorization: Bearer {seu-token}
   ```

### Roles Disponíveis

- **Admin**: Acesso total ao sistema (CRUD de jogos e usuários)
- **Client**: Acesso de leitura aos jogos

---

## Monitoramento e Logs

### Kibana

Acesse o Kibana em http://localhost:5601 para visualizar os logs da aplicação.

#### Configurar Index Pattern no Kibana

1. Acesse Kibana: http://localhost:5601
2. Vá em **Management** > **Stack Management** > **Index Patterns**
3. Clique em **Create index pattern**
4. Digite `logs-*` no campo de padrão
5. Selecione `@timestamp` como campo de tempo
6. Clique em **Create index pattern**

Agora você pode visualizar os logs em **Discover**.

### Correlation ID

Todas as requisições recebem um Correlation ID único para rastreamento. Esse ID é:
- Retornado no header `X-Correlation-Id` da resposta
- Incluído em todos os logs relacionados à requisição
- Útil para debugging e análise de fluxos completos

## Comandos Docker Úteis

```bash
# Iniciar todos os serviços
docker-compose up -d

# Ver logs de todos os serviços
docker-compose logs -f

# Ver logs de um serviço específico
docker-compose logs -f api

# Parar todos os serviços
docker-compose down

# Parar e remover volumes (limpar dados)
docker-compose down -v

# Reconstruir imagens
docker-compose build --no-cache

# Reiniciar um serviço específico
docker-compose restart api
```

## Documentação Adicional

- [Documentação do Projeto (Miro)](https://miro.com/app/board/uXjVJwHC17w=/?share_link_id=400299830879)

## Licença

Este projeto foi desenvolvido para fins educacionais como parte do Tech Challenge da Pós-Graduação FIAP.


