namespace AntiScam.Domain.Entities;

/// <summary>
/// Abstract base class for all domain entities.
/// Provides common audit properties: identity, timestamps, and soft-delete flag.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Globally unique identifier for the entity.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// UTC timestamp indicating when the entity was first persisted.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// UTC timestamp indicating the last time the entity was modified.
    /// Updated automatically by the persistence layer on every save.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Soft-delete flag. When <see langword="true"/> the entity is logically
    /// deleted and should be excluded from standard queries.
    /// </summary>
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// Optional identifier of the user who created this entity.
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Optional identifier of the user who last modified this entity.
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Marks the entity as soft-deleted and records who performed the action.
    /// </summary>
    /// <param name="deletedBy">Identifier of the actor performing the deletion.</param>
    public void SoftDelete(string? deletedBy = null)
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = deletedBy;
    }

    /// <summary>
    /// Updates the <see cref="UpdatedAt"/> timestamp and the <see cref="UpdatedBy"/> field
    /// to reflect the current modification.
    /// </summary>
    /// <param name="updatedBy">Identifier of the actor performing the update.</param>
    public void Touch(string? updatedBy = null)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }
}
