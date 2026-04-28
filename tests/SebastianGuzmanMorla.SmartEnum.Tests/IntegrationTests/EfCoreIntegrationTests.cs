using Microsoft.EntityFrameworkCore;
using SebastianGuzmanMorla.SmartEnum.Attributes;
using SebastianGuzmanMorla.SmartEnum.Converters.EntityFrameworkCore;
using SebastianGuzmanMorla.SmartEnum.Tests.Types;

namespace SebastianGuzmanMorla.SmartEnum.Tests.IntegrationTests;

public class EfCoreIntegrationTests : IDisposable
{
    private class TestDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Use In-Memory database with explicit SmartEnum configuration
                optionsBuilder.UseInMemoryDatabase(databaseName: "test_db");
            }
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<TestStatus>()
                .HaveConversion<SmartEnumConverter<TestStatus, string>,
                    SmartEnumComparer<TestStatus, string>>();

            configurationBuilder.Properties<TestPermissionFlags>()
                .HaveConversion<SmartEnumFlagsValueConverter<TestPermissionFlags, TestPermission, string>,
                    SmartEnumFlagsValueComparer<TestPermissionFlags, TestPermission, string>>();
        }

        public DbSet<TestEntity> Entities { get; set; }
    }

    private readonly TestDbContext _context;

    public EfCoreIntegrationTests()
    {
        _context = new TestDbContext();
        // Create database automatically when first query is executed
        _context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task SmartEnum_SaveAndRetrieve_WorksCorrectly()
    {
        // Arrange
        var entity = new TestEntity
        {
            Name = "Test Entity",
            Status = TestStatus.Active
        };

        // Act
        _context.Entities.Add(entity);
        await _context.SaveChangesAsync();

        var retrieved = await _context.Entities.FindAsync(entity.Id);

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.Status.Should().Be(TestStatus.Active);
        retrieved.Status.Value.Should().Be("Active");
    }

    [Fact]
    public async Task SmartEnum_QueryByValue_WorksCorrectly()
    {
        // Arrange
        var entity1 = new TestEntity { Name = "Entity1", Status = TestStatus.Active };
        var entity2 = new TestEntity { Name = "Entity2", Status = TestStatus.Inactive };

        _context.Entities.AddRange(entity1, entity2);
        await _context.SaveChangesAsync();

        // Act
        var activeEntities = await _context.Entities
            .Where(e => e.Status == TestStatus.Active)
            .ToListAsync();

        // Assert
        activeEntities.Should().ContainSingle();
        activeEntities[0].Id.Should().Be(entity1.Id);
    }

    [Fact]
    public async Task SmartEnumFlags_SaveAndRetrieve_WorksCorrectly()
    {
        // Arrange
        var entity = new TestEntity
        {
            Name = "Test Entity",
            Permissions = new TestPermissionFlags(TestPermission.Read, TestPermission.Write)
        };

        // Act
        _context.Entities.Add(entity);
        await _context.SaveChangesAsync();

        var retrieved = await _context.Entities.FindAsync(entity.Id);

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.Permissions.Flags.Should().HaveCount(2);
        retrieved.Permissions.Flags.Should().Contain(TestPermission.Read);
        retrieved.Permissions.Flags.Should().Contain(TestPermission.Write);
    }

    [Fact]
    public async Task SmartEnumFlags_QueryByFlags_WorksCorrectly()
    {
        // Arrange
        var entity1 = new TestEntity
        {
            Name = "Entity1",
            Permissions = new TestPermissionFlags(TestPermission.Read)
        };
        var entity2 = new TestEntity
        {
            Name = "Entity2",
            Permissions = new TestPermissionFlags(TestPermission.Read, TestPermission.Write)
        };

        _context.Entities.AddRange(entity1, entity2);
        await _context.SaveChangesAsync();

        // Act - This is a simplified query; in real scenarios, you might need custom SQL
        var entitiesWithRead = await _context.Entities
            .ToListAsync(); // Load all and filter in memory

        var filtered = entitiesWithRead.Where(e => e.Permissions.Has(TestPermission.Read)).ToList();

        // Assert
        filtered.Should().HaveCount(2);
        filtered.Select(e => e.Id).Should().Contain(new[] { entity1.Id, entity2.Id });
    }

    [Fact]
    public async Task ChangeTracking_WorksWithSmartEnum()
    {
        // Arrange
        var entity = new TestEntity { Name = "Test", Status = TestStatus.Active };
        _context.Entities.Add(entity);
        await _context.SaveChangesAsync();

        // Act
        entity.Status = TestStatus.Inactive;
        var changes = _context.ChangeTracker.Entries<TestEntity>()
            .Where(e => e.State == EntityState.Modified)
            .ToList();

        // Assert
        changes.Should().ContainSingle();
        await _context.SaveChangesAsync();

        var updated = await _context.Entities.FindAsync(entity.Id);
        updated!.Status.Should().Be(TestStatus.Inactive);
    }

    [Fact]
    public async Task ChangeTracking_WorksWithSmartEnumFlags()
    {
        // Arrange
        var entity = new TestEntity
        {
            Name = "Test",
            Permissions = new TestPermissionFlags(TestPermission.Read)
        };
        _context.Entities.Add(entity);
        await _context.SaveChangesAsync();

        // Act
        entity.Permissions = new TestPermissionFlags(TestPermission.Read, TestPermission.Write);
        var changes = _context.ChangeTracker.Entries<TestEntity>()
            .Where(e => e.State == EntityState.Modified)
            .ToList();

        // Assert
        changes.Should().ContainSingle();
        await _context.SaveChangesAsync();

        var updated = await _context.Entities.FindAsync(entity.Id);
        updated!.Permissions.Flags.Should().HaveCount(2);
    }
}