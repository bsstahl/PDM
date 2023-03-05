# PDM: Protobuf Data Mapper
PDM is a library for mapping and transforming one [Protobuf](https://protobuf.dev/) message into another. 

It was initially hallucinated by ChatGPT as a Python library, but this implementation is in C#.

## Installation
You can install PDM via NuGet:

```powershell
Install-Package PDM
```

Or via .NET CLI:

```bash
dotnet add package PDM
```

## Usage
Here's a basic example of how to use PDM:

```csharp
public byte[] Transform(byte[] sourceMessage)
{
	// Transforms from the supplied sourceMessage which 
	// is created from the ThreeFields protobuf and arrives in 
	// ProtoBuf wire format into a TwoFields type message which
	// is returned in ProtoBuf wire format.

	// Source Message ProtoBuf:
	// message ThreeFields {
	//	  string StringValue = 5;
	//	  float FloatValue = 10;
	//	  int32 IntegerValue = 15;
	// }

	// Target Message ProtoBuf:
	// message TwoFields {
	//	  string StringValue = 5;
	//	  int32 IntegerValue = 15;
	// }

	// By default, all fields are transformed directly
	// thus, if no changes are needed and the message is to
	// be simply copied, no Transformations are required.
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

## Contributing
Contributions are welcome! If you have an idea for a new feature or find a bug, please open an issue. If you'd like to contribute code, please fork the repository and submit a pull request.

Before submitting a pull request, please ensure that your code passes the existing unit tests and that any new code is adequately tested. To run the tests locally, run:

## License
PDM is licensed under the GNU Affero General Public License v3.0. See [LICENSE](.\LICENSE) for more information.