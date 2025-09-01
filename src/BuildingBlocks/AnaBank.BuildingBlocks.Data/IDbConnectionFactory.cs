using System.Data;

namespace AnaBank.BuildingBlocks.Data;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}