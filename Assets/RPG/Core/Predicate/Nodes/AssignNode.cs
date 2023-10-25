namespace RPG.Core.Predicate.Nodes {
    public class AssignNode : ExpressionNode {
        public PredicateToken Name { get; init; }
        public ExpressionNode Value { get; init; }
    }
}
