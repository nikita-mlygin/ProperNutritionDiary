using DiaryMessageStore;
using ProperNutritionDiary.DiaryContracts.Update;

namespace DiaryMessageStore;

public class MessageStore
{
    private readonly List<DiaryItemUpdatedEvent> _messages = [];

    public void AddMessage(DiaryItemUpdatedEvent message)
    {
        _messages.Add(message);
    }

    public IReadOnlyList<DiaryItemUpdatedEvent> GetAllMessages() => _messages.AsReadOnly();
}
