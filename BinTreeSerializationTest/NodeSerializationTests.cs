using System;
using Xunit;
using BinTreeSerialization;

namespace BinTreeSerializationTest
{
    public class NodeSerializationTests
    {
        [Theory]
        [InlineData("root value")]
        [InlineData("")]
        [InlineData(null)]
        public void TestRootNodeValue(string expectedValue)
        {
            var node = new Node() {
                Value = expectedValue
            };

            Node actual = Roundtrip(node);
            Assert.Equal(expectedValue, actual.Value);
            Assert.Null(node.Left);
            Assert.Null(node.Right);

        }

       /// <summary>
        /// Arbirtary tree with all possible node payload conditions
        /// </summary>
        [Fact]
        public void TestTree() {
            var node = new Node() {
                Value = "root",
                Left = new Node() {
                    Value = "left",
                    Left = new Node() {
                        Value = "left.left"
                    }
                },
                Right = new Node() {
                    Value = "right",
                    Right = new Node {
                        Value = null,
                        Right = new Node {
                            Value = "",
                            Right = new Node {
                                Value = "правый.правый.правый.правый",
                                Left = new Node {
                                    Value = "right.right.right.right.left"
                                }
                            }
                        }
                    }
                }
            };

            Node actual = Roundtrip(node);

            Assert.Equal("root", actual.Value);
            Assert.Equal("left.left", actual.Left.Left.Value);
            Assert.Null(actual.Right.Right.Value);
            Assert.Equal("", actual.Right.Right.Right.Value);
            Assert.Equal("правый.правый.правый.правый", actual.Right.Right.Right.Right.Value);
            Assert.Equal("right.right.right.right.left", actual.Right.Right.Right.Right.Left.Value);
            Assert.Null(actual.Right.Right.Right.Right.Right);
        }

        [Fact]
        public void TestDecodeException() {
            Exception e = Assert.Throws<Exception>(() => NodeCodec.Deserialize(""));
            Assert.Equal("Deserealization error", e.Message);

            e = Assert.Throws<Exception>(() => NodeCodec.Deserialize("bla bla bla"));
            Assert.Equal("Deserealization error", e.Message);
        }


        private static Node Roundtrip(Node node) {
            return NodeCodec.Deserialize(NodeCodec.Serialize(node));
        }
    }
}
