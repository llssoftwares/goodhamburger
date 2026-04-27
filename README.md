# Good Hamburger

Sistema de gerenciamento de pedidos para uma hamburgueria, desenvolvido como desafio técnico utilizando .NET 10, Clean Architecture e Blazor.

## Visão Geral

A aplicação permite gerenciar pedidos de forma completa, incluindo criação, atualização, listagem e remoção. Também contempla o gerenciamento de itens do cardápio e cálculo automático de descontos progressivos com base na quantidade de itens.

A solução foi estruturada com foco em separação de responsabilidades, testabilidade e facilidade de evolução.

---

## Instruções de Execução

### Pré-requisitos

* .NET 10 SDK
* Docker

---

### Execução com .NET Aspire

Na raiz do projeto:

```bash
dotnet run --project src/GoodHamburger.AppHost/GoodHamburger.AppHost.csproj
```

O Aspire é responsável por orquestrar toda a aplicação, iniciando automaticamente:

* API REST
* Aplicação Web (Blazor)
* Banco de dados PostgreSQL
* pgAdmin
* Dashboard de observabilidade

---

### Acesso aos Recursos

Após a inicialização, os serviços estarão disponíveis nas seguintes URLs:

* API:
  [https://api-goodhamburger.dev.localhost:7536/](https://api-goodhamburger.dev.localhost:7536/)

* Aplicação Web (Blazor):
  [https://web-goodhamburger.dev.localhost:7104/](https://web-goodhamburger.dev.localhost:7104/)

* pgAdmin:
  [http://pgadmin-goodhamburger.dev.localhost:58001/browser/](http://pgadmin-goodhamburger.dev.localhost:58001/browser/)

---

### Documentação da API

A documentação interativa (Scalar) pode ser acessada em:

```
https://api-goodhamburger.dev.localhost:7536/scalar/v1
```

---

### Executar Testes

```bash
dotnet test test/GoodHamburger.Tests/GoodHamburger.Tests.csproj
```

---

## Decisões de Arquitetura

### Clean Architecture

A solução foi organizada em camadas (Domain, Application, Infrastructure e API), com dependências unidirecionais.
Essa abordagem facilita testes, manutenção e evolução do sistema, além de reduzir acoplamento entre regras de negócio e detalhes técnicos.

---

### Entity Framework Core

Foi utilizado como ORM por oferecer:

* Mapeamento tipado
* Migrações versionadas
* Integração nativa com .NET

A configuração das entidades foi feita via Fluent API para manter o `DbContext` limpo e desacoplado.

---

### Minimal APIs

Escolhidas no lugar de controllers tradicionais por:

* Menor complexidade
* Melhor legibilidade
* Menor overhead

Adequado para APIs enxutas como a deste desafio.

---

### Blazor Server

Utilizado para o frontend por permitir:

* Reutilização de C#
* Integração simples com a API
* Desenvolvimento rápido sem necessidade de frameworks JavaScript adicionais

---

### .NET Aspire

Adotado como meio de execução da aplicação:

* Centraliza a orquestração de todos os serviços
* Elimina a necessidade de configuração manual de infraestrutura
* Garante consistência entre ambientes de desenvolvimento
* Facilita debugging e observabilidade via dashboard

---

### Observabilidade com .NET Aspire e OpenTelemetry

A aplicação utiliza o projeto `ServiceDefaults` do .NET Aspire para padronizar observabilidade entre os serviços.

Foram configurados automaticamente:

* Logs estruturados com OpenTelemetry
* Métricas:

  * ASP.NET Core (requisições, latência e throughput)
  * HttpClient
  * Runtime do .NET
* Tracing distribuído:

  * Instrumentação de requisições HTTP (entrada e saída)
  * Correlação entre serviços

Essa abordagem permite observar o comportamento da aplicação em tempo real, sem acoplamento a ferramentas específicas.

---

### Camada de Serviços

A lógica de negócio foi centralizada em serviços como:

* OrderService
* MenuService
* DiscountCalculationService

Isso evita lógica distribuída entre endpoints e facilita testes unitários.

---

### Validação e Regras de Negócio

Foi criado um serviço específico para validações (`OrderValidationService`), evitando acoplamento com a camada de API e permitindo reutilização.

---

### DTOs

Os dados expostos pela API são desacoplados das entidades de domínio, evitando vazamento de regras internas e facilitando evolução do contrato da API.

---

### Tratamento de Erros

Utilização de `ProblemDetails` para respostas padronizadas, melhorando a previsibilidade para o cliente da API.

---

### Testes

Foram implementados testes unitários com foco em:

* Regras de negócio
* Cálculo de descontos
* Validações

Uso de xUnit e Moq para isolamento das dependências.

---

## O Que Foi Deixado de Fora

Alguns pontos não foram implementados por não serem essenciais para o escopo do desafio, mas são importantes em um cenário real:

### Autenticação e Autorização

A API está aberta e não possui controle de acesso.

Possível evolução:

* JWT
* ASP.NET Identity
* Integração com provedores externos

---

### Paginação

As listagens retornam todos os registros.

Possível evolução:

* Paginação por offset ou cursor
* Filtros e ordenação

---

### Cache

Não há uso de cache para cardápio ou pedidos.

Possível evolução:

* Redis para dados frequentemente acessados

---

### CI/CD

Não há pipeline automatizado.

Possível evolução:

* GitHub Actions para build, testes e deploy

---

### Segurança

Não foram implementados:

* Rate limiting
* Security headers

---

### Internacionalização

A aplicação está apenas em português e sem suporte a múltiplos idiomas.

---

## Considerações Finais

A solução foi construída com foco em organização, clareza e boas práticas, priorizando uma base sólida que permita evolução futura. Algumas decisões foram intencionalmente simplificadas para manter o escopo adequado ao desafio técnico, sem comprometer a qualidade estrutural do código.