namespace ProperNutritionDiary.BuildingBlocks.DomainExtensions;

public static class LinqExtensions
{
    public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunkSize)
    {
        int i = 0;
        return source.GroupBy(x => (int)Math.Floor((i++) / (double)chunkSize));
    }
}
