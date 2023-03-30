using Strumenta.Sharplasu.Validation;
using Strumenta.Sharplasu.Model;

namespace Strumenta.Sharplasu.Serialization
{
    public interface ParseResultSerializer {
        string generateString<T>(ParsingResult<T> parseResult) where T : Node;
        string generateString<T>(T tree) where T : Node;
    }

    public interface ParseResultDeserializer {
        ParsingResult<T> deserializeResult<T>(string serializedParseResult) where T : Node;
        T deserializeTree<T>(string serializedTree) where T : Node;
    }
}
