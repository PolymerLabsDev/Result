using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace PolymerLabs.Result;

/// <summary>
///   The result of an operation.
/// </summary>
public readonly struct Result
{
  private readonly Outcome _outcome;

  private Result(Outcome outcome)
  {
    _outcome = outcome;
  }

  /// <summary>
  ///   Handler in the case of an <see cref="Outcome.Error" /> outcome.
  /// </summary>
  /// <param name="handler">Action to perform on an Error result.</param>
  [DebuggerStepThrough]
  public OkStage OnError(Action handler)
  {
    if (_outcome == Outcome.Error) handler();

    return new OkStage(this);
  }

  /// <summary>
  ///   Async handler in the case of an <see cref="Outcome.Error" /> outcome.
  /// </summary>
  /// <param name="handler">Action to perform on an Error result.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  [DebuggerStepThrough]
  public async Task<OkStage> OnErrorAsync(Func<Task> handler,
    CancellationToken cancellationToken = default)
  {
    if (_outcome != Outcome.Error) return new OkStage(this);

    cancellationToken.ThrowIfCancellationRequested();
    await handler().ConfigureAwait(false);

    return new OkStage(this);
  }

  /// <summary>
  ///   Represents an Ok <see cref="Result" /> or <see cref="Result{T}" />.
  /// </summary>
  /// <returns><see cref="OkResult" /> that is implicitly castable to a <see cref="Result" /> or <see cref="Result{T}" />.</returns>
  public static OkResult Ok { get; } = new();

  /// <summary>
  ///   Represents an Error <see cref="Result" />.
  /// </summary>
  /// <returns><see cref="ErrorResult" /> that is implicitly castable to a <see cref="Result" />.</returns>
  public static ErrorResult Error { get; } = new();

  // Convert Ok to an Ok result.
  public static implicit operator Result(OkResult _)
  {
    return new Result(Outcome.Ok);
  }

  // Convert Error to an Error result.
  public static implicit operator Result(ErrorResult _)
  {
    return new Result(Outcome.Error);
  }

  /// <summary>
  ///   Stage for handling the Ok outcome of a <see cref="Result" />.
  /// </summary>
  public readonly struct OkStage(Result result)
  {
    /// <summary>
    ///   Handler for the case of an <see cref="Outcome.Ok" /> outcome.
    /// </summary>
    /// <param name="handler">Action to perform on an Ok result.</param>
    [DebuggerStepThrough]
    public void OnOk(Action handler)
    {
      if (result._outcome != Outcome.Ok) return;

      handler();
    }

    /// <summary>
    ///   Async handler for the case of an <see cref="Outcome.Ok" /> outcome.
    /// </summary>
    /// <param name="handler">Action to perform on an Ok result.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    [DebuggerStepThrough]
    public Task OnOkAsync(Func<Task> handler, CancellationToken cancellationToken = default)
    {
      if (result._outcome != Outcome.Ok) return Task.CompletedTask;

      cancellationToken.ThrowIfCancellationRequested();
      return handler();
    }
  }
}

