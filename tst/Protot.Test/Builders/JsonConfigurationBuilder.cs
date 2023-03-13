using Microsoft.Extensions.Configuration;
using System.Text;


namespace Protot.Test.Builders;

[ExcludeFromCodeCoverage]
internal class JsonConfigurationBuilder
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
			""Value"": ""StringValue:StringValue""
		}},

		{{
			""TransformationType"": ""ReplaceField"",
			""SubType"": ""renames"",
			""Value"": ""FloatValue:FloatValue""
		}},

		{{
			""TransformationType"": ""InsertField"",
			""SubType"": ""static"",
			""Value"": ""IntegerValue:Int32:173559425""
		}},
	]
}}";

	public IConfiguration BuildAllTypes() 
		=> this.Build(AllTypesJson);

	public IConfiguration Build(string json)
	{
		if (string.IsNullOrWhiteSpace(json))
		{
			throw new ArgumentException($"'{nameof(json)}' cannot be null or whitespace.", nameof(json));
		}

		using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
		return new ConfigurationBuilder()
			.AddJsonStream(stream)
			.Build();
	}
}
