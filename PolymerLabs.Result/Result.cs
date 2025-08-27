namespace PolymerLabs.Result;

/// <summary>
///   Utility that provides the capability to return a successful result, or in the case of failure, a failure reason.
/// </summary>
/// <typeparam name="TError">The type of the Error result.</typeparam>
public readonly struct Result<TError>
{
  private readonly TError _error;
  private readonly bool _ok;

  public Result()
  {
    _error = default!;
    _ok = true;
  }

  private Result(TError error)
  {
    _error = error;
    _ok = false;
  }

  /// <summary>
  ///   Invokes one of two functions depending on whether this <see cref="Result" /> is Ok or Error.
  /// </summary>
  /// <param name="ok">Function to invoke if the result is Ok.</param>
  /// <param name="error">Function to invoke if the result is Error.</param>
  public void When(Action ok, Action<TError> error)
  {
    if (_ok)
    {
      ok();
    }
    else
    {
      error(_error);
    }
  }

  /// <summary>
  ///   Invokes a function when this <see cref="Result" /> is Error. If calling a function for both Ok and Error, prefer
  ///   <see cref="When" />.
  /// </summary>
  /// <param name="error">Function to invoke if the result is Error.</param>
  public void WhenError(Action<TError> error)
  {
    if (!_ok)
    {
      error(_error);
    }
  }

  /// <summary>
  ///   Create an Ok result.
  /// </summary>
  /// <typeparam name="TError">The type of the error.</typeparam>
  /// <returns>An Ok result.</returns>
  public static Result<TError> Ok()
  {
    return new Result<TError>();
  }

  /// <summary>
  ///   Create an Ok result.
  /// </summary>
  /// <param name="error">The error.</param>
  /// <typeparam name="TError">The type of the error.</typeparam>
  /// <returns>An Ok result.</returns>
  public static Result<TError> Error(TError error)
  {
    return new Result<TError>(error);
  }

  // Convert the Ok enum to an Ok result.
  public static implicit operator Result<TError>(OkResult _)
  {
    return Ok();
  }

  // Convert a value of type TError to an Error result.
  public static implicit operator Result<TError>(TError reason)
  {
    return Error(reason);
  }
}

/// <summary>
///   Utility that provides the capability to return a successful result, or in the case of failure, a failure reason.
/// </summary>
public static class Result
{
  /// <summary>
  ///   Get an Ok result, does not contain Error information. Should be cast to <see cref="Result" />.
  /// </summary>
  /// <returns>An <see cref="OkResult" />.</returns>
  public static OkResult Ok => OkResult.Ok;

  /// <summary>
  ///   Get an Error result with an error.
  /// </summary>
  /// <param name="error">The error.</param>
  /// <typeparam name="TError">The type of the Error result.</typeparam>
  /// <returns>An Ok result.</returns>
  public static Result<TError> Error<TError>(TError error)
  {
    return Result<TError>.Error(error);
  }
}

/// <summary>
///   Representative of an Ok result, regardless of the error type.
/// </summary>
public readonly struct OkResult
{
  /// <summary>
  ///   A read-only instance of an Ok result.
  /// </summary>
  public static OkResult Ok { get; } = new();
}