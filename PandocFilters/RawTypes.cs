using System.Collections.Generic;
using OneOf;
using Newtonsoft.Json;
using DataValue = OneOf.OneOf<PandocFilters.RawTypes.TagContent, string, long, PandocFilters.RawTypes.TagContent1>;

namespace PandocFilters.RawTypes {
    public record TagContent(string T, OneOf<
        DataValue[],
        DataValue
    >? C);

    // for tuples
    public class TagContent1 : List<DataValue> { }

    public class RawPandoc {
        [JsonProperty] public int[] PandocApiVersion = default!;
        [JsonProperty] public Dictionary<string, TagContent> Meta = default!;
        [JsonProperty] public TagContent[] Blocks = default!;
    }
}
