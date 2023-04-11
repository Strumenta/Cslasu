using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using ExtensionMethods;
using Strumenta.Sharplasu.Model;
using Strumenta.Sharplasu.Validation;

namespace Strumenta.Sharplasu.Testing
{
    public class ASTDifferenceException : Exception
    {
        public string Context { get; }
        public object Expected { get; }
        public object Actual { get; }

        public ASTDifferenceException(string context, object expected, object actual)
            : this(context, expected, actual, $"{context}: expecting {expected}, actual {actual}")
        {
        }

        public ASTDifferenceException(string context, object expected, object actual, string message)
            : base(message)
        {
            Context = context;
            Expected = expected;
            Actual = actual;
        }
    }
    
    public static class Testing
    {
        public static void AssertParsingResultsAreEqual<T>(
            ParsingResult<T> expected, ParsingResult<T> actual,
            string context = "<root>", bool considerPosition = false)
            where T : Node
        {
            if (!expected.Issues.SequenceEqual(actual.Issues))
                throw new ASTDifferenceException(context, expected.Issues, actual.Issues);

            if (!(
                    (expected.Root == null && actual.Root == null) ||
                    (expected.Root != null && actual.Root != null)))
                throw new ASTDifferenceException(context, expected.Root, actual.Root,
                    $"Expected root {expected.Root}, actual {actual.Root}");
            
            AssertASTsAreEqual(expected.Root, actual.Root, context, considerPosition);
        }
        
        public static void AssertASTsAreEqual<TNode>(
            Node expected,
            ParsingResult<TNode> actual,
            string context = "<root>",
            bool considerPosition = false) where TNode : Node
        {
            if (actual.Issues.Count > 0)
                throw new ASTDifferenceException(context, expected, actual, actual.Issues.ToString());
            
            AssertASTsAreEqual(expected, actual.Root, context, considerPosition);
        }
        
        public static void AssertASTsAreEqual(
            Node expected,
            Node actual,
            string context = "<root>",
            bool considerPosition = false)
        {
            if (expected.GetType() != actual.GetType())
                throw new ASTDifferenceException(context, expected, actual,
                    $"{context}: expected node of type {expected.GetType().FullName}," +
                    $"but found {actual.GetType().FullName}");

            if (considerPosition && !expected.SpecifiedPosition.Equals(actual.SpecifiedPosition))
                throw new ASTDifferenceException(context, expected.SpecifiedPosition, actual.SpecifiedPosition,
                    $"{context}.position");

            foreach (var propertyInfo in expected.GetType().GetProperties())
            {
                var actualPropertyValue = propertyInfo.GetValue(actual);
                var expectedPropertyValue = propertyInfo.GetValue(expected);
                
                if (expectedPropertyValue == null || actualPropertyValue == null) continue;

                if (expectedPropertyValue is Node expectedNode && actualPropertyValue is Node actualNode)
                {
                    AssertASTsAreEqual(expectedNode, actualNode, context, considerPosition);
                }
                else if (expectedPropertyValue is IEnumerable<object> expectedIEnumerable && actualPropertyValue is IEnumerable<object> actualIEnumerable)
                {
                    if (expectedIEnumerable.Count() != actualIEnumerable.Count())
                        throw new ASTDifferenceException(context, expectedIEnumerable, actualIEnumerable,
                            $"{context}, comparing property {propertyInfo.Name}, expected {expectedIEnumerable}, actual {actualIEnumerable}");
                    for (var i = 0; i < expectedIEnumerable.Count(); i++)
                    {
                        var expectedElement = expectedIEnumerable.ElementAt(i);
                        var actualElement = actualIEnumerable.ElementAt(i);
                        if (expectedElement is Node expectedChild && actualElement is Node actualChild)
                        {
                            AssertASTsAreEqual(expectedChild, actualChild, $"{context}.{propertyInfo.Name}[{i}]");
                        }
                        else if (!expectedElement.Equals(actualElement))
                        {
                            throw new ASTDifferenceException(context, expectedElement, actualElement,
                                $"{context}, property {propertyInfo.Name}[{i}] is {expectedElement}, but found {actualElement}");
                        }
                    }
                }
                else if (!expectedPropertyValue.Equals(actualPropertyValue))
                    throw new ASTDifferenceException(context, expectedPropertyValue, actualPropertyValue,
                        $"{context}, comparing property {propertyInfo.Name}, expected {expectedPropertyValue}, actual {actualPropertyValue}");
            }
        }
    }
}

namespace ExtensionMethods
{
    public static class TestingExtensions
    {
        public static bool IsANode(this Type type)
        {
            return typeof(Node).IsAssignableFrom(type);
        }

        public static bool ProvidesNodes(this Type type)
        {
            return type.IsANode();
        }
    }
}