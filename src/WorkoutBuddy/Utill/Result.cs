using System.Diagnostics.Contracts;
using WorkoutBuddy.Features.ErrorHandling;

public readonly struct Result<A>
{
    internal readonly ResultState State;
    internal readonly A? Value;
    readonly HttpResponseException? exception;

    public Result(A value)
    {
        State = ResultState.Success;
        Value = value;
        exception = null;
    }

    public Result(HttpResponseException e)
    {
        State = ResultState.Faulted;
        exception = e;
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
    public R Match<R>(Func<A, R> Succ, Func<HttpResponseException, R> Fail) =>
    IsFaulted
        ? Fail(exception!)
        : Succ(Value!);
}

public enum ResultState : byte
{
    Faulted,
    Success
}