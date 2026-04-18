using System.Collections;
using UnityEngine;

namespace GameParticle
{
    /// <summary>
    /// 파티클 시스템을 제어하는 컴포넌트입니다.
    /// 파티클 오브젝트에 직접 붙여서 사용하며,
    /// 재생 / 정지 / 위치 지정 / 지연 재생 / 반복 기능을 제공합니다.
    ///
    /// [사용 방법]
    /// 1. 파티클 시스템 오브젝트에 이 스크립트를 Add Component 합니다.
    /// 2. Target Particle을 비워두면 같은 오브젝트의 파티클을 자동으로 찾습니다.
    /// 3. Spawn Point에 빈 오브젝트를 드래그해서 파티클 발생 위치를 지정합니다.
    /// 4. 다른 스크립트에서 Play() / Stop()을 호출하거나
    ///    Inspector의 Play On Awake를 체크하면 시작 시 자동 재생됩니다.
    ///
    /// [자연스러운 재생을 위한 권장 설정]
    /// Start Lifetime >= Duration >= Particle System의 Duration
    /// 이 순서를 지켜야 파티클이 끝까지 올라가다 자연스럽게 사라집니다.
    /// </summary>
    public class ParticleController : MonoBehaviour
    {
        // ───────────────────────────────────────────────────────────
        // Inspector 설정 항목
        // ───────────────────────────────────────────────────────────

        [Header("Particle Reference")]
        [Tooltip("제어할 파티클 시스템을 여기에 드래그하세요.\n" +
                 "비워두면 이 오브젝트에 붙은 파티클 시스템을 자동으로 찾아 적용합니다.")]
        [SerializeField] private ParticleSystem targetParticle;

        [Header("Spawn")]
        [Tooltip("파티클이 생성될 위치 기준이 되는 오브젝트입니다.\n" +
                 "빈 오브젝트를 만들어서 원하는 위치에 배치한 뒤 여기에 드래그하세요.\n" +
                 "비워두면 이 오브젝트의 현재 위치에서 재생됩니다.")]
        [SerializeField] private Transform spawnPoint;

        [Header("Playback")]
        [Tooltip("true로 설정하면 게임 시작 시 자동으로 재생됩니다.")]
        [SerializeField] private bool playOnAwake = false;

        [Tooltip("Play() 호출 후 실제 재생까지 기다리는 시간(초)입니다.\n" +
                 "예: 이벤트 연출이 끝난 뒤 파티클을 띄우고 싶을 때 사용합니다.")]
        [Min(0f)]
        [SerializeField] private float delayBeforePlay = 0f;

        [Tooltip("파티클이 1회 뿜어져 나오는 시간(초)입니다.\n" +
                 "파티클 시스템 Inspector의 Duration 값과 맞춰서 설정하세요.\n" +
                 "이 값보다 Start Lifetime이 짧으면 파티클이 일찍 사라집니다.")]
        [Min(0f)]
        [SerializeField] private float duration = 2f;

        [Tooltip("파티클 하나가 화면에 존재하는 시간(초)입니다.\n" +
                 "이 값이 짧으면 파티클이 목적지까지 도달하기 전에 사라집니다.\n" +
                 "Duration보다 크거나 같게 설정하는 것을 권장합니다.")]
        [Min(0f)]
        [SerializeField] private float startLifetime = 3f;


        [Header("Repeat")]
        [Tooltip("true로 설정하면 아래 Repeat Count만큼 반복 재생합니다.\n" +
                 "기본값은 꺼져 있으며, 1회만 재생됩니다.")]
        [SerializeField] private bool useRepeat = false;

        [Tooltip("반복 횟수입니다. Use Repeat이 켜져 있을 때만 적용됩니다.\n" +
                 "0으로 설정하면 Stop()을 호출할 때까지 무한 반복합니다.")]
        [Min(0)]
        [SerializeField] private int repeatCount = 3;

        // ───────────────────────────────────────────────────────────
        // 내부 상태
        // ───────────────────────────────────────────────────────────

