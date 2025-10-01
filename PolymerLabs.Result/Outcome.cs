namespace PolymerLabs.Result;

/// <summary>
///   The outcome of a process.
/// </summary>
internal enum Outcome
{
  /// <summary>
  ///   The process succeeded and is Ok.
  /// </summary>
  Ok = 0,

  /// <summary>
  ///   The process failed and resulted in an Error.
  /// </summary>
  Error = 1
}