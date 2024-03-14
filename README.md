# KONSI.CHALLENGE
Este projeto consiste em uma API que busca e retorna o número da matrícula de servidores em uma API externa da Konsi, empresa que coleta dados para propor melhores opções de créditos aos clientes. A aplicação inclui a geração de token de autenticação, consulta do CPF do cliente e retorno dos dados de benefícios encontrados, como número do benefício e código do tipo do benefício.

## Tecnologias Utilizadas

  - C#
  - RabbitMQ
  - Redis
  - Elasticsearch

## Pré-requisitos Locais

### Para rodar o projeto localmente, é necessário ter instalado:

  - .NET Core SDK
  - RabbitMQ
  - Redis
  - Elasticsearch

## Como Rodar o Projeto Localmente 

1. Clone este repositório: git clone <link_do_repositório>
2. Acesse o diretório do projeto: cd <nome_do_diretório>
3. Execute o comando para restaurar as dependências: dotnet restore
4. Configure as credenciais da API externa e demais informações sensíveis conforme necessário.
5. Inicie os serviços do RabbitMQ, Redis e Elasticsearch.
6. Execute o projeto: dotnet run
7. Acesse a interface web através do navegador e utilize o campo de busca para consultar os dados de matrícula.
