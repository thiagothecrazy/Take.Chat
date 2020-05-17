using System.Collections.Generic;
using System.Linq;

namespace Take.Chat.Core
{
    public class Resultado<T> where T : class
    {
        public IEnumerable<string> Notificacoes { get; set; } = new List<string>();

        public bool Sucesso { get { return !Notificacoes.Any(); } }

        public T Objeto { get; }

        private Resultado(T obj)
        {
            Objeto = obj;
        }

        private Resultado(IEnumerable<string> notificacaoes)
        {
            Objeto = null;
            Notificacoes = notificacaoes;
        }

        public static Resultado<T> Ok(T obj)
        {
            return new Resultado<T>(obj);
        }

        public static Resultado<T> Erro(IEnumerable<string> notificacaoes)
        {
            return new Resultado<T>(notificacaoes);
        }
    }
}
