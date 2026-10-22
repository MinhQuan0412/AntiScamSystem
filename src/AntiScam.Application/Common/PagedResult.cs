namespace AntiScam.Application.Common;

/// <summary>
/// Represents a paginated result set wrapping a collection of items.
/// </summary>
/// <typeparam name="T">The type of items in the result.</typeparam>
public sealed class PagedResult<T>
{
    /// <summary>Gets the items on the current page.</summary>
    public IEnumerable<T> Items { get; init; } = Enumerable.Empty<T>();

    /// <summary>Gets the total number of items across all pages.</summary>
    public int TotalCount { get; init; }

    /// <summary>Gets the current 1-based page number.</summary>
    public int Page { get; init; }

    /// <summary>Gets the number of items per page.</summary>
    public int PageSize { get; init; }

    /// <summary>Gets the total number of pages.</summary>
    public int TotalPages { get; init; }

    /// <summary>Gets a value indicating whether a previous page exists.</summary>
    public bool HasPreviousPage { get; init; }

    /// <summary>Gets a value indicating whether a next page exists.</summary>
    public bool HasNextPage { get; init; }

    /// <summary>
    /// Creates a new <see cref="PagedResult{T}"/> from the given items and pagination metadata.
    /// </summary>
    /// <param name="items">The items for the current page.</param>
    /// <param name="totalCount">Total number of items across all pages.</param>
    /// <param name="page">Current 1-based page number.</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <returns>A fully constructed <see cref="PagedResult{T}"/>.</returns>
    public static PagedResult<T> Create(IEnumerable<T> items, int totalCount, int page, int pageSize)
    {
        if (page < 1) throw new ArgumentOutOfRangeException(nameof(page), "Page must be >= 1.");
        if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize), "PageSize must be >= 1.");

        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedResult<T>
        {
            Items          = items,
            TotalCount     = totalCount,
            Page           = page,
            PageSize       = pageSize,
            TotalPages     = totalPages,
            HasPreviousPage = page > 1,
            HasNextPage    = page < totalPages
        };
    }

    /// <summary>Returns an empty paged result with the supplied pagination parameters.</summary>
    public static PagedResult<T> Empty(int page = 1, int pageSize = 20) =>
        Create(Enumerable.Empty<T>(), 0, page, pageSize);
}
