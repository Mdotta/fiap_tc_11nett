# Testes TDD - FIAP Cloud Games

Este projeto contém os testes unitários desenvolvidos seguindo a metodologia **Test-Driven Development (TDD)** para a plataforma FIAP Cloud Games.

## Estrutura de Testes

### 1. Testes de Usuários (`Usuarios/`)
- **UserValidationTests.cs**: Testes de validação de cadastro de usuários
  - Validação de formato de e-mail
  - Validação de senha segura (mínimo 8 caracteres, números, letras e caracteres especiais)
  - Criação de usuários com diferentes roles (Admin, Client)

### 2. Testes de Jogos (`Jogos/`)
- **HelperTests.cs**: Testes básicos de criação e validação de jogos
  - Criação de jogos com dados válidos
  - Validação de campos obrigatórios (nome, desenvolvedor, distribuidora)
  - Validação de preço (não pode ser negativo)
  - Validação de data de lançamento (não pode ser futura)
  - Validação de categorias

- **JogoServiceTests.cs**: Testes de serviços de gerenciamento de jogos
  - Cadastro de jogos pelo administrador
  - Validação de permissões
  - Desativação de jogos
  - Atualização de preços
  - Múltiplas categorias

### 3. Testes de Compras (`Compras/`)
- **PurchaseTests.cs**: Testes básicos de criação de compras
  - Criação de compras válidas
  - Validação de dados obrigatórios
  - Validação de valores

- **PurchaseServiceTests.cs**: Testes de serviços de compra
  - Compra de jogos ativos
  - Prevenção de compra de jogos inativos
  - Prevenção de compra duplicada
  - Registro correto de valores e datas
  - Suporte a jogos gratuitos

### 4. Testes de Biblioteca (`Biblioteca/`)
- **LibraryTests.cs**: Testes de biblioteca de jogos do usuário
  - Criação de biblioteca para usuário
  - Adição de jogos na biblioteca
  - Verificação de jogos na biblioteca
  - Contagem de jogos
  - Prevenção de duplicatas

## Executando os Testes

### Via Visual Studio
1. Abra o Test Explorer (Test > Test Explorer)
2. Execute todos os testes ou testes específicos

### Via Terminal
```bash
dotnet test Postech.NETT11.PhaseOne.Tdd/Postech.NETT11.PhaseOne.Tests.csproj
```

### Com cobertura de código
```bash
dotnet test --collect:"XPlat Code Coverage"
```

## Dependências de Testes

- **xunit**: Framework de testes
- **xunit.runner.visualstudio**: Runner para Visual Studio
- **Moq**: Framework de mocking (para testes futuros com dependências)
- **FluentAssertions**: Biblioteca de asserções mais legíveis

## Metodologia TDD

Os testes foram desenvolvidos seguindo o ciclo TDD:

1. **Red**: Escrever um teste que falha
2. **Green**: Implementar o código mínimo para passar no teste
3. **Refactor**: Melhorar o código mantendo os testes passando

## Cobertura de Testes

Os testes cobrem:
- ✅ Validação de cadastro de usuários (e-mail e senha)
- ✅ Cadastro de jogos pelo administrador
- ✅ Validações de regras de negócio de jogos
- ✅ Compra de jogos pelo usuário
- ✅ Biblioteca de jogos do usuário
- ✅ Prevenção de compras duplicadas
- ✅ Validações de dados obrigatórios

## Próximos Passos

- [ ] Implementar testes de integração
- [ ] Adicionar testes de performance
- [ ] Implementar testes de autenticação e autorização
- [ ] Adicionar testes de endpoints da API
- [ ] Implementar testes com mocks para repositórios
