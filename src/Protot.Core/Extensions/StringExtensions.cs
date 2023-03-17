namespace Protot.Core.Extensions;

internal static class StringExtensions
{
   internal static IEnumerable<(string sourceField, string tragetField)> ParsePairs(this string value)
    {
        var fieldPairs = value.Split(new []{ ','}, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();

        return fieldPairs.Select(pair => pair.ParsePair()).ToList();
    }

   private static (string sourceField, string tragetField) ParsePair(this string pair)
   {
       (string, string) results = (String.Empty, String.Empty);

       var itemPair = pair.Split(':');
       if (itemPair.Length == 2)
       {
           results = (itemPair[0], itemPair[1]);
       }

       return results;
   }
   
   internal static string JoinPairs(this IEnumerable<string> pairs)
   {
       return string.Join(',', pairs);
   }
   
   internal static string ToLowerEnum(this Enum value)
   {
       return value.ToString().ToLowerInvariant();
   }

   internal static IEnumerable<string> EmbeddedFieldNames(this string value)
   {
       return value.Split('.', StringSplitOptions.RemoveEmptyEntries);
   }

   internal static (string fieldName, string type, string value) ParseInsert(this string transformValue)
   {
       var fieldInfo =  transformValue.Split(':');
       return (fieldInfo[0], fieldInfo[1], fieldInfo[2]);
   }
}