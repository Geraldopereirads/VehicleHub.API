# VehicleHub.API

## Visão Geral

O **VehicleHub.API** é uma API desenvolvida em **.NET (Minimal API)** para o gerenciamento de veículos de uma concessionária.  
O projeto foi pensado para ser **simples, escalável e bem estruturado**, focando em autenticação, autorização por perfil e operações CRUD, sem a complexidade de um DDD completo.

A aplicação utiliza **JWT para autenticação**, **Entity Framework Core** como ORM e **SQL Server** como banco de dados.

---

## Objetivo do Projeto

Permitir que usuários autenticados possam:

- Realizar login e receber um token JWT
- Acessar e gerenciar veículos conforme seu perfil (Admin ou Editor)
- Consumir endpoints protegidos de forma segura

O projeto também serve como **estudo prático de arquitetura**, segurança com JWT e boas práticas no backend.

---

## Arquitetura

O projeto se inspira em conceitos de **Clean Architecture** e **Onion Architecture**, porém de forma simplificada:

- Não há separação em múltiplos projetos
- Uso de **Minimal API**
- Camadas bem definidas por pastas




---

## Autenticação e Autorização

A aplicação utiliza **JWT (JSON Web Token)** para controle de acesso.

### Perfis de Usuário

- **Administrador (Admin)**
  - Acesso total à aplicação
  - Pode criar novos administradores
  - Gerencia veículos e usuários

- **Editor**
  - Acesso apenas às rotas permitidas pelo administrador
  - Não pode criar novos administradores

As permissões são aplicadas diretamente nas rotas utilizando políticas de autorização.

---

## Funcionalidades

### Autenticação

- [x] Login com email e senha e Perfil
- [x] Geração de token JWT

### Administradores

- [x] Criar administrador
- [x] Listar todos os administradores
- [x] Buscar administrador por ID


### Veículos

- [x] Criar veículo
- [x] Listar veículos com paginação
- [x] Buscar veículo por ID
- [x] Atualizar veículo
- [x] Remover veículo

---

## Migrations e Atualização do Banco de Dados

O projeto utiliza **Entity Framework Core** para gerenciamento do banco de dados e versionamento do schema através de **migrations**.

Antes de executar os comandos, certifique-se de que:

- A **connection string** está corretamente configurada
- O projeto está definido como **Startup Project**
- O pacote `Microsoft.EntityFrameworkCore.Tools` está instalado

---

### Criar uma Nova Migration

Utilize estes comandos sempre que houver alterações nas entidades:

```bash
dotnet ef migrations add NomeDaMigration
```

```bash
dotnet ef database update
```

```bash
dotnet ef migrations remove
```

```bash
dotnet ef migrations list
```
