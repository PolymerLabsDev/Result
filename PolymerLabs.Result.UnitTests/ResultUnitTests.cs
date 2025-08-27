using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace PolymerLabs.Result.UnitTests;

[TestFixture]
[ExcludeFromCodeCoverage]
public class ResultUnitTests
{
  private enum TestError
  {
    FirstError,
    SecondError
  }

  [Test]
  public void Result_When_DoesInvokeCorrectMethod()
  {
    var okResult = Result<TestError>.Ok();
    var errorResult = Result<TestError>.Error(TestError.FirstError);

    Assert.DoesNotThrow(() => okResult.When(
      () => { },
      _ => throw new UnreachableException()));

    Assert.DoesNotThrow(() => errorResult.When(
      () => throw new UnreachableException(),
      _ => { }));
  }

  [Test]
  public void Result_When_DoesInvokeCorrectMethod_WhenUsingImplicitConversion()
  {
    Result<TestError> okResult = Result.Ok;
    Result<TestError> errorResult = TestError.SecondError;

    Assert.DoesNotThrow(() => okResult.When(
      () => { },
      _ => throw new UnreachableException()));

    Assert.DoesNotThrow(() => errorResult.When(
      () => throw new UnreachableException(),
      _ => { }));
  }

  [Test]
  public void Result_When_DoesPassThroughError()
  {
    const TestError error = TestError.SecondError;
    var result = Result.Error(error);

    var retrievedTestError = TestError.FirstError;
    result.When(
      () => throw new UnreachableException(),
      e => { retrievedTestError = e; });

    Assert.That(retrievedTestError, Is.EqualTo(error));
  }

  [Test]
  public void Result_WhenError_OnlyCallsWhenError()
  {
    var errorResult = Result.Error(TestError.FirstError);
    Result<TestError> okResult = Result.Ok;

    var errorCalled = false;
    errorResult
      .WhenError(_ => { errorCalled = true; });

    Assert.DoesNotThrow(() =>
    {
      okResult
        .WhenError(_ => throw new UnreachableException());
    });

    Assert.That(errorCalled, Is.True);
  }
}