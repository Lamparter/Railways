using Microsoft.FSharp.Core;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Riverside.Railways;

/// <summary>
/// Represents the outcome of an operation, encapsulating its success status, result value, and any associated error.
/// </summary>
/// <remarks>
/// The <see cref="Result{T}"/> struct provides a standardized way to represent the result of an operation, including whether the operation succeeded (<see cref="IsSuccess"/>), the resulting value (<see cref="Value"/>), and any error information (<see cref="Error"/>).
/// It supports implicit conversions from values, exceptions, and F# result types, making it versatile for interoperation between different paradigms.
/// </remarks>
/// <typeparam name="T">The type of the value contained in the result.</typeparam>
[Serializable]
public struct Result<T> : IEquatable<Result<T>>, IEquatable<FSharpResult<T, object>>, IComparable<Result<T>>, IComparable<FSharpResult<T, object>>
{
	/// <summary>
	/// Indicates whether operation associated with this result was successful.
	/// </summary>
	/// <remarks>
	/// This property is a shorthand for <see cref="Status"/> property.
	/// </remarks>
	public readonly bool IsSuccess => Status;

	/// <summary>
	/// Indicates whether operation associated with this result failed.
	/// </summary>
	/// <remarks>
	/// This property is a shorthand for the inverse of the <see cref="Status"/> property.
	/// </remarks>
	public readonly bool IsFailure => !Status;

	/// <summary>
	/// The generic status of the result.
	/// </summary>
	/// <remarks>
	/// <c>true</c> indicates success, <c>false</c> indicates failure.
	/// </remarks>
	public bool Status { get; private set; }

	/// <summary>
	/// The value of the result.
	/// </summary>
	public T Value { get; private set; }

