// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

using FuManchu.Text;
using FuManchu.Tokenizer;

BenchmarkSwitcher.FromAssembly(typeof(Benchmarking).Assembly).Run(args);

//Benchmarking b = new();
//for (int i = 0; i < 10_000; i++)
//{
//	b.Test();
//}

[MemoryDiagnoser, ShortRunJob]
public class Benchmarking
{
	[Benchmark]
	public void Test()
	{
		const string Template = "{{#if Variable}}Hello World{{/if}}";

		Baseline(Template);
	}

	void Baseline(string template)
	{
		using var reader = new StringReader(template);
		using var source = new SeekableTextReader(reader);
		using var tokenizer = new HandlebarsTokenizer(source);

		while (tokenizer.NextSymbol() != null) { }
	}
}
