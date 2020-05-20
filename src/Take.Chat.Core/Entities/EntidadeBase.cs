using System;

namespace Take.Chat.Core.Entities
{
    public abstract class EntidadeBase
    {
        public Guid ID = Guid.NewGuid();
    }
}
