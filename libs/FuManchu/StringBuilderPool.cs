// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

using System.Text;

using Microsoft.Extensions.ObjectPool;

namespace FuManchu;

static class StringBuilderPool
{
	static readonly ObjectPool<StringBuilder> _pool
		= new DefaultObjectPoolProvider().CreateStringBuilderPool();

	public static StringBuilder Rent() => _pool.Get();

	public static void Release(StringBuilder builder) => _pool.Return(builder);
}
