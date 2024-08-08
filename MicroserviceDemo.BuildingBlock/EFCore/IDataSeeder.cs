namespace MicroserviceDemo.BuildingBlock.EFCore;

public interface IDataSeeder
{
    Task SeedAllAsync();
}