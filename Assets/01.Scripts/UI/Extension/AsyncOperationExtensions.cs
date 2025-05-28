using System.Threading.Tasks;
using UnityEngine;

public static class AsyncOperationExtensions
{
    // AsyncOperation 타입을 Task로 변환해주는 확장 메서드
    public static Task ToTask(this AsyncOperation asyncOp) 
    {
        // 단순히 완료만 알리는 용(반환값이 object)의 TaskCompletionSource를 만들어주고
        // asyncOp의 작업이 끝났을 때 호출되는 이벤트인 .completed에 
        // => 우리가 만든 Task도 "끝났다"고 알려준다.
        // 결과적으로 asyncOp가 끝났을 때, Task를 반환하면서 await가 가능해지는 것.
        var tcs = new TaskCompletionSource<object>();
        asyncOp.completed += _ => tcs.SetResult(null); 
        return tcs.Task;
    }
}