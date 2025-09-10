# PolymerLabs.Result

## Overview

The goal of `PolymerLabs.Result` is to provide alternative options for returning values and propegating errors to assist developers in writing cleaner, safer, more performant code.

This package defines two core types, `Result` and `ValueResult`, that each represent the result of an operation that may succeed or fail. Rather than relying on exceptions or nullable values, these types encode success (`Ok`) and failure (`Error`) states explicitly in the return type.

The result types provide functional interfaces (`When`, `WhenError`) to allow for different behaviors depending on the result, whilst also ensuring that both success and failure cases are always handled.

The design is inspired by [Rust’s `std::result`](https://doc.rust-lang.org/std/result/), adapted for idiomatic C# usage.

## Installation

TODO

## Usage

### Result\<TError\>

The following example demonstrates a validation method with multiple discrete checks. Each failure case returns a descriptive error message, while success returns a valueless `Ok`.

```cs
public enum ValidationError
{
  TooLong,
  TooShort,
  InvalidCharacter
}

public static Result<ValidationError> ValidateUsername(string username)
{
  // Implicit error (recommended): string is automatically converted to Result<string>.
  if (username.Length > 10) return ValidationError.TooLong;

  // Explicit error: inferred from the argument type.
  if (username.Length < 2) return ValidationError.TooShort;

  // Fully explicit error, typically not required.
  if (username.StartsWith('-')) return ValidationError.InvalidCharacter;

  // Success (recommended): implicit Ok.
  return Result.Ok;

  // Fully explicit Ok, typically not required.
  // return Result<string>.Ok();
}
```
Using the `ValidateUsername` method defined above, the following example demonstrates its usage. Parameter names are included for clarity, though optional in practice.

```cs
// Handling both Ok and Error cases:
ValidateUsername("my-username")
  .When(
    ok: () =>
    {
      // Actions to run on success.
    },
    error: message =>
    {
      Alert($"Failed username validation: {message}");
    });

// Handling only the Error case:
ValidateUsername("my-username")
  .WhenError(message =>
  {
    Alert($"Failed username validation: {message}");
  });
```

### ValueResult\<TValue, TError\>

`ValueResult<TValue, TError>` allows returning either a successful value or an error. Unlike `Result<TError>`, the success state carries a value of type `TValue`.  

```cs
public enum ParseError
{
  Unauthorized,
  Empty,
  InvalidFormat,
  OutOfRange
}

public ValueResult<int, ParseError> ParseInt(string input)
{
  if (!context.Permissions.ParseInt) return ParseError.Unauthorized;

  if (string.IsNullOrWhiteSpace(input)) return ParseError.Empty;

  if (!int.TryParse(input, out var value)) return ParseError.InvalidFormat;

  if (value is < 0 or > 100) return ParseError.OutOfRange;

  // Success and value: implicit conversion from TValue → Ok<int, ParseError>(value). 
  return value;

  // Explicit alternative:
  // return ValueResult<int, ParseError>.Ok(value);
}
```

To retrieve a value from `ValueResult<TValue, TError>`, call `When` or `WhenError`. These force failure handling and guarantees a result in all paths (you can still throw for unrecoverable errors).

```cs
const int DefaultId = 42;

// Handle only Error, it must return a value to be used in the case of an Error result.
var id2 = ParseInt("invalid")
  .WhenError(e =>
  {
    Console.WriteLine($"Parse unsuccessful ({e}).");
    // use a switch to spice up error handling.
    return e switch
    {
        ParseError.Empty => 0,
        ParseError.InvalidFormat or ParseError.OutOfRange => DefaultId,
        // in the case of unrecoverable errors, throw an exception.
        ParseError.Unauthorized => throw new UnauthorizedException()
    }
  });

// Handle both Ok and Error. Only the Error branch must return a value.
var id = ParseInt("11")
  .When(
    ok: v =>
    {
      Console.WriteLine("Parse successful.");
    },
    error: e =>
    {
      Console.WriteLine($"Parse unsuccessful ({e}).");
      return DefaultId;
    });
  });

// Handle both Ok and Error. In this case, both braches return a value.
var id = ParseInt("11")
  .When(
    ok: v =>
    {
      Console.WriteLine("Parse successful.");
      return v; // optionally return a value.
    },
    error: e =>
    {
      Console.WriteLine($"Parse unsuccessful ({e}).");
      return DefaultId;
    });
```

#### Mutations

Basic mutations can be performed on the value that is passed back. Expanding on the example above, we mutate the value on the Ok path.

```cs
// Clamp the parsed id to a maximum of 50.
var clampedId = ParseInt("73")
  .When(
    ok: v =>
    {
      Console.WriteLine("Parse successful; clamping to ≤ 50.");
      return Math.Min(v, 50); // enforcing an upper bound.
    },
    error: e =>
    {
      Console.WriteLine($"Parse unsuccessful ({e}), using default value.");
      return DefaultId;
    });
```

The type of the value can also be mutated, irrespective of the `TValue` of the `ValueResult`. The demonstration below converts the result to a `float` before returning it.

```cs
// Convert the parsed id to float; both branches must return the same type.
var floatId = ParseInt("42")
  .When(
    ok: v =>
    {
      Console.WriteLine("Parse successful.");
      return (float)v;
    },
    error: e =>
    {
      Console.WriteLine($"Parse unsuccessful ({e}), using default value.");
      return (float)DefaultId;
    });

Console.WriteLine(floatId / 8); // "5.25"
```