/// <summary>
///   The result of an operation. If the operation failed, a failure reason is given.
/// </summary>
/// <typeparam name="TReason">The type of the error reason.</typeparam>
public readonly struct Result<TReason>
{
  private readonly TReason _reason;
  private readonly Outcome _outcome;

  private Result(TReason reason)
  {
    _reason = reason;
    _outcome = Outcome.Error;
  }

  /// <summary>
  ///   Handler in the case of an <see cref="Outcome.Error" /> outcome.
  /// </summary>
  /// <param name="handler">Action to perform on an Error result. The failure reason is provided through a parameter.</param>
  [DebuggerStepThrough]
  public OkStage OnError(Action<TReason> handler)
  {
    if (_outcome == Outcome.Error) handler(_reason);

    return new OkStage(this);
  }

  /// <summary>
  ///   Async handler in the case of an <see cref="Outcome.Error" /> outcome.
  /// </summary>
  /// <param name="handler">Action to perform on an Error result.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  [DebuggerStepThrough]
  public async Task<OkStage> OnErrorAsync(Func<TReason, Task> handler, CancellationToken cancellationToken = default)
  {
    if (_outcome != Outcome.Error) return new OkStage(this);

    cancellationToken.ThrowIfCancellationRequested();
    await handler(_reason).ConfigureAwait(false);

    return new OkStage(this);
  }

  /// <summary>
  ///   Represents an Error <see cref="Result{T}" />.
  /// </summary>
  /// <param name="reason">The reason for the failure.</param>
  /// <returns>An Error <see cref="Result{T}" /> with a reason for the failure.</returns>
  public static Result<TReason> Error(TReason reason)
  {
    return new Result<TReason>(reason);
  }

  // Convert an Ok to an Ok result.
  public static implicit operator Result<TReason>(OkResult _)
  {
    return new Result<TReason>();
  }

  // Convert a reason to an Error result.
  public static implicit operator Result<TReason>(TReason reason)
  {
    return new Result<TReason>(reason);
  }

  /// <summary>
  ///   Stage for handling the Ok outcome of a <see cref="Result{T}" />.
  /// </summary>
  public readonly struct OkStage(Result<TReason> result)
  {
    /// <summary>
    ///   Handler for the case of an <see cref="Outcome.Ok" /> outcome.
    /// </summary>
    /// <param name="handler">Action to perform on an Ok result.</param>
    [DebuggerStepThrough]
    public void OnOk(Action handler)
    {
      if (result._outcome != Outcome.Ok) return;

      handler();
    }

    /// <summary>
    ///   Async handler for the case of an <see cref="Outcome.Ok" /> outcome.
    /// </summary>
    /// <param name="handler">Action to perform on an Ok result.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    [DebuggerStepThrough]
    public Task OnOkAsync(Func<Task> handler, CancellationToken cancellationToken = default)
    {
      if (result._outcome != Outcome.Ok) return Task.CompletedTask;

      cancellationToken.ThrowIfCancellationRequested();
      return handler();
    }
  }
}

