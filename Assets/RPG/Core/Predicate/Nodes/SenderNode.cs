namespace RPG.Core.Predicate.Nodes {
    public class SenderNode : ExpressionNode {
        public IdentifierNode Receiver { get; init; }
        public FunctionNode Action { get; init; }
    }
}
