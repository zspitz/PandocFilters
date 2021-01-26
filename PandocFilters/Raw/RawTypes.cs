using System.Collections.Generic;
using OneOf;
using Newtonsoft.Json;
using DataValue = OneOf.OneOf<PandocFilters.Raw.TagContent?, string, long, double, bool, PandocFilters.Raw.TagContent1, PandocFilters.Raw.Citation>;
using System.Collections.Immutable;

namespace PandocFilters.Raw {
    public record TagContent(string T, OneOf<
        ImmutableDictionary<string, DataValue>,
        DataValue[],
        DataValue        
    >? C);

    // for tuples
    public class TagContent1 : List<DataValue> { }

    public record RawPandoc(
        [property: JsonProperty(PropertyName ="pandoc-api-version")] int[] PandocApiVersion,
        [property: JsonProperty] ImmutableDictionary<string, TagContent> Meta,
        [property: JsonProperty] ImmutableList<TagContent> Blocks
    );

    // For some unknown reason, Citations get serialized as plain objects, not as objects with `t` and `c` properties
    public record Citation(string CitationId, TagContent1 CitationPrefix, TagContent1 CitationSuffix, TagContent CitationMode, int CitationNoteNum, int CitationHash);
}
