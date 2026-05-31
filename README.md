# 📚 API Restful - Sistema de Livraria

Uma robusta API Restful desenvolvida em **C# (.NET Core)** para o gerenciamento de um catálogo de livros. O sistema conta com uma arquitetura dividida em camadas (Controllers, Services e Repositories) e implementa um sistema completo de **Autenticação e Autorização utilizando JWT (JSON Web Tokens)** e criptografia de senhas com **BCrypt**.

## ✨ Funcionalidades

* **Gerenciamento de Livros (CRUD):** Criação, listagem (com filtro opcional por título), atualização (total via PUT ou parcial via PATCH) e exclusão de exemplares.
* **Autenticação Segura:** Registro de novos usuários com hash de senhas (BCrypt) e login gerando tokens JWT válidos por 2 horas.
* **Controle de Acesso (RBAC):** Proteção de rotas baseada em perfis (Roles). Exemplo: Apenas usuários com o perfil `AGENTE` ou `ADMIN` podem deletar livros.
* **Middleware Customizado:** Validação de tokens e injeção do usuário logado no contexto da requisição (`JwtMiddleware`).
* **Documentação Viva:** Totalmente documentada utilizando **Swagger** (OpenAPI), permitindo testes práticos direto no navegador.

---

## 🛠️ Tecnologias Utilizadas

* **Linguagem/Framework:** C# / ASP.NET Core Web API
* **ORM (Banco de Dados):** Entity Framework Core (`Microsoft.EntityFrameworkCore`)
* **Segurança e Criptografia:** JWT (`System.IdentityModel.Tokens.Jwt`) e `BCrypt.Net`
* **Documentação:** Swashbuckle (Swagger)
* **Padrões de Projeto:** Repository Pattern, Dependency Injection (Injeção de Dependência) e Service Pattern.

---

## 🛣️ Rotas da API (Endpoints)

### 🔐 Autenticação (`/api/auth`)

| Método | Rota | Descrição | Requer Token? |
| --- | --- | --- | --- |
| **POST** | `/registrar` | Cria um novo usuário e retorna o Token JWT. | Não |
| **POST** | `/login` | Autentica o usuário e gera o Token JWT. | Não |

### 📖 Livros (`/api/livros`)

| Método | Rota | Descrição | Requer Token? | Restrição (Role) |
| --- | --- | --- | --- | --- |
| **GET** | `/` | Retorna todos os livros (aceita `?titulo=...`). | Não | Nenhuma |
| **GET** | `/{id}` | Busca os detalhes de um livro específico. | Não | Nenhuma |
| **POST** | `/` | Cadastra um novo livro no catálogo. | Sim | Nenhuma |
| **PUT** | `/{id}` | Substitui integralmente os dados de um livro. | Sim | Nenhuma |
| **PATCH** | `/{id}` | Atualiza campos isolados de um livro. | Sim | Nenhuma |
| **DELETE** | `/{id}` | Remove um livro do banco de dados. | Sim | Apenas **AGENTE** |

---

## 🚀 Como Executar o Projeto

### Pré-requisitos

* [.NET SDK](https://dotnet.microsoft.com/download) (Recomendado versão 6.0 ou superior).
* Uma IDE como Visual Studio, Visual Studio Code ou Rider.
* Ferramenta de testes de API (Postman, Insomnia) ou apenas o navegador para acessar o Swagger.

### Passos para rodar localmente

1. **Clone o repositório:**
```bash
git clone https://github.com/seu-usuario/sistema-livros.git
cd sistema-livros

```


2. **Restaure as dependências do pacote:**
```bash
dotnet restore

```


3. **Configure o Banco de Dados:**
* Certifique-se de que a *Connection String* no seu `appsettings.json` aponta para o seu banco de dados (SQL Server, SQLite, Postgres, etc).
* Rode as migrações do Entity Framework para criar as tabelas `Livros` e `Usuarios`:
```bash
dotnet ef database update

```




4. **Execute a aplicação:**
```bash
dotnet run

```


5. **Acesse o Swagger:**
* Abra o navegador e vá para `https://localhost:<porta>/swagger` para ver a interface interativa da documentação e testar os endpoints.



---

## 🔒 Segurança e Perfis de Acesso

O sistema utiliza a variável `Perfil` no modelo de `Usuario` para definir o nível de acesso. Por padrão, novos usuários recebem o perfil `CLIENTE`.

Para testar a rota de exclusão (`DELETE /api/livros/{id}`), você precisará:

1. Cadastrar um usuário comum.
2. Alterar manualmente no banco de dados o perfil desse usuário de `CLIENTE` para `AGENTE`.
3. Fazer login novamente para gerar um novo token JWT contendo as novas *Claims*.
4. Passar o Token no cabeçalho da requisição (`Authorization: Bearer <seu-token>`).
