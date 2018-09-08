using AlmVR.Common.Models;
using System.Threading.Tasks;

namespace AlmVR.Server.Core.Providers
{
    public interface ICardProvider
    {
        Task<CardModel> GetCardAsync(string id);
        Task MoveCardAsync(CardModel card, SwimLaneModel targetSwimLane);
    }
}
