using Microsoft.Extensions.Configuration;
using System.Text;

namespace PDM.Core.Test.Builders;

[ExcludeFromCodeCoverage]
internal class TransformationConfigurationBuilder
{
	public static string ConfigKey => "Transforms";

	public static string AllTypesJson => $@"
{{
	""{ConfigKey}"": [
		{{
			""TransformationType"": ""InsertField"",
			""SubType"": ""include"",
			""Value"": ""0""
		}},

		{{
			""TransformationType"": ""ReplaceField"",
			""SubType"": ""renames"",
			""Value"": ""3000:5""
		}},

		{{
			""TransformationType"": ""ReplaceField"",
			""SubType"": ""renames"",
			""Value"": ""4200:10""
		}},

		{{
			""TransformationType"": ""InsertField"",
			""SubType"": ""static"",
			""Value"": ""15:VarInt:173559425""
		}},
	]
}}";

	public IConfiguration BuildAllTypes() => this.Build(AllTypesJson);

	public IConfiguration Build(string json)
	{
		if (string.IsNullOrWhiteSpace(json))
		{
			throw new ArgumentException($"'{nameof(json)}' cannot be null or whitespace.", nameof(json));
		}

		var builder = new ConfigurationBuilder();

		using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
		builder.AddJsonStream(stream);

		return builder.Build();
	}
}
