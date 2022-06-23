using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Puzzle.Compound.Common.Hubs
{
    public class CounterHub : Hub
    {
        public async Task UpdateAddedExcelOwnersCount(int count)
        {
            await Clients.Client(Context.ConnectionId).SendAsync("UpdateAddedExcelOwnersCount", count);
        }
        public async Task ForceLogoutUser(string userId)
        {
            await Clients.All.SendAsync("ForceLogoutUser", userId);
        }
        public async Task UpdatePendingListCount(bool isUpdated)
        {
            await Clients.All.SendAsync("UpdatePendingListCount", isUpdated);
        }

        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }
    }
}
