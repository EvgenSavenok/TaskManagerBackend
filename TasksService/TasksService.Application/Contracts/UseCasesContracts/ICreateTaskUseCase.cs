using Application.DataTransferObjects;
using Application.DataTransferObjects.TasksDto;

namespace Application.Contracts.UseCasesContracts;

public interface ICreateTaskUseCase
{
    public Task ExecuteAsync(TaskForCreationDto taskDto);
}