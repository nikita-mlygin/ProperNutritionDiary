namespace ProperNutritionDiary.DiaryApi;

public static class TaskExtensions
{
    public static Task<Either<Error, T>> ToEitherAsync<T>(this Task<Option<T>> task, Error error)
    {
        return task.Map(option =>
            option.Match(
                Some: Either<Error, T>.Right,
                None: () => error // Adjust the error message or value as needed
            )
        );
    }
}
