using System.Data;
using Dapper;

namespace ProperNutritionDiary.BuildingBlocks.PersistencePackages.Dapper;

public class GuidTypeHandler : SqlMapper.TypeHandler<Guid>
{
    public override void SetValue(IDbDataParameter parameter, Guid guid)
    {
        parameter.Value = guid.ToByteArray();
    }

    public override Guid Parse(object value)
    {
        return new Guid((byte[])value);
    }
}
