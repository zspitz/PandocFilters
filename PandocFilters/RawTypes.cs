using System.Collections.Generic;
using OneOf;
using Newtonsoft.Json;
using DataValue = OneOf.OneOf<PandocFilters.RawTypes.TagContent, string, long, PandocFilters.RawTypes.TagContent1>;
using System.Collections.Immutable;

namespace PandocFilters.RawTypes {
    public record TagContent(string T, OneOf<
        DataValue[],
        DataValue
    >? C);

    // for tuples
    public class TagContent1 : List<DataValue> { }

    public record RawPandoc(
        [property: JsonProperty] int[] PandocApiVersion,
        [property: JsonProperty] ImmutableDictionary<string, TagContent> Meta,
        [property: JsonProperty] ImmutableList<TagContent> Blocks
    );
}
