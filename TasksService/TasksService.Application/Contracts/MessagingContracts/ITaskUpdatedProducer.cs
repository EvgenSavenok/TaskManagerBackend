using Application.DataTransferObjects.TasksDto;

namespace Application.Contracts.MessagingContracts;

public interface ITaskUpdatedProducer
{
    void PublishTaskUpdatedEvent(UpdateTaskEventDto createTaskEventDto);
}