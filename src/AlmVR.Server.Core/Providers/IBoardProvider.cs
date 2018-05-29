using AlmVR.Common.Models;
using System.Threading.Tasks;

namespace AlmVR.Server.Core.Providers
{
    public interface IBoardProvider
    {
        Task<BoardModel> GetBoardAsync();
    }
}
