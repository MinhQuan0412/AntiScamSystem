namespace AntiScam.Application.Common;

/// <summary>
/// Represents the outcome of an operation that returns a value of type <typeparamref name="T"/>.
/// Follows the Result / Either pattern to avoid exception-driven flow control.
/// </summary>
/// <typeparam name="T">The type of the success value.</typeparam>
public sealed class Result<T>
{
    /// <summary>Gets a value indicating whether the operation succeeded.</summary>
    public bool IsSuccess { get; }

    /// <summary>Gets a value indicating whether the operation failed.</summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>Gets the success value; <c>null</c> when <see cref="IsFailure"/> is true.</summary>
    public T? Value { get; }

    /// <summary>Gets the first error message, or <c>null</c> on success.</summary>
    public string? Error => Errors.Count > 0 ? Errors[0] : null;

    /// <summary>Gets all error messages accumulated during the operation.</summary>
    public List<string> Errors { get; }

    private Result(bool isSuccess, T? value, List<string> errors)
    {
        IsSuccess = isSuccess;
        Value     = value;
        Errors    = errors;
    }

    /// <summary>Creates a successful result containing the given <paramref name="value"/>.</summary>
    public static Result<T> Success(T value) =>
        new(true, value, new List<string>());

    /// <summary>Creates a failed result with a single error message.</summary>
    public static Result<T> Failure(string error) =>
        new(false, default, new List<string> { error });

    /// <summary>Creates a failed result with multiple error messages.</summary>
    public static Result<T> Failure(List<string> errors) =>
        new(false, default, errors ?? throw new ArgumentNullException(nameof(errors)));

    /// <summary>Creates a failed result from another result, preserving its errors.</summary>
    public static Result<T> Failure(Result result) =>
        new(false, default, result.Errors);

    /// <summary>
    /// Implicitly converts a value to a successful <see cref="Result{T}"/>.
    /// </summary>
    public static implicit operator Result<T>(T value) => Success(value);

    /// <summary>
    /// Projects the success value using <paramref name="selector"/> when the result is successful,
    /// or propagates the errors otherwise.
    /// </summary>
    public Result<TOut> Map<TOut>(Func<T, TOut> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);
        return IsSuccess
            ? Result<TOut>.Success(selector(Value!))
            : Result<TOut>.Failure(Errors);
    }

    /// <inheritdoc/>
    public override string ToString() =>
        IsSuccess ? $"Success({Value})" : $"Failure([{string.Join(", ", Errors)}])";
}

/// <summary>
/// Represents the outcome of a void operation (no return value).
/// </summary>
public sealed class Result
{
    /// <summary>Gets a value indicating whether the operation succeeded.</summary>
    public bool IsSuccess { get; }

    /// <summary>Gets a value indicating whether the operation failed.</summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>Gets the first error message, or <c>null</c> on success.</summary>
    public string? Error => Errors.Count > 0 ? Errors[0] : null;

    /// <summary>Gets all error messages accumulated during the operation.</summary>
    public List<string> Errors { get; }

    private Result(bool isSuccess, List<string> errors)
    {
        IsSuccess = isSuccess;
        Errors    = errors;
    }

    /// <summary>Singleton successful result.</summary>
    public static Result Ok() => new(true, new List<string>());

    /// <summary>Alias for Ok() to represent a successful void Result.</summary>
    public static Result Success() => new(true, new List<string>());

    /// <summary>Creates a failed result with a single error message.</summary>
    public static Result Failure(string error) =>
        new(false, new List<string> { error });

    /// <summary>Creates a failed result with multiple error messages.</summary>
    public static Result Failure(List<string> errors) =>
        new(false, errors ?? throw new ArgumentNullException(nameof(errors)));

    /// <summary>Converts this result to a typed result, propagating errors.</summary>
    public Result<T> ToTyped<T>() => IsSuccess
        ? throw new InvalidOperationException("Cannot convert a successful void Result to a typed Result without a value.")
        : Result<T>.Failure(Errors);

    /// <inheritdoc/>
    public override string ToString() =>
        IsSuccess ? "Success" : $"Failure([{string.Join(", ", Errors)}])";
}
