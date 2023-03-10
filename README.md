# PDM: Protobuf Data Mapper
PDM is a library for mapping and transforming one [Protobuf](https://protobuf.dev/) message into another. 

It was initially hallucinated by ChatGPT as a Python library, but this implementation is in C#.

## Installation
You can install PDM via NuGet:

```powershell
Install-Package ProtobufDataMapper
```

Or via .NET CLI:

```bash
dotnet add package ProtobufDataMapper
```

## Usage
Here's a basic example of how to use PDM:

### Sample Code

```csharp
public byte[] Transform(byte[] sourceMessage)
{
	// Transforms from the supplied sourceMessage which 
	// is created from the ThreeFields protobuf and arrives in 
	// ProtoBuf wire format into a TwoFields type message which
	// is returned in ProtoBuf wire format.

	var targetMapping = new List<PDM.Entities.Transformation>()
		{
			{ new PDM.Entities.Transformation() // Remove field 10 from the output
				{
					TransformationType = PDM.Enums.TransformationType.ReplaceField,
					SubType = "blacklist",
					Value = "10"
				}
			}
		};

	var mapper = new PDM.ProtobufMapper(targetMapping);
	return await mapper.MapAsync(sourceMessage);
}
```

This code is based on the following ProtoBuf file definitions:

### Source Message

```protobuf
message ThreeFields {
    string StringValue = 5;
    float FloatValue = 10;
    int32 IntegerValue = 15;
}
```

### Target Message

```protobuf
message TwoFields {
    string StringValue = 5;
    int32 IntegerValue = 15;
}
```

If no transformations are supplied, all fields are copied directly to the output. Thus, if no 
changes are needed and the source message is to be simply copied to the target, no Transformations 
are required. In this example the target message only requires 2 of the 3 fields, so we use 
the `ReplaceField.blacklist` transform to remove the unwanted field from the output while 
still copying the other 2 fields directly to the target.

Target mappings can be supplied by configuration allowing the same codebase to be 
reconfigured and used for various transformations as needed.

### Json Configuration

```json
{
	"Transformations": [
		{
			"TransformationType": "InsertField",
			"SubType": "include",
			"Value": "0"
		},

		{
			"TransformationType": "ReplaceField",
			"SubType": "renames",
			"Value": "3000:5"
		},

		{
			"TransformationType": "ReplaceField",
			"SubType": "renames",
			"Value": "4200:10"
		},

		{
			"TransformationType": "InsertField",
			"SubType": "static",
			"Value": "15:VarInt:173559425"
		},
	]
}
```

In your application code, you can dynamically transform protobuf messages by injecting an `IConfiguration` instance,
and then building a `ProtobufMapper` with the specified configurations.

```csharp
public class MyTransformer
{
	private readonly ProtobufMapper mapper;

	public MyTransformer(IConfiguration configuration)
	{
		var transformations = configuration?.BuildTransformations("Transformations") ?? throw new ArgumentNullException(configuration);
		this.mapper = new ProtobufMapper(transformations);
	}

	public async Task<byte[]> TransformMessage(byte[] sourceMessageBytes)
	{
		var resultMessageBytes = await this.mapper.MapAsync(sourceMessageBytes);
		return resultMessageBytes;
	}
}
```

## Contributing
Contributions are welcome! If you have an idea for a new feature or find a bug, please open an issue. If you'd like to contribute code, please fork the repository and submit a pull request.

Before submitting a pull request, please ensure that your code passes the existing unit tests and that any new code is adequately tested. To run the tests locally, run:

## License
PDM is licensed under the GNU Affero General Public License v3.0. See [LICENSE](https://github.com/bsstahl/PDM/blob/main/LICENSE) for more information.
