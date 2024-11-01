// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu;

using System.Collections.Generic;

/// <summary>
/// Defines the required contract for implementing an operator provider.
/// </summary>
public interface IOperatorProvider
{
	/// <summary>
	/// Gets the available operators.
	/// </summary>
	/// <returns>The set of operators.</returns>
	IEnumerable<IOperator> GetOperators();
}
