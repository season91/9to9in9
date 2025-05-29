using System.Collections.Generic;

public class SelectorNode : INode
{
    List<INode> children; // 여러 노드를 가질 수 있도록 리스트 생성

    public SelectorNode()
    {
        children = new List<INode>();
    }

    public void Add(INode node)
    {
        children.Add(node);
    } // 셀렉터에 자식노드를 추가하는 메서드

    public INode.State Evaluate()
    {
        // 리스트 내의 노드들을 왼쪽부터(넣은 순으로) 검사
        foreach (INode child in children)
        {

            INode.State state = child.Evaluate();
            // child 노드의 state 가 하나라도 SUCCESS 이면 성공을 반환
            // 실행 중인 경우 RUN 반환
            switch (state)
            {
                case INode.State.SUCCESS:
                    return INode.State.SUCCESS;
                case INode.State.RUN:
                    return INode.State.RUN;
            }
        }

        // 반복문이 끝났다면 해당 셀렉터의 자식노드들은 전부 FAILED 상태이므로 셀렉터는 FAILED 반환
        return INode.State.FAILED;
    }
}