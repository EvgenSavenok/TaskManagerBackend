using Application.Contracts.Grpc;
using Application.Contracts.MessagingContracts;
using Application.Contracts.Redis;
using Application.Contracts.RepositoryContracts;
using Application.UseCases.Commands.TaskCommands.CreateTask;
using AutoMapper;
using FluentValidation;
using Moq;
using TasksService.Domain.Models;

namespace Tests.TasksService;

public class CreateTaskCommandHandlerTests
{
    private readonly Mock<IRepositoryManager> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<CustomTask>> _validatorMock;
    private readonly CreateTaskCommandHandler _handlerTests;
    private readonly Mock<ITaskCreatedProducer> _taskCreatedProducerMock;
    private readonly Mock<IUserGrpcService> _userGrpcMock;
    
    public CreateTaskCommandHandlerTests()
    {
        _repositoryMock = new Mock<IRepositoryManager>();
        _mapperMock = new Mock<IMapper>();
        _validatorMock = new Mock<IValidator<CustomTask>>();
        _taskCreatedProducerMock = new Mock<ITaskCreatedProducer>();
        _userGrpcMock = new Mock<IUserGrpcService>();

        _handlerTests = new CreateTaskCommandHandler(
            _repositoryMock.Object,
            _mapperMock.Object,
            _validatorMock.Object,
            _taskCreatedProducerMock.Object,
            _userGrpcMock.Object
        );
    }
}