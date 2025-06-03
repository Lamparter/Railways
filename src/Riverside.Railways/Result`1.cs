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
public partial struct Result<T> : IConvertible, IEquatable<Result<T>>, IEquatable<FSharpResult<T, object>>, IComparable<Result<T>>, IComparable<FSharpResult<T, object>>
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

	public override string ToString()
	{
		if (Status)
			return Value!.ToString();
		else
			return Error!.ToString();
	}
}
