namespace Application.Contracts.MessagingContracts;

public interface ITaskDeletedProducer
{
    void PublishTaskDeletedEvent(Guid taskId);
}