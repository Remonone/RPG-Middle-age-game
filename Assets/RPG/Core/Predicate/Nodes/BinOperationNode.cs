namespace RPG.Core.Predicate.Nodes {
    public class BinOperationNode : ExpressionNode {
        public PredicateToken Operator { get; init; }
        public ExpressionNode LeftExpression { get; init; }
        public ExpressionNode RightExpression { get; init; }
    }
}
