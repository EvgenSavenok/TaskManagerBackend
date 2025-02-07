using Application.DataTransferObjects.TasksDto;

namespace Application.Contracts.UseCasesContracts.Tasks;

public interface ICreateTaskUseCase
{
    public Task ExecuteAsync(TaskForCreationDto taskDto);
}