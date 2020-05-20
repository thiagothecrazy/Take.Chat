using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Take.Chat.Core.Entities;
using Take.Chat.Core.Interfaces;

namespace Take.Chat.Infrastructure.Data
{
    public class BatePapoRepositorio : IBatePapoRepositorio
    {
        private readonly Guid _batePapoID = Guid.NewGuid();
        private readonly ConcurrentDictionary<string, Sala> _repositorio = new ConcurrentDictionary<string, Sala>();

        private readonly WebSocketRepositorio _webSocketRepositorio;

        public BatePapoRepositorio(WebSocketRepositorio webSocketRepositorio)
        {
            _webSocketRepositorio = webSocketRepositorio;
        }

        public BatePapo ObterBatePapo()
        {
            return new BatePapo { ID = _batePapoID,  Salas = ListarTodasSalas() } ;
        }

        public Sala ObterSalaPeloNome(string nome)
        {
            return _repositorio.FirstOrDefault(d => d.Value.Nome == nome).Value;
        }

        public IEnumerable<Sala> ListarTodasSalas()
        {
            return _repositorio.Select(d => d.Value);
        }

        public Sala ObterSalaPeloUsuario(Usuario usuario)
        {
            return _repositorio.Where(d => d.Value.Usuarios.Any(d => d.ID == usuario.ID)).FirstOrDefault().Value;
        }

        public void AdicionarSala(Sala sala)
        {
            _repositorio.TryAdd(sala.ID.ToString(), sala);
        }

        public Usuario ObterUsuarioPeloApelido(string apelido)
        {
            return _repositorio.SelectMany(d => d.Value.Usuarios).FirstOrDefault(d => d.Apelido == apelido);
        }

        public Usuario ObterUsuarioPeloID(string id)
        {
            return _repositorio.SelectMany(d => d.Value.Usuarios).FirstOrDefault(d => d.ID.ToString() == id);
        }

        public void AdicionarUsuarioSala(Usuario usuario, Sala sala)
        {
            _repositorio.First(d => d.Value.ID == sala.ID).Value.Usuarios.Add(usuario);
            _webSocketRepositorio.AdicionarOuAlterarSocket(usuario.ID.ToString(), null);
        }

        public void RemoverUsuarioSala(Usuario usuario)
        {
            var usuarioID = usuario.ID.ToString();
            var salaID = ObterSalaPeloUsuario(usuario).ID;

            _repositorio.First(d => d.Value.ID == salaID).Value.Usuarios.Remove(usuario);
            _webSocketRepositorio.RemoverSocketAsync(usuarioID);
        }

        public async void EnviarMensagemAsync(Usuario usuario, string mensagem)
        {
            await _webSocketRepositorio.EnviarMensagemAsync(usuario.ID.ToString(), mensagem);
        }

    }
}
