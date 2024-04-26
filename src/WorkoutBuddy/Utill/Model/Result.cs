using System.Diagnostics.Contracts;
using WorkoutBuddy.Util.ErrorHandling;

public readonly struct Result<A>
{
    internal readonly ResultState State;
    internal readonly A Value;
    internal readonly Error Error;

    public Result(A value)
    {
        State = ResultState.Success;
        Value = value;
        Error = null!;
    }

    public Result(Error e)
    {
        State = ResultState.Faulted;
        Error = e;
        Value = default!;
    }

    public static implicit operator Result<A>(A value) => new Result<A>(value);

    [Pure]
    public bool IsFaulted =>
    State == ResultState.Faulted;

    [Pure]
    public bool IsSuccess =>
        State == ResultState.Success;

    [Pure]
    public R Match<R>(Func<A, R> Success, Func<Error, R> Fail) =>
    IsFaulted
        ? Fail(Error!)
        : Success(Value!);
}

public enum ResultState : byte
{
    Faulted,
    Success
}


public class Result2<TValue, TError>
{
    public readonly TValue? Value;
    public readonly TError? Error;

    private bool _isSuccess;

    private Result2(TValue value)
    {
        _isSuccess = true;
        Value = value;
        Error = default;
    }

    private Result2(TError error)
    {
        _isSuccess = false;
        Value = default;
        Error = error;
    }

    //happy path
    public static implicit operator Result2<TValue, TError>(TValue value) => new Result2<TValue, TError>(value);

    //error path
    public static implicit operator Result2<TValue, TError>(TError error) => new Result2<TValue, TError>(error);

    public Result2<TValue, TError> Match(Func<TValue, Result2<TValue, TError>> success, Func<TError, Result2<TValue, TError>> failure)
    {
        if (_isSuccess)
        {
            return success(Value!);
        }
        return failure(Error!);
    }

}