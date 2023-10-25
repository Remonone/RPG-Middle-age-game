using System.Collections.Generic;

namespace RPG.Core.Predicate.Nodes {
    public class FunctionNode : ExpressionNode {
        public PredicateToken Name { get; init; }
        public List<ExpressionNode> Args { get; init; }
    }
}
