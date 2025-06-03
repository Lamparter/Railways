using Microsoft.FSharp.Core;
using System.Collections.Generic;

namespace Riverside.Railways;

partial struct Result<T>
{
	public bool Equals(Result<T> other)
	{
		return (Status == other.Status) &&
			   EqualityComparer<T>.Default.Equals(Value, other.Value) &&
			   (Error == other.Error);
	}

	public bool Equals(FSharpResult<T, object> other)
	{
		return (Status == other.IsOk) &&
			   EqualityComparer<T>.Default.Equals(Value, other.ResultValue) &&
			   (Error == other.ErrorValue);
	}

	public int CompareTo(Result<T> other)
	{
		int statusComparison = Status.CompareTo(other.Status);
		if (statusComparison != 0)
			return statusComparison;

		if (Status)
		{
			return Comparer<T>.Default.Compare(Value, other.Value);
		}
		else
		{
			return Comparer<object>.Default.Compare(Error!, other.Error!);
		}
	}

	public int CompareTo(FSharpResult<T, object> other)
	{
		int statusComparison = Status.CompareTo(other.IsOk);
		if (statusComparison != 0)
			return statusComparison;

		if (Status)
		{
			return Comparer<T>.Default.Compare(Value, other.ResultValue);
		}
		else
		{
			return Comparer<object>.Default.Compare(Error!, other.ErrorValue);
		}
	}
}
