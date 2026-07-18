using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Sportiva.Hubs;

[Authorize]
public class ChatHub : Hub
{
}
