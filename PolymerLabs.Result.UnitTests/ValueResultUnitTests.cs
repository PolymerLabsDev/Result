using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace PolymerLabs.Result.UnitTests;

[TestFixture]
[ExcludeFromCodeCoverage]
public class ValueResultUnitTests
{
  private enum TestError
  {
    FirstError,
    SecondError
  }

  [Test]
  public void ValueResult_When_DoesInvokeCorrectMethod()
  {
    var okResult = ValueResult<int, TestError>.Ok(0);
    var errorResult = ValueResult<int, TestError>.Error(TestError.FirstError);

    const int errorValue = 20;
    const int okValue = 10;

    var okMatchResult = okResult.When(
      _ => okValue,
      _ => throw new UnreachableException());

    var errorMatchResult = errorResult.When(
      _ => throw new UnreachableException(),
      _ => errorValue);

    Assert.Multiple(() =>
    {
      Assert.That(okMatchResult, Is.EqualTo(okValue));
      Assert.That(errorMatchResult, Is.EqualTo(errorValue));
    });
  }

  [Test]
  public void ValueResult_When_DoesPassThroughValueAndError()
  {
    const int okSetValue = 20;
    const TestError errorSetValue = TestError.SecondError;

    var okResult = ValueResult<int, TestError>.Ok(okSetValue);
    var errorResult = ValueResult<int, TestError>.Error(errorSetValue);

    int? retrievedOkValue = null;
    okResult.When(
      v =>
      {
        retrievedOkValue = v;
        return v;
      },
      _ => throw new UnreachableException());

    TestError? retrievedErrorValue = null;
    errorResult.When(
      _ => throw new UnreachableException(),
      e =>
      {
        retrievedErrorValue = e;
        return 0;
      });

    Assert.Multiple(() =>
    {
      Assert.That(retrievedOkValue, Is.Not.Null);
      Assert.That(retrievedOkValue, Is.EqualTo(okSetValue));
      Assert.That(retrievedErrorValue, Is.Not.Null);
      Assert.That(retrievedErrorValue, Is.EqualTo(errorSetValue));
    });
  }

  [Test]
  public void ValueResult_When_DoesReturnValueOfDifferentType()
  {
    var okResult = ValueResult<int, TestError>.Ok(0);
    var errorResult = ValueResult<int, TestError>.Error(TestError.FirstError);

    const string errorValue = "20";
    const string okValue = "10";

    var okMatchResult = okResult.When(
      _ => okValue,
      _ => throw new UnreachableException());

    var errorMatchResult = errorResult.When(
      _ => throw new UnreachableException(),
      _ => errorValue);

    Assert.Multiple(() =>
    {
      Assert.That(okMatchResult, Is.EqualTo(okValue));
      Assert.That(errorMatchResult, Is.EqualTo(errorValue));
    });
  }

  [Test]
  public void ValueResult_WhenError_OnlyCalledWhenError()
  {
    const int okValue = 10;
    var okResult = ValueResult<int, TestError>.Ok(okValue);
    var errorResult = ValueResult<int, TestError>.Error(TestError.FirstError);

    const int errorValue = 10;
    var errorMatchResult = errorResult.WhenError(_ => errorValue);
    var okMatchResult = okResult.WhenError(_ => throw new UnreachableException());

    Assert.Multiple(() =>
    {
      Assert.That(okMatchResult, Is.EqualTo(okValue));
      Assert.That(errorMatchResult, Is.EqualTo(errorValue));
    });
  }

  [Test]
  public void ValueResult_WhenError_DoesPassError()
  {
    const TestError error = TestError.SecondError;
    var result = ValueResult<int, TestError>.Error(TestError.SecondError);


    var retrievedError = TestError.FirstError;
    result.WhenError(e =>
    {
      retrievedError = e;
      return 0;
    });

    Assert.That(retrievedError, Is.EqualTo(error));
  }

  [Test]
  public void ValueResult_Creators_DoesCreateMatchingResult()
  {
    const int okValue = 5;
    const int errorValue = 10;
    var okResult = ValueResult.Ok<int, TestError>(okValue);
    var errorResult = ValueResult.Error<int, TestError>(TestError.SecondError);

    var okMatchResult = okResult.When(
      _ => okValue,
      _ => throw new UnreachableException());
    var errorMatchResult = errorResult.When(
      _ => throw new UnreachableException(),
      _ => errorValue);

    Assert.Multiple(() =>
    {
      Assert.That(okMatchResult, Is.EqualTo(okValue));
      Assert.That(errorMatchResult, Is.EqualTo(errorValue));
    });
  }

  [Test]
  public void ValueResult_Operators_DoesCreateMatchingResult()
  {
    const int okValue = 5;
    const int errorValue = 10;

    ValueResult<int, TestError> okValueResult = okValue;
    ValueResult<int, TestError> errorValueResult = TestError.SecondError;

    var okMatchResult = okValueResult.When(
      _ => okValue,
      _ => throw new UnreachableException());
    var errorMatchResult = errorValueResult.When(
      _ => throw new UnreachableException(),
      _ => errorValue);

    Assert.Multiple(() =>
    {
      Assert.That(okMatchResult, Is.EqualTo(okValue));
      Assert.That(errorMatchResult, Is.EqualTo(errorValue));
    });
  }
}