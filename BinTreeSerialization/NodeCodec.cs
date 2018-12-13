using System;
using System.Text;

namespace BinTreeSerialization {
    /// <summary>
    /// Codec uses the following format to encode nodes: stringLength:string
    /// special conditions:
    /// when child node doesn't exist the stringLength encoded as "-1"
    /// when node exists but it's value is null the stringLength encoded as "-2"
    /// 
    /// exaple of serialized binary tree "4:root4:left9:left.left-1:-1:-1:5:right-1:-2:-1:0:-1:27:правый.правый.правый.правый28:right.right.right.right.left-1:-1:-1:"
    /// 
    /// </summary>
    public static class NodeCodec {

        const int NoNodeMarker = -1;
        const int NullNodeMarker = -2;


        public static string Serialize(Node node) {
            if(node == null) {
                return null;
            }
            var sb = new StringBuilder();
            EncodeNode(node, sb);
            return sb.ToString();
        }


        public static Node Deserialize(string src) {
            if(src == null) {
                return null;
            }

            try {
                Tuple<Node, int> decoded = DecodeNode(src, 0);
                return decoded.Item1;
            } catch(Exception e) {
                throw new Exception("Deserealization error", e);
            }
        }


        private static void EncodeNode(Node node, StringBuilder sb) {
            if(node == null) {
                // null node gets encoded as NoNodeMarker
                sb.Append(NoNodeMarker)
                    .Append(':');
                return;
            }

            // write marker representing node value's string length, NullNodeMarker if string is null
            if(node.Value == null) {
                sb.Append(NullNodeMarker)
                    .Append(':');
            } else {
                sb.Append(node.Value.Length)
                    .Append(':') // marker: length + ':'
                    .Append(node.Value);
            }

            EncodeNode(node.Left, sb);
            EncodeNode(node.Right, sb);
        }


        private static Tuple<Node, int> DecodeNode(string src, int position) {
            // get marker aka node value's string length (payLoacLength).
            int endOfMarker = src.IndexOf(':', position);
            int payloadLength = int.Parse(src.Substring(position, endOfMarker - position));

            // advance current position post marker
            position = endOfMarker + 1;

            if(payloadLength == NoNodeMarker) {
                return new Tuple<Node, int>(null, position);
            }

            string payload = null;
            if(payloadLength != NullNodeMarker) {
                payload = src.Substring(position, payloadLength);
                position += payloadLength;
            }

            Tuple<Node, int> left = DecodeNode(src, position);
            Tuple<Node, int> right = DecodeNode(src, left.Item2);

            var node = new Node {
                Value = payload,
                Left = left.Item1,
                Right = right.Item1
            };

            return Tuple.Create(node, right.Item2);
        }
    }
}