	/// <summary>
	/// The error associated with the result, if any.
	/// </summary>
	public object? Error { get; private set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="Result{T}"/> struct with the specified value.
	/// </summary>
	/// <param name="value">The <typeparamref name="T"/> value that the <see cref="Result{T}"/> struct will encapsulate.</param>
	public Result(T value)
	{
		Value = value;
		Status = Convert.ToBoolean(value);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Result{T}"/> struct with the specified F# result.
	/// </summary>
	/// <remarks>
	/// This constructor allows for easy conversion from F# results (<see cref="FSharpResult{T, TError}"/>) to the <see cref="Result{T}"/> struct.
	/// </remarks>
	/// <param name="fsharpResult">The <see cref="FSharpResult{T, TError}"/> instance to construct from.</param>
	public Result(FSharpResult<T, object> fsharpResult)
	{
		Value = fsharpResult.ResultValue;
		Status = fsharpResult.IsOk;
		Error = fsharpResult.ErrorValue;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Result{T}"/> struct with the specified exception.
	/// </summary>
	/// <remarks>
	/// This constructor allows for easy encapsulation of errors or exceptions that occur during operations, providing a direct way to represent failure in the result.
	/// </remarks>
	/// <param name="exception">The <see cref="Exception"/> instance to construct from.</param>
	public Result(Exception exception)
	{
		Value = default!;
		Status = false;
		Error = exception;
	}

	/// <summary>
	/// Determines whether the current <see cref="Result{T}"/> is equal to another <see cref="Result{T}"/>.
	/// </summary>
	/// <param name="other">The other <see cref="Result{T}"/> to compare with.</param>
	/// <returns><c>true</c> if the results are equal; otherwise, <c>false</c>.</returns>
	public readonly bool Equals(Result<T> other)
	{
		return (Status == other.Status) &&
			EqualityComparer<T>.Default.Equals(Value, other.Value) &&
			(Error == other.Error);
	}

	/// <summary>
	/// Determines whether the current <see cref="Result{T}"/> is equal to a specified F# result.
	/// </summary>
	/// <param name="other">The F# result to compare with.</param>
	/// <returns><c>true</c> if the results are equal; otherwise, <c>false</c>.</returns>
	public readonly bool Equals(FSharpResult<T, object> other)
	{
		return (Status == other.IsOk) &&
			EqualityComparer<T>.Default.Equals(Value, other.ResultValue) &&
			(Error == other.ErrorValue);
	}

	/// <summary>
	/// Compares the current <see cref="Result{T}"/> with another <see cref="Result{T}"/> and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other.
	/// </summary>
	/// <param name="other">The other <see cref="Result{T}"/> to compare with.</param>
	/// <returns>A value that indicates the relative order of the objects being compared.</returns>
	public readonly int CompareTo(Result<T> other)
	{
		int statusComparison = Status.CompareTo(other.Status);

		return statusComparison != 0
			? statusComparison
			: Status
			? Comparer<T>.Default.Compare(Value, other.Value)
			: Comparer<object>.Default.Compare(Error!, other.Error!);
	}

	/// <summary>
	/// Compares the current <see cref="Result{T}"/> with a specified F# result and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other.
	/// </summary>
	/// <param name="other">The F# result to compare with.</param>
	/// <returns>A value that indicates the relative order of the objects being compared.</returns>
	public readonly int CompareTo(FSharpResult<T, object> other)
	{
		int statusComparison = Status.CompareTo(other.IsOk);

		return statusComparison != 0
			? statusComparison
			: Status ? Comparer<T>.Default.Compare(Value, other.ResultValue)
			: Comparer<object>.Default.Compare(Error!, other.ErrorValue);
	}

	/// <summary>
	/// Projects the value of a successful result into a new form.
	/// </summary>
	/// <typeparam name="TResult">The type of the value returned by the mapping function.</typeparam>
	/// <param name="mapper">A transform function to apply to the value if the result is successful.</param>
	/// <returns>A new <see cref="Result{TResult}"/> containing the mapped value or the original error.</returns>
	public readonly Result<TResult> Map<TResult>(Func<T, TResult> mapper)
	{
		return Status
			? new Result<TResult>(mapper(Value))
			: new Result<TResult>((Exception?)Error ?? new Exception("No value"));
	}

	/// <summary>
	/// Projects the value of a successful result into a new <see cref="Result{TResult}"/>.
	/// </summary>
	/// <typeparam name="TResult">The type of the value returned by the binding function.</typeparam>
	/// <param name="binder">A transform function to apply to the value if the result is successful.</param>
	/// <returns>The result of the binding function or the original error.</returns>
	public readonly Result<TResult> Bind<TResult>(Func<T, Result<TResult>> binder)
	{
		return Status
			? binder(Value)
			: new Result<TResult>((Exception?)Error ?? new Exception("No value"));
	}

	/// <summary>
	/// Matches on the result, executing the appropriate function depending on success or failure.
	/// </summary>
	/// <typeparam name="TResult">The type of the value returned by the match functions.</typeparam>
	/// <param name="onSuccess">Function to execute if the result is successful.</param>
	/// <param name="onFailure">Function to execute if the result is a failure.</param>
	/// <returns>The result of the executed function.</returns>
	public readonly TResult Match<TResult>(Func<T, TResult> onSuccess, Func<object?, TResult> onFailure)
	{
		return Status
			? onSuccess(Value)
			: onFailure(Error);
	}

	/// <summary>
	/// Executes the specified action if the result is successful.
	/// </summary>
	/// <param name="action">The action to execute on the value.</param>
	/// <returns>The original <see cref="Result{T}"/>.</returns>
	public readonly Result<T> OnSuccess(Action<T> action)
	{
		if (Status)
			action(Value);

		return this;
	}

	/// <summary>
	/// Executes the specified action if the result is a failure.
	/// </summary>
	/// <param name="action">The action to execute on the error.</param>
	/// <returns>The original <see cref="Result{T}"/>.</returns>
	public readonly Result<T> OnFailure(Action<object?> action)
	{
		if (!Status)
			action(Error);

		return this;
	}

	/// <summary>
	/// Gets the value if the result is successful; otherwise, returns the specified default value.
	/// </summary>
	/// <param name="defaultValue">The value to return if the result is a failure.</param>
	/// <returns>The value if successful; otherwise, the default value.</returns>
	public readonly T GetValueOrDefault(T defaultValue = default!)
	{
		return Status
			? Value
			: defaultValue;
	}

	/// <summary>
	/// Gets the error if the result is a failure; otherwise, returns the specified default error.
	/// </summary>
	/// <param name="defaultError">The error to return if the result is successful.</param>
	/// <returns>The error if failed; otherwise, the default error.</returns>
	public readonly object? GetErrorOrDefault(object? defaultError = null)
	{
		return !Status
			? Error
			: defaultError;
	}

	/// <summary>
	/// Deconstructs the result into its success status, value, and error.
	/// </summary>
	/// <param name="isSuccess">Indicates whether the result is successful.</param>
	/// <param name="value">The value of the result.</param>
	/// <param name="error">The error associated with the result, if any.</param>
	public readonly void Deconstruct(out bool isSuccess, out T value, out object? error)
	{
		isSuccess = Status;
		value = Value;
		error = Error;
	}

	/// <summary>
	/// Returns a string representation of the result.
	/// </summary>
	/// <returns>A string that represents the result.</returns>
	public override readonly string ToString()
	{
		return Status
			? Value!.ToString()
			: Error!.ToString();
	}

	/// <summary>
	/// Implicitly converts an F# result to a <see cref="Result{T}"/>.
	/// </summary>
	/// <param name="fsharpResult">The F# result to convert.</param>
	public static implicit operator Result<T>(FSharpResult<T, object> fsharpResult) => new(fsharpResult);

	/// <summary>
	/// Implicitly converts a value to a successful <see cref="Result{T}"/>.
	/// </summary>
	/// <param name="value">The value to convert.</param>
	public static implicit operator Result<T>(T value) => new(value);

	/// <summary>
	/// Implicitly converts an exception to a failed <see cref="Result{T}"/>.
	/// </summary>
	/// <param name="exception">The exception to convert.</param>
	public static implicit operator Result<T>(Exception exception) => new(exception);

	/// <summary>
	/// Implicitly converts a <see cref="Result{T}"/> to its value.
	/// </summary>
	/// <param name="result">The result to convert.</param>
	public static implicit operator T(Result<T> result) => result.Value;

	/// <summary>
	/// Implicitly converts a <see cref="Result{T}"/> to an <see cref="Exception"/>, if the error is an exception.
	/// </summary>
	/// <param name="result">The result to convert.</param>
	public static implicit operator Exception?(Result<T> result) => result.Error as Exception;

	/// <summary>
	/// Implicitly converts a <see cref="Result{T}"/> to an F# result.
	/// </summary>
	/// <param name="result">The result to convert.</param>
	public static implicit operator FSharpResult<T, object>(Result<T> result)
	{
		return result.Status
			? FSharpResult<T, object>.NewOk(result.Value)
			: FSharpResult<T, object>.NewError(result.Error!);
	}

	/// <summary>
	/// Implicitly converts a <see cref="Result{T}"/> to a boolean indicating success.
	/// </summary>
	/// <param name="result">The result to convert.</param>
	public static implicit operator bool(Result<T> result) => result.Status;
}
