using System.Text.Json;
using System.Text.Json.Serialization;
using Strumenta.Sharplasu.Serialization.Json;
using Strumenta.Sharplasu.Serialization.Xml;
using Strumenta.Sharplasu.Tests.Models;
using Strumenta.Sharplasu.Validation;

namespace Strumenta.Sharplasu.Tests {

    [TestClass]
    public class XmlTest
    {
        [TestMethod]
        public void TestSerializationAndBack() {
            var parser = new ExampleSharpLasuParser();
            var tree = parser.GetTreeForText("set foo = 123 set bar = 1.23");
            var serializedResult = new XmlGenerator().generateString(tree);
            var deserializedResult = new XmlDeserializer().deserializeResult<CompilationUnit>(serializedResult);
            Assert.AreEqual(tree, deserializedResult);
        }
    }

    [TestClass]
    public class JsonTest
    {
        [TestMethod]
        public void TestSerializationAndBack() {
            var parser = new ExampleSharpLasuParser();
            var tree = parser.GetTreeForText("set foo = 123 set bar = 1.23");
            var serializedResult = new JsonGenerator().generateString(tree);
            var deserializedResult = new JsonDeserializer().deserializeResult<CompilationUnit>(serializedResult);
            Assert.AreEqual(tree, deserializedResult);
        }

        [TestMethod]
        public void TestCompilationUnitSerializationAndBack() {
            var parser = new ExampleSharpLasuParser();
            var parseResult = parser.GetTreeForText("set foo = 123 set bar = 1.23");
            var options = new JsonSerializerOptions {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true
            };
            var compilationUnit = parseResult.Root;

            Assert.IsTrue(parseResult.Correct);
            Assert.IsNotNull(compilationUnit);

            var serializedCompilationUnit = JsonSerializer.Serialize(compilationUnit, options);
            var deserializedResult = JsonSerializer.Deserialize<CompilationUnit>(serializedCompilationUnit, options);

            Assert.AreEqual(deserializedResult, compilationUnit);
        }

        [TestMethod]
        public void TestJsonParseResultIssuesDeserialization()
        {
            string json =
@"{
    ""Issues"" : [{
        ""IssueType"" : 0,
        ""Message"" : ""This is an example of a lexical error"",
        ""Position"" : null,
        ""IssueSeverity"" : 0
    }, {
        ""IssueType"" : 1,
        ""Message"" : ""This is an example of a syntactic error"",
        ""Position"" : null,
        ""IssueSeverity"" : 2
    }]
}";

            var deserializedResult = new JsonDeserializer().deserializeResult<CompilationUnit>(json);
            Assert.IsInstanceOfType(deserializedResult, typeof(ParsingResult<CompilationUnit>));
            Assert.AreEqual(2, deserializedResult.Issues.Count);
            Assert.AreEqual(
                new Issue(IssueType.LEXICAL, "This is an example of a lexical error", null),
                deserializedResult.Issues[0]);
            Assert.AreEqual(
                new Issue(IssueType.SYNTACTIC, "This is an example of a syntactic error", null, IssueSeverity.Info),
                deserializedResult.Issues[1]);
        }
    }
}