        // 현재 실행 중인 재생 코루틴을 추적합니다.
        // 새로 Play()가 호출되면 이전 코루틴을 중단하고 새로 시작합니다.
        private Coroutine _playCoroutine;

        // ───────────────────────────────────────────────────────────
        // Unity 생명주기
        // ───────────────────────────────────────────────────────────

        private void Awake()
        {
            // Target Particle이 비어 있으면 같은 오브젝트의 파티클을 자동으로 찾습니다.
            if (targetParticle == null)
                targetParticle = GetComponent<ParticleSystem>();

            // 파티클을 찾지 못했으면 경고 로그를 출력합니다.
            if (targetParticle == null)
            {
                Debug.LogWarning($"[ParticleController] ParticleSystem을 찾을 수 없습니다. " +
                                 $"Target Particle을 직접 할당하거나 같은 오브젝트에 ParticleSystem을 추가하세요.", this);
                return;
            }

            if (playOnAwake)
                Play();
        }

        // ───────────────────────────────────────────────────────────
        // 공개 메서드 (외부에서 호출)
        // ───────────────────────────────────────────────────────────

        /// <summary>
        /// 파티클을 재생합니다.
        /// 이미 재생 중이라면 처음부터 다시 재생합니다.
        /// </summary>
        public void Play()
        {
            if (targetParticle == null) return;

            // 이미 재생 중인 코루틴이 있으면 중단 후 새로 시작합니다.
            if (_playCoroutine != null)
                StopCoroutine(_playCoroutine);

            _playCoroutine = StartCoroutine(PlayRoutine());
        }

        /// <summary>
        /// 파티클을 즉시 정지하고 화면에서 제거합니다.
        /// </summary>
        public void Stop()
        {
            if (_playCoroutine != null)
            {
                StopCoroutine(_playCoroutine);
                _playCoroutine = null;
            }

            targetParticle?.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        /// <summary>
        /// 파티클 발생 위치를 런타임에서 변경합니다.
        /// 캐릭터 위치가 동적으로 바뀔 때 사용하세요.
        /// </summary>
        /// <param name="point">새로운 기준 Transform</param>
        public void SetSpawnPoint(Transform point) => spawnPoint = point;

        // ───────────────────────────────────────────────────────────
        // 내부 재생 로직
        // ───────────────────────────────────────────────────────────

        /// <summary>
        /// 실제 재생 흐름을 처리하는 코루틴입니다.
        /// Delay → 위치 이동 → 설정 적용 → 반복 재생 순서로 동작합니다.
        /// </summary>
        private IEnumerator PlayRoutine()
        {
            // 1. 설정된 시간만큼 대기합니다.
            if (delayBeforePlay > 0f)
                yield return new WaitForSeconds(delayBeforePlay);

            // 2. Spawn Point가 있으면 해당 위치로, 없으면 현재 위치 그대로 사용합니다.
            if (spawnPoint != null)
                transform.position = spawnPoint.position;

            // 3. 스크립트 값을 파티클 시스템에 적용합니다.
            //    스크립트 값이 파티클 시스템 Inspector 설정을 덮어씁니다.
            var main = targetParticle.main;
            main.startLifetime = startLifetime;
            main.startDelay = delayBeforePlay;


            // 4. 반복 횟수를 결정합니다.
            //    Use Repeat이 꺼져 있으면 무조건 1회만 재생합니다.
            //    Use Repeat이 켜져 있고 Repeat Count가 0이면 무한 반복합니다.
            int totalCount = useRepeat ? repeatCount : 1;
            int playedCount = 0;

            while (totalCount == 0 || playedCount < totalCount)
            {
                // 이전 재생이 남아 있으면 초기화 후 재생합니다.
                targetParticle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                targetParticle.Play();

                yield return new WaitForSeconds(duration);

                playedCount++;
            }

            // 5. 모든 재생이 끝나면 파티클을 정리합니다.
            targetParticle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            _playCoroutine = null;
        }
    }
}