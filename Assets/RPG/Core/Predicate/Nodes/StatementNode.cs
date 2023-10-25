using System.Collections.Generic;

namespace RPG.Core.Predicate.Nodes {
    public class StatementNode: ExpressionNode {
        private List<ExpressionNode> _nodes = new List<ExpressionNode>();

        public List<ExpressionNode> Nodes => _nodes;

        public void AddNode(ExpressionNode node) => _nodes.Add(node);
    }
}
