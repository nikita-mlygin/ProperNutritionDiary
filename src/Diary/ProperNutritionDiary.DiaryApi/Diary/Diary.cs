namespace ProperNutritionDiary.DiaryApi.Diary;

public class Diary
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime Date { get; set; }
    public List<DiaryEntry> DiaryEntries { get; set; } = [];
}
