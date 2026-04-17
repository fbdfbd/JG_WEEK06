이 폴더는 현재 SO 구조 기준 샘플 CSV 세트입니다.

핵심 규칙
- 여러 참조값은 `|` 로 구분합니다. 예: `trust_plus_1|external_interest_flag`
- 빈 칸은 `없음` 또는 `미사용` 의미입니다.
- `card_options`, `week_cards`, `event_choices`, `event_step_dialogue_lines` 같은 반복 배열은 owner 기준 전체 재구성 전제로 봅니다.
- `event_kind` 값은 `routine`, `story`, `private_dialogue` 입니다.
- `semantic` 값은 `Direct`, `Modified`, `Blocked` 입니다.
- `visual_state` 값은 `Neutral`, `Curious`, `Anxious`, `Trusting`, `Obedient`, `Conflicted` 입니다.

이번 구조에서 추가된 포인트
- `speakers.csv` 로 화자 사전을 관리합니다.
- `event_steps.csv` 에서는 이제 `nemo_line` 대신 장면 설명만 적습니다.
- 실제 step 대사는 `event_step_dialogue_lines.csv` 에서 관리합니다.
- 실제 choice 결과 대사는 `event_choice_dialogue_lines.csv` 에서 관리합니다.

샘플 포인트
- `interactions.csv` 에서 `group`, `conditional_stat_delta`, `add_reaction_log` 예시를 볼 수 있습니다.
- `events.csv` 와 `event_*_conditions.csv` 에서 이벤트 조건을 여러 줄로 나누는 방식이 들어 있습니다.
- `event_step_dialogue_lines.csv` 와 `event_choice_dialogue_lines.csv` 에서 다화자 대사와 `line_order` 작성 방식을 확인할 수 있습니다.

추가 문서
- `CSV_MANUAL.md`: 전체 작성 매뉴얼
- `WEEK_CSV_GUIDE.md`: week 관련 CSV가 왜 나뉘었는지 설명
