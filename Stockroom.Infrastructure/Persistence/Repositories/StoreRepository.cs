using BuildingBlocks.Core.Seedwork.ValueObject;
using BuildingBlocks.Shared.Enums;
using Npgsql;
using Stockroom.Domain.Entities;
using Stockroom.Domain.Interfaces;
using Stockroom.Infrastructure.Persistence.Data;

namespace Stockroom.Infrastructure.Persistence.Repositories;

public class StoreRepository
    : RepositoryBase<StoreEntity, Guid>,
        IStoreRepository
{
    public StoreRepository(
        AppDbContext context) : base(context)
    {
    }
    
    protected override string TableName        => "\"Stores\"";
    protected override string PrimaryKeyColumn => "\"Id\"";

    protected override Guid GetPrimaryKey(StoreEntity entity)          => entity.Id;
    protected override void SetPrimaryKey(StoreEntity entity, Guid id) => entity.Id = id;

    protected override string GetInsertSql() 
        => """
        INSERT INTO "Stores" (
            "Id", "Name", "Type", "Description", "Phone", "Email",
            "IsActive", "IsDeleted",
            "CreatedAt", "UpdatedAt", "DeletedAt",
            "CreatedBy", "UpdatedBy",
            "Street", "City", "State", "PostalCode", "Country"
        ) VALUES (
            @id, @name, @type, @description, @phone, @email,
            @is_active, @is_deleted,
            @created_at, @updated_at, @deleted_at,
            @created_by, @updated_by,
            @street, @city, @state, @postal_code, @country
        )
        RETURNING "Id";
        """;

    protected override string GetUpdateSql() 
        => """
        UPDATE "Stores" SET
            "Name"        = @name,
            "Type"        = @type,
            "Description" = @description,
            "Phone"       = @phone,
            "Email"       = @email,
            "IsActive"    = @is_active,
            "UpdatedAt"    = @updated_at,
            "UpdatedBy"    = @updated_by,
            "Street"      = @street,
            "City"        = @city,
            "State"       = @state,
            "PostalCode"  = @postal_code,
            "Country"     = @country
        WHERE "Id" = @id;
        """;

    protected override IEnumerable<NpgsqlParameter> GetParameters(StoreEntity entity) =>
    [
        new NpgsqlParameter("id",          entity.Id),
        new NpgsqlParameter("name",        entity.Name),
        new NpgsqlParameter("type",        (int)entity.Type),
        new NpgsqlParameter("description", (object?)entity.Description ?? DBNull.Value),
        new NpgsqlParameter("phone",       (object?)entity.Phone       ?? DBNull.Value),
        new NpgsqlParameter("email",       (object?)entity.Email       ?? DBNull.Value),
        new NpgsqlParameter("is_active",   entity.IsActive),
        new NpgsqlParameter("is_deleted",  entity.IsDeleted),
        new NpgsqlParameter("created_at",  new DateTime(entity.CreatedAt, DateTimeKind.Utc)),
        new NpgsqlParameter("updated_at",  entity.UpdatedAt  is null ? DBNull.Value : new DateTime(entity.UpdatedAt.Value,  DateTimeKind.Utc)),
        new NpgsqlParameter("deleted_at",  entity.DeletedAt  is null ? DBNull.Value : new DateTime(entity.DeletedAt.Value,  DateTimeKind.Utc)),
        new NpgsqlParameter("created_by",  entity.CreatedBy),
        new NpgsqlParameter("updated_by",  entity.UpdatedBy),
        new NpgsqlParameter("street",      entity.Address.Street),
        new NpgsqlParameter("city",        entity.Address.City),
        new NpgsqlParameter("state",       entity.Address.State),
        new NpgsqlParameter("postal_code", entity.Address.PostalCode),
        new NpgsqlParameter("country",     entity.Address.Country),
    ];

    protected override StoreEntity Map(NpgsqlDataReader r) => new()
    {
        Id          = r.GetGuid(r.GetOrdinal("Id")),
        Name        = r.GetString(r.GetOrdinal("Name")),
        Type        = (EStoreType)r.GetInt32(r.GetOrdinal("Type")),
        Description = r.IsDBNull(r.GetOrdinal("Description")) ? null : r.GetString(r.GetOrdinal("Description")),
        Phone       = r.IsDBNull(r.GetOrdinal("Phone"))       ? null : r.GetString(r.GetOrdinal("Phone")),
        Email       = r.IsDBNull(r.GetOrdinal("Email"))       ? null : r.GetString(r.GetOrdinal("Email")),
        IsActive    = r.GetBoolean(r.GetOrdinal("IsActive")),
        IsDeleted   = r.GetBoolean(r.GetOrdinal("IsDeleted")),
        CreatedAt   = r.GetDateTime(r.GetOrdinal("CreatedAt")).Ticks,
        UpdatedAt   = r.IsDBNull(r.GetOrdinal("UpdatedAt"))  ? null : r.GetDateTime(r.GetOrdinal("UpdatedAt")).Ticks,
        DeletedAt   = r.IsDBNull(r.GetOrdinal("DeletedAt"))  ? null : r.GetDateTime(r.GetOrdinal("DeletedAt")).Ticks,
        CreatedBy   = r.GetGuid(r.GetOrdinal("CreatedBy")),
        UpdatedBy   = r.IsDBNull(r.GetOrdinal("UpdatedBy"))  ? null : r.GetGuid(r.GetOrdinal("UpdatedBy")),
        Address     = new Address(
            street:     r.GetString(r.GetOrdinal("Street")),
            city:       r.GetString(r.GetOrdinal("City")),
            state:      r.GetString(r.GetOrdinal("State")),
            postalCode: r.GetString(r.GetOrdinal("PostalCode")),
            country:    r.GetString(r.GetOrdinal("Country"))
        )
    };
}
