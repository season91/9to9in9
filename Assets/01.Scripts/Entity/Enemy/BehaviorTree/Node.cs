public interface INode
{
    public enum State
    { RUN, SUCCESS, FAILED }

    public INode.State Evaluate(); // 판단하여 상태 리턴
}