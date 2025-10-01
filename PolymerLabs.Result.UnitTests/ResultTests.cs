namespace PolymerLabs.Result.UnitTests;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

[TestFixture]
[ExcludeFromCodeCoverage]
public class ResultTests
{
  private enum TestError
  {
    FirstError,
    SecondError
  }

  [Test]
  public void Result_OnErrorOnOk_DoesInvokeCorrectMethod()
  {
    Result okResult = Result.Ok;
    Result errorResult = Result.Error;
    
    Assert.DoesNotThrow(() =>
    {
      okResult
        .OnError(() => throw new UnreachableException())
        .OnOk(() => { });
    });

    Assert.DoesNotThrow(() =>
    {
      errorResult
        .OnError(() => { })
        .OnOk(() => throw new UnreachableException());
    });
  }
  
  [Test]
  public void Result_OnErrorAsyncOnOkAsync_DoesInvokeCorrectMethod()
  {
    Result okResult = Result.Ok;
    Result errorResult = Result.Error;
    
    Func<Task> throwFunc = () => throw new UnreachableException();
    var blankFunc = () => Task.CompletedTask;
    
    Assert.DoesNotThrow(async () =>
    {
      await (await okResult
        .OnErrorAsync(throwFunc)).OnOkAsync(blankFunc);
    });

    Assert.DoesNotThrow(async () =>
    {
      await (await errorResult
        .OnErrorAsync(blankFunc)).OnOkAsync(throwFunc);
    });
  }

  [Test]
  public void Result_When_DoesPassThroughError()
  {
    const TestError error = TestError.SecondError;
    var result = Result.OnError(error);

    var retrievedTestError = TestError.FirstError;
    result.On(
      () => throw new UnreachableException(),
      e => { retrievedTestError = e; });

    Assert.That(retrievedTestError, Is.EqualTo(error));
  }

  [Test]
  public void Result_WhenError_OnlyCallsWhenError()
  {
    var errorResult = Result.OnError(TestError.FirstError);
    Result<TestError> okResult = Result.Ok();

    var errorCalled = false;
    errorResult
      .OnError(_ => { errorCalled = true; });

    Assert.DoesNotThrow(() =>
    {
      okResult
        .OnError(_ => throw new UnreachableException());
    });

    Assert.That(errorCalled, Is.True);
  }
  
  [Test]
  public void ResultWithReason_When_DoesInvokeCorrectMethod()
  {
    Result<TestError> okResult = Result.Ok;
    Result<TestError> errorResult = TestError.SecondError;

    Assert.DoesNotThrow(() => okResult.);

    Assert.DoesNotThrow(() => errorResult.On(
      () => throw new UnreachableException(),
      _ => { }));
  }

  [Test]
  public void ResultWithReason_When_DoesPassThroughError()
  {
    const TestError error = TestError.SecondError;
    var result = Result.OnError(error);

    var retrievedTestError = TestError.FirstError;
    result.On(
      () => throw new UnreachableException(),
      e => { retrievedTestError = e; });

    Assert.That(retrievedTestError, Is.EqualTo(error));
  }

  [Test]
  public void ResultWithReason_WhenError_OnlyCallsWhenError()
  {
    var errorResult = Result.OnError(TestError.FirstError);
    Result<TestError> okResult = Result.Ok();

    var errorCalled = false;
    errorResult
      .OnError(_ => { errorCalled = true; });

    Assert.DoesNotThrow(() =>
    {
      okResult
        .OnError(_ => throw new UnreachableException());
    });

    Assert.That(errorCalled, Is.True);
  }
}