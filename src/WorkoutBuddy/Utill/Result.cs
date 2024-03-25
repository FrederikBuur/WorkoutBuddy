using System.Diagnostics.Contracts;
using WorkoutBuddy.Features.ErrorHandling;

public readonly struct Result<A>
{
    internal readonly ResultState State;
    internal readonly A? Value;
    internal readonly Error? Error;

    public Result(A value)
    {
        State = ResultState.Success;
        Value = value;
        Error = null;
    }

    public Result(Error e)
    {
        State = ResultState.Faulted;
        Error = e;
        Value = default(A);
    }

    public static implicit operator Result<A>(A value) => new(value);

    [Pure]
    public bool IsFaulted =>
    State == ResultState.Faulted;

    [Pure]
    public bool IsSuccess =>
        State == ResultState.Success;

    [Pure]
    public R Match<R>(Func<A, R> Succ, Func<Error, R> Fail) =>
    IsFaulted
        ? Fail(Error!)
        : Succ(Value!);
}

public enum ResultState : byte
{
    Faulted,
    Success
}