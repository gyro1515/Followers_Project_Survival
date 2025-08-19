public class BehaviourTreeRunner
{
    INode _rootNode;
    public BehaviourTreeRunner(INode rootNode)
    {
        _rootNode = rootNode;
    }

    public void Operate()
    {
        _rootNode.Evaluate();
    }
}