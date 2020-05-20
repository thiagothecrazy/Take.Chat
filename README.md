# Take.Chat

O Take.Chat é uma sala de bate-papo simples baseada em WebSockets, desenvolvido em .NET Core C# no Visual Studio 2019.

## Arquitetura

Arquitetura baeada Onion Architecture com testes unitário do projeto 'Core', que possui toda a regra de negócio.
 
## Licença

Este projeto não possui objetivos comerciais, contruído para exemplificar a utilização da arquitetura e tecnologia.

## Instruções de Uso

O projeto esta configurado para iniciar automaticamente na rota "http://localhost:51275/", que é a página inicial.

Nesta página deve-se informar o nome da sala de bata-papo e o apelido do usuário para entrar em um sala de bata-papo.
 - Ao informar um nome de sala não existente, uma sala será criada automaticamente com o nome infromado.
 - O apelido do usuário deve ser único, independente da sala que estiver.

Ao entrar na sala de bate-papo, aparecerá instruções de uso com o comandos configurados:
 - **/ajuda** : Para receber as instruções de uso novamente
 - **/sair** : Para sair da sala bata-papo
 - **@fulano** Minha mensagem : Para enviar uma mensagem direcionada para 'fulano'
 - **/p @fulano**  Minha mensagem : Para enviara uma mensagem privada para 'fulano', outras pessoas na sala não podem ver esta mensagem.
 
 
 
 