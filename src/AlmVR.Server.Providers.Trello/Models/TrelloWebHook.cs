using System;
using System.Collections.Generic;
using System.Text;

namespace AlmVR.Server.Providers.Trello.Models
{
    internal class TrelloWebHook : IEquatable<TrelloWebHook>
    {
        public string ID { get; set; }
        public string Description { get; set; }
        public string IdModel { get; set; }
        public string CallbackURL { get; set; }
        public bool Active { get; set; } = true;

        public bool Equals(TrelloWebHook other) =>
            ID == other.ID &&
            Description == other.Description &&
            IdModel == other.IdModel &&
            CallbackURL == other.CallbackURL &&
            Active == other.Active;

        public override bool Equals(object obj)
        {
            if (!(obj is TrelloWebHook))
                return false;

            return Equals(obj as TrelloWebHook);
        }

        public override int GetHashCode()
        {
            var hashCode = -785764;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ID);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Description);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(IdModel);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CallbackURL);
            hashCode = hashCode * -1521134295 + Active.GetHashCode();
            return hashCode;
        }

        public bool Match(TrelloWebHook other) =>
            IdModel == other.IdModel &&
            CallbackURL == other.CallbackURL;
    }
}
