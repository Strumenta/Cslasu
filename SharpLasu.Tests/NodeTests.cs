﻿using Strumenta.Sharplasu.Model;
using Strumenta.Sharplasu.Testing;
using Strumenta.Sharplasu.Tests.Models;
using Strumenta.Sharplasu.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strumenta.Sharplasu.Tests
{

    [TestClass]
    public class NodeTests
    {
        [TestMethod]
        public void CheckingChildrenAreFound()
        {
            var one = new TopNode
            {
                GoodStuff = 1,
                BadStuff = 2,
                Smaller = new SmallNode
                {
                    Description = "I stand here"
                }
            };
            List<Issue> issues = new List<Issue>();

            Assert.AreEqual(1, one.Children.Count);
        }

        [TestMethod]
        public void CheckingNodeArePrintCorrectly()
        {
            var one = new TopNode
            {
                GoodStuff = 1,
                BadStuff = 2,
                Smaller = new SmallNode
                {
                    Description = "I stand here"
                }
            };
            List<Issue> issues = new List<Issue>();

            Assert.AreEqual(
@"TopNode
  GoodStuff 1
  BadStuff 2
  SmallNode
    Description I stand here
", one.MultiLineString());
        }

        [TestMethod]
        public void CheckingParseTreeNodeAndPositionAreAdded()
        {
            var one = new TopNode
            {
                GoodStuff = 1,
                BadStuff = 2,                
            };
            List<Issue> issues = new List<Issue>();

            Assert.IsNull(one.SpecifiedPosition);
            Assert.IsNull(one.ParseTreeNode);

            one.WithParseTreeNode(new Antlr4.Runtime.ParserRuleContext());
            one.WithPosition(new Position(new Point(1,1), new Point(1,2)));
            
            Assert.IsNotNull(one.SpecifiedPosition);
            Assert.IsNotNull(one.ParseTreeNode);

        }
    }
}
