using Application.DataTransferObjects.TasksDto;

namespace Application.Contracts.MessagingContracts;

public interface ITaskCreatedProducer
{
    void PublishTaskCreatedEvent(CreateTaskEventDto createTaskEventDto);
}