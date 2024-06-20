using MassTransit;
using ProperNutritionDiary.DiaryContracts.Update;

namespace DiaryMessageStore;

public class DiaryItemAddedEventConsumer(MessageStore messageStore)
    : IConsumer<DiaryItemUpdatedEvent>
{
    private readonly MessageStore _messageStore = messageStore;

    public Task Consume(ConsumeContext<DiaryItemUpdatedEvent> context)
    {
        _messageStore.AddMessage(context.Message);
        return Task.CompletedTask;
    }
}
