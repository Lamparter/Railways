using System;

namespace Riverside.Railways;

public interface IResult<T> : IResult
{
	T Value { get; }
}
