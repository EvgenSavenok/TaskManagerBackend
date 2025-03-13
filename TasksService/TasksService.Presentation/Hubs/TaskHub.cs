using Application.DataTransferObjects.TasksDto;
using Microsoft.AspNetCore.SignalR;

namespace TasksService.Presentation.Hubs;

public class TaskHub : Hub
{
    public async Task SendTaskUpdate(TaskDto taskDto)
    {
        await Clients.All.SendAsync("ReceiveTaskUpdate", taskDto);
    }
}