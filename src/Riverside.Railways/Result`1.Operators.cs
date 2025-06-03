using Microsoft.FSharp.Core;
using System;

namespace Riverside.Railways;

partial struct Result<T>
{
	public static implicit operator Result<T>(FSharpResult<T, object> fsharpResult) => new(fsharpResult);

	public static implicit operator Result<T>(T value) => new(value);

	public static implicit operator Result<T>(Exception exception) => new(exception);

	public static implicit operator T(Result<T> result) => result.Value;

	public static implicit operator Exception?(Result<T> result) => result.Error as Exception;

	public static implicit operator FSharpResult<T, object>(Result<T> result)
	{
		return result.Status
			? FSharpResult<T, object>.NewOk(result.Value)
			: FSharpResult<T, object>.NewError(result.Error!);
	}

	public static implicit operator bool(Result<T> result) => result.Status;
}