/// <summary>
///   The result of an operation that returns a value. If the operation failed, a failure reason is given.
/// </summary>
/// <typeparam name="TReason">The type of the error reason.</typeparam>
/// <typeparam name="TValue">The type of the returned value.</typeparam>
public readonly struct Result<TReason, TValue>
{
  private readonly TValue _value;
  private readonly TReason _reason;
  private readonly Outcome _outcome;

  private Result(TValue value)
  {
    _value = value;
    _reason = default!;
    _outcome = Outcome.Ok;
  }

  private Result(TReason reason)
  {
    _value = default!;
    _reason = reason;
    _outcome = Outcome.Error;
  }

  /// <summary>
  ///   Handler in the case of an <see cref="Outcome.Error" /> outcome.
  /// </summary>
  /// <param name="handler">Action to perform on an Error result. The parameter is the failure reason.</param>
  [DebuggerStepThrough]
  public OkStage OnError(Action<TReason> handler)
  {
    if (_outcome == Outcome.Error) handler(_reason);

    return new OkStage(this);
  }

  /// <summary>
  ///   Handler in the case of an <see cref="Outcome.Error" /> outcome.
  /// </summary>
  /// <param name="handler">
  ///   Action to perform on an Error result. The parameter is the failure reason, and in the case of an
  ///   <see cref="Outcome.Error" /> outcome the return value will be passed back to the caller.
  /// </param>
  /// <typeparam name="TResultValue">Type of the value passed back to the caller.</typeparam>
  [DebuggerStepThrough]
  public OkWithDefaultStage<TResultValue> OnError<TResultValue>(Func<TReason, TResultValue> handler)
  {
    TResultValue defaultValue = default!;
    if (_outcome == Outcome.Error) defaultValue = handler(_reason);

    return new OkWithDefaultStage<TResultValue>(this, defaultValue);
  }

  /// <summary>
  ///   Async handler in the case of an <see cref="Outcome.Error" /> outcome.
  /// </summary>
  /// <param name="handler">
  ///   Action to perform on an Error result. The parameter is the failure reason, and in the case of an
  ///   <see cref="Outcome.Error" /> outcome the return value will be passed back to the caller.
  /// </param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <typeparam name="TResultValue">Type of the value passed back to the caller.</typeparam>
  [DebuggerStepThrough]
  public async Task<OkWithDefaultStage<TResultValue>> OnErrorAsync<TResultValue>(
    Func<TReason, Task<TResultValue>> handler, CancellationToken cancellationToken = default)
  {
    TResultValue defaultValue = default!;
    if (_outcome != Outcome.Error) return new OkWithDefaultStage<TResultValue>(this, defaultValue);

    cancellationToken.ThrowIfCancellationRequested();
    defaultValue = await handler(_reason).ConfigureAwait(false);

    return new OkWithDefaultStage<TResultValue>(this, defaultValue);
  }

  /// <summary>
  ///   Async handler in the case of an <see cref="Outcome.Error" /> outcome.
  /// </summary>
  /// <param name="handler">Action to perform on an Error result. The parameter is the failure reason.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  [DebuggerStepThrough]
  public async Task<OkStage> OnErrorAsync(Func<TReason, Task> handler, CancellationToken cancellationToken = default)
  {
    if (_outcome != Outcome.Error) return new OkStage(this);

    cancellationToken.ThrowIfCancellationRequested();
    await handler(_reason).ConfigureAwait(false);

    return new OkStage(this);
  }

  // Convert a value of type TOk to an Ok result.
  public static implicit operator Result<TReason, TValue>(TValue value)
  {
    return new Result<TReason, TValue>(value);
  }

  // Convert a value of type TError to an Error result.
  public static implicit operator Result<TReason, TValue>(TReason reason)
  {
    return new Result<TReason, TValue>(reason);
  }

  /// <summary>
  ///   Stage for handling the Ok outcome of a <see cref="Result{T, T}" />.
  /// </summary>
  public readonly struct OkStage(Result<TReason, TValue> result)
  {
    /// <summary>
    ///   Handler for the case of an <see cref="Outcome.Ok" /> outcome.
    /// </summary>
    /// <param name="handler">Action to perform on an Ok result. The parameter is the success value.</param>
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OnOk(Action<TValue> handler)
    {
      if (result._outcome != Outcome.Ok) return;

      handler(result._value);
    }

    /// <summary>
    ///   Async handler for the case of an <see cref="Outcome.Ok" /> outcome.
    /// </summary>
    /// <param name="handler">Action to perform on an Ok result. The parameter is the success value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task OnOkAsync(Func<TValue, Task> handler, CancellationToken cancellationToken = default)
    {
      if (result._outcome != Outcome.Ok) return Task.CompletedTask;

      cancellationToken.ThrowIfCancellationRequested();
      return handler(result._value);
    }
  }

  /// <summary>
  ///   Stage for handling the Ok outcome of a <see cref="Result{T, T}" /> when a default value is provided in the case of
  ///   failure.
  /// </summary>
  public readonly struct OkWithDefaultStage<TResultValue>(Result<TReason, TValue> result, TResultValue defaultValue)
  {
    /// <summary>
    ///   Handler for the case of an <see cref="Outcome.Ok" /> outcome.
    /// </summary>
    /// <param name="handler">Action to perform on an Ok result. The parameter is the success value.</param>
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TResultValue OnOk(Func<TValue, TResultValue> handler)
    {
      return result._outcome != Outcome.Ok ? defaultValue : handler(result._value);
    }

    /// <summary>
    ///   Async handler for the case of an <see cref="Outcome.Ok" /> outcome.
    /// </summary>
    /// <param name="handler">Action to perform on an Ok result. The parameter is the success value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task<TResultValue> OnOkAsync(Func<TValue, Task<TResultValue>> handler,
      CancellationToken cancellationToken = default)
    {
      if (result._outcome != Outcome.Ok) return Task.FromResult(defaultValue);

      cancellationToken.ThrowIfCancellationRequested();
      return handler(result._value);
    }
  }
}

/// <summary>
///   An Ok <see cref="Result" /> or <see cref="Result{T}" />.
/// </summary>
public readonly struct OkResult;

/// <summary>
///   An Error <see cref="Result" />.
/// </summary>
public readonly struct ErrorResult;