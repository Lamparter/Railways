using System;

namespace Riverside.Railways;

public interface IResult
{
	bool IsSuccess { get; }

	bool IsFailure { get; }

	bool Status { get; }

	object? Error { get; }
}
