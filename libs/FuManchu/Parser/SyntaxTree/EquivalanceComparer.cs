// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Parser.SyntaxTree;

using System.Collections.Generic;
using System.Text;

/// <summary>
/// Compares two objects by equivalance.
/// </summary>
public class EquivalanceComparer : IEqualityComparer<SyntaxTreeNode>
{
	private readonly StringBuilder? _builder = null;
	private readonly int _level = 0;

	public EquivalanceComparer(StringBuilder? builder = null, int level = 0)
	{
		_builder = builder;
		_level = level;
	}

	/// <inheritdoc />
	public bool Equals(SyntaxTreeNode x, SyntaxTreeNode y)
	{
		if (_builder == null)
		{
			return x.EquivalentTo(y);
		}

		return x.EquivalentTo(y, _builder, _level);
	}

	/// <inheritdoc />
	public int GetHashCode(SyntaxTreeNode obj)
	{
		return obj.GetHashCode();
	}
}
