using System.Collections;
using UnityEngine;

// 주간(Week) 흐름에서 재생되는 컷신 플레이어의 공통 기반 클래스입니다.
// 실제 컷신 종류(영상, 애니메이션, 연출 UI 등)에 따라
// 이 클래스를 상속해서 재생 방식만 구체적으로 구현하면 됩니다.
public abstract class WeekFlowCutscenePlayerBase : MonoBehaviour
{
    // 이 플레이어가 담당하는 컷신의 고유 ID입니다.
    // 외부에서 특정 컷신을 재생 요청할 때 어떤 플레이어가 처리할지 구분하는 용도로 사용됩니다.
    [SerializeField] private string _cutsceneId = string.Empty;

    // 이 컷신이 "흐름을 막는(blocking)" 타입인지 여부입니다.
    // true이면 컷신이 끝날 때까지 다음 진행으로 넘어가지 않도록 제어할 수 있습니다.
    // false이면 컷신 재생과 별개로 다음 로직이 계속 진행될 수 있습니다.
    [SerializeField] private bool _isBlocking = true;

    // 외부에서 컷신 ID를 읽을 수 있도록 제공하는 프로퍼티입니다.
    public string CutsceneId => _cutsceneId;

    // 외부에서 이 컷신이 blocking 타입인지 확인할 수 있도록 제공하는 프로퍼티입니다.
    public bool IsBlocking => _isBlocking;

    // 이벤트 전체에 걸쳐 유지되어야 하는 배경/스테이지용 플레이어인지 여부입니다.
    // 기본값은 false이며, 필요하면 파생 클래스에서 true로 오버라이드합니다.
    //
    // 예를 들어:
    // - 이벤트 공통 배경 연출
    // - 스텝이 바뀌어도 계속 살아 있어야 하는 무대 연출
    //
    // 이런 경우 true를 반환하면, 브리지(관리 로직) 쪽에서
    // 스텝별 컷신이 위에 덮여 재생되더라도 이 플레이어는 유지할 수 있습니다.
    public virtual bool PersistsForEvent => false;

    // 현재 컷신이 실제로 재생 중인지 나타냅니다.
    // 파생 클래스에서 현재 상태를 기준으로 구현해야 합니다.
    public abstract bool IsPlaying { get; }

    // 주어진 요청(request)을 이 플레이어가 처리할 수 있는지 검사합니다.
    // 기본 구현은 항상 true를 반환하므로,
    // 필요하다면 파생 클래스에서 요청 조건을 보고 재생 가능 여부를 제한하면 됩니다.
    //
    // 예:
    // - 특정 컷신 ID만 허용
    // - 특정 상태일 때만 재생 가능
    // - 필요한 리소스가 준비된 경우에만 재생 가능
    public virtual bool CanPlay(WeekFlowCutsceneRequest request)
    {
        return true;
    }

    // 컷신 재생을 시작하는 코루틴입니다.
    // 실제 재생 로직은 파생 클래스에서 구현합니다.
    //
    // request에는 컷신 재생에 필요한 정보가 담겨 있다고 볼 수 있습니다.
    // IEnumerator를 사용하므로, 재생 과정 전체를 코루틴으로 제어할 수 있습니다.
    public abstract IEnumerator Play(WeekFlowCutsceneRequest request);

    // 현재 재생 중인 컷신을 "스킵 시도"합니다.
    // 스킵에 성공하면 true, 스킵할 수 없으면 false를 반환하도록 구현하는 것이 일반적입니다.
    //
    // 예:
    // - 사용자가 스킵 버튼을 눌렀을 때 호출
    // - 스킵 가능한 연출이면 즉시 종료 처리
    // - 스킵 불가능한 연출이면 false 반환
    public abstract bool TrySkip();

    // 현재 컷신을 즉시 중단합니다.
    // 일반적인 종료 연출이나 후처리 없이 바로 멈춰야 할 때 사용됩니다.
    //
    // 예:
    // - 씬 전환 직전 강제 정리
    // - 예외 상황에서 즉시 연출 종료
    public abstract void StopImmediate();
}
