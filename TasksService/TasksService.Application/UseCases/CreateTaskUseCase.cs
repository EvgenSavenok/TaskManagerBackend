using Application.Contracts.RepositoryContracts;
using Application.Contracts.UseCasesContracts;
using Application.DataTransferObjects.TasksDto;
using AutoMapper;
using TasksService.Domain.Models;

namespace Application.UseCases;

public class CreateTaskUseCase : ICreateTaskUseCase
{
    private readonly IRepositoryManager _repository;
    private readonly IMapper _mapper;

    public CreateTaskUseCase(IRepositoryManager repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task ExecuteAsync(TaskForCreationDto taskDto)
    {
        var taskEntity = _mapper.Map<CustomTask>(taskDto);
        _repository.Task.Create(taskEntity);
        await _repository.SaveAsync();
    }
}