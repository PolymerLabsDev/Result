namespace PolymerLabs.Result;

/// <summary>
///   Utility that provides the capability to return a successful result with a value, or in the case of failure, a failure
///   reason.
/// </summary>
/// <typeparam name="TValue">The type of the value.</typeparam>
/// <typeparam name="TError">The type of the Error result.</typeparam>
public readonly struct ValueResult<TValue, TError>
{
  private readonly TValue _value;
  private readonly TError _error;
  private readonly bool _ok;

  private ValueResult(TValue value)
  {
    _value = value;
    _error = default!;
    _ok = true;
  }

  private ValueResult(TError error)
  {
    _value = default!;
    _error = error;
    _ok = false;
  }

  /// <summary>
  ///   Invokes one of two evaluators depending on whether this <see cref="ValueResult{TValue,TError}" /> is Ok or Error.
  /// </summary>
  /// <param name="ok">Function to invoke if the result is Ok; if called, it's result is returned.</param>
  /// <param name="error">Function to invoke if the result is Error; if called, it's result is returned.</param>
  /// <returns>The <see cref="TValue" /> returned by the appropriate evaluator.</returns>
  public TValue When(Func<TValue, TValue> ok, Func<TError, TValue> error)
  {
    return _ok ? ok(_value) : error(_error);
  }

  /// <summary>
  ///   Invokes one of two evaluators depending on whether this <see cref="ValueResult{TValue,TError}" /> is Ok or Error.
  /// </summary>
  /// <param name="ok">Function to invoke if the result is Ok.</param>
  /// <param name="error">Function to invoke if the result is Error; if called, it's result is returned.</param>
  /// <returns>The <see cref="TValue" /> of the result, or in the case of Error, the result from the Error function.</returns>
  public TValue When(Action<TValue> ok, Func<TError, TValue> error)
  {
    if (!_ok)
    {
      return error(_error);
    }

    ok(_value);
    return _value;
  }

  /// <summary>
  ///   Invokes one of two evaluators depending on whether this <see cref="ValueResult{TValue,TError}" /> is Ok or Error. The
  ///   evaluators return <see cref="TResult" />, rather than <see cref="TValue" />.
  /// </summary>
  /// <param name="ok">Function to invoke if the result is Ok; if called, it's result is returned.</param>
  /// <param name="error">Function to invoke if the result is Error; if called, it's result is returned.</param>
  /// <returns>The <see cref="TResult" /> returned by the appropriate evaluator.</returns>
  public TResult When<TResult>(Func<TValue, TResult> ok, Func<TError, TResult> error)
  {
    return _ok ? ok(_value) : error(_error);
  }

  /// <summary>
  ///   Invokes a function if this <see cref="ValueResult" /> is Error.
  /// </summary>
  /// <param name="error">Function to invoke if the result is Error; if called, it's result is returned.</param>
  /// <returns>The <see cref="TValue" /> of the result, or in the case of Error, the result from the Error function.</returns>
  public TValue WhenError(Func<TError, TValue> error)
  {
    return _ok ? _value : error(_error);
  }

  /// <summary>
  ///   Create an Ok result and provide a value.
  /// </summary>
  /// <param name="value">The result value.</param>
  /// <typeparam name="TValue">The type of value.</typeparam>
  /// <typeparam name="TError">The type of the error.</typeparam>
  /// <returns>An Ok result.</returns>
  public static ValueResult<TValue, TError> Ok(TValue value)
  {
    return new ValueResult<TValue, TError>(value);
  }

  /// <summary>
  ///   Create an Error result and provide a value.
  /// </summary>
  /// <param name="error">The error.</param>
  /// <typeparam name="TValue">The type of value.</typeparam>
  /// <typeparam name="TError">The type of the error.</typeparam>
  /// <returns>An Ok result.</returns>
  public static ValueResult<TValue, TError> Error(TError error)
  {
    return new ValueResult<TValue, TError>(error);
  }

  // Convert a value of type TOk to an Ok result.
  public static implicit operator ValueResult<TValue, TError>(TValue value)
  {
    return Ok(value);
  }

  // Convert a value of type TError to an Error result.
  public static implicit operator ValueResult<TValue, TError>(TError reason)
  {
    return Error(reason);
  }
}

/// <summary>
///   Utility that provides the capability to return a successful result with a value, or in the case of failure, a failure
///   reason.
/// </summary>
public static class ValueResult
{
  /// <summary>
  ///   Get an Ok result with a value.
  /// </summary>
  /// <param name="value">The result.</param>
  /// <typeparam name="TValue">The type of the value.</typeparam>
  /// <typeparam name="TError">The type of the Error result.</typeparam>
  /// <returns>An Ok result.</returns>
  public static ValueResult<TValue, TError> Ok<TValue, TError>(TValue value)
    where TValue : struct where TError : struct, Enum
  {
    return ValueResult<TValue, TError>.Ok(value);
  }

  /// <summary>
  ///   Get an Error result with an error.
  /// </summary>
  /// <param name="error">The error.</param>
  /// <typeparam name="TOk">The type of the value.</typeparam>
  /// <typeparam name="TError">The type of the Error result.</typeparam>
  /// <returns>An Ok result.</returns>
  public static ValueResult<TOk, TError> Error<TOk, TError>(TError error)
    where TOk : struct where TError : struct, Enum
  {
    return ValueResult<TOk, TError>.Error(error);
  }
}