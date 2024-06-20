namespace ProperNutritionDiary.DiaryApi.Diary.Get;

public record DiaryResponse(
    Guid Id,
    Guid UserId,
    DateTime Date,
    List<DiaryEntryResponse> DiaryEntries
);
