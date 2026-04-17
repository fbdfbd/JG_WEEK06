# CSV 작성 매뉴얼

이 문서는 `Assets/Data/CSV` 폴더의 CSV를 작성할 때 참고하는 인수인계용 매뉴얼이다.
이번 버전은 `화자 SO + 대사 라인 배열` 구조를 기준으로 정리되어 있다.

## 1. 전체 구조 한눈에 보기

현재 CSV 연결 구조는 아래 흐름으로 보면 된다.

```text
flags.csv
  -> interactions.csv
  -> event_flag_conditions.csv

card_types.csv
  -> cards.csv
  -> events.csv (routine 전용 related_information_type_ids)
  -> event_information_conditions.csv

speakers.csv
  -> event_step_dialogue_lines.csv
  -> event_choice_dialogue_lines.csv

interactions.csv
  -> card_options.csv
  -> weeks.csv
  -> events.csv
  -> event_steps.csv
  -> event_choices.csv

cards.csv
  -> card_options.csv
  -> week_cards.csv

weeks.csv
  -> week_cards.csv
  -> events.csv

events.csv
  -> event_flag_conditions.csv
  -> event_stat_conditions.csv
  -> event_information_conditions.csv
  -> event_steps.csv

event_steps.csv
  -> event_step_dialogue_lines.csv
  -> event_choices.csv
  -> event_steps.csv (default_next_step_id)

event_choices.csv
  -> event_choice_dialogue_lines.csv
  -> event_steps.csv (next_step_id)
```

핵심 흐름은 이렇다.

```text
1. 먼저 플래그 / 카드 타입 / 화자 / 인터랙션 / 카드를 정의한다.
2. 그 다음 주차 루트와 주차 카드 목록을 정의한다.
3. 마지막으로 이벤트, 조건, 스텝, 선택지, 대사 라인을 정의한다.
```

## 2. 공통 작성 규칙

```text
- 첫 줄은 반드시 헤더다.
- CSV는 쉼표 구분이 기본이다.
- 여러 개 값을 한 칸에 넣을 때는 | 로 연결한다.
  예: trust_plus_1|reaction_log_night_promise
- 비어 있는 칸은 "없음", "미사용", "조건 없음" 의미다.
- true / false 는 소문자로 통일한다.
- 순서 컬럼은 0부터 시작하는 정수를 권장한다.
- ID는 사람이 읽기 쉬운 고정 문자열로 작성한다.
- 가급적 lower_snake_case 형식을 쓴다.
- 참조하는 ID는 반드시 다른 CSV에 실제로 존재해야 한다.
```

자주 쓰는 enum/고정값:

```text
semantic
- Direct
- Modified
- Blocked

event_kind
- routine
- story
- private_dialogue

visual_state
- Neutral
- Curious
- Anxious
- Trusting
- Obedient
- Conflicted

interaction kind
- stat_delta
- set_flag
- group
- conditional_stat_delta
- add_reaction_log
```

다중값 입력 예시:

```csv
interaction_ids
trust_plus_1|reaction_log_night_promise

related_information_type_ids
card_type_externalinfo|card_type_letter

preferred_semantics
Direct|Modified
```

## 3. 파일별 작성법

## `flags.csv`

용도:

```text
- 플래그 SO 정의용
- 조건 검사, 엔딩 분기, SetFlag Interaction의 원본 데이터
```

헤더:

```csv
flag_id,display_name,description
```

예시:

```csv
flag_id,display_name,description
letter_suspected,편지 의심,전달되지 않은 편지나 숨긴 편지를 네모가 의심하는 상태
external_interest,바깥 관심,네모가 집 밖의 정보와 세계에 지속적으로 관심을 보이는 상태
```

주의:

```text
- 문자열 ID로 직접 비교될 수 있으니 한번 정한 flag_id는 쉽게 바꾸지 않는다.
```

## `card_types.csv`

용도:

```text
- 카드 타입 SO 정의용
- 카드 분류 기준
- 일부 런타임 피드백 분기의 기준
```

헤더:

```csv
card_type_id,display_name
```

예시:

```csv
card_type_id,display_name
card_type_letter,편지
card_type_externalinfo,외부 정보
card_type_houseTalk,집 안의 대화
```

주의:

```text
- 기존 card_type_id는 런타임 분기에서 직접 참조될 수 있으므로 멋대로 바꾸지 않는다.
```

## `speakers.csv`

용도:

```text
- 화자 SO 정의용
- step/choice 대사 라인의 speaker 참조 원본
```

헤더:

```csv
speaker_id,display_name,default_visual_state
```

컬럼 설명:

```text
- speaker_id
  화자 고유 ID

- display_name
  화면에 표시할 이름

- default_visual_state
  기본 표정 상태
  현재는 주로 표시/확장 대비용으로 본다
```

예시:

```csv
speaker_id,display_name,default_visual_state
speaker_nemo,네모,Neutral
speaker_maid,하인,Neutral
speaker_parent,어머니,Trusting
```

## `interactions.csv`

용도:

```text
- 카드 선택
- 이벤트 완료
- step 진입
- choice 선택

이 모든 곳에서 공통으로 쓰는 효과 정의
```

헤더:

```csv
interaction_id,kind,display_name,stat_type,amount,flag_id,condition_stat,min_value,target_stat,reaction_text,child_interaction_ids
```

종류별 사용법:

```text
1. stat_delta
- stat_type, amount 사용

2. set_flag
- flag_id 사용

3. group
- child_interaction_ids 사용

4. conditional_stat_delta
- condition_stat, min_value, target_stat, amount 사용

5. add_reaction_log
- reaction_text 사용
```

예시:

```csv
interaction_id,kind,display_name,stat_type,amount,flag_id,condition_stat,min_value,target_stat,reaction_text,child_interaction_ids
trust_plus_1,stat_delta,신뢰 +1,Trust,1,,,,,,
letter_suspected_flag,set_flag,편지 의심 플래그,,,letter_suspected,,,,,
outside_interest_bundle,group,바깥 관심 묶음,,,,,,,,curiosity_plus_1|external_interest_flag
```

## `cards.csv`

헤더:

```csv
card_id,card_type_id,title,original_text
```

예시:

```csv
card_id,card_type_id,title,original_text
card_001_parentsletter,card_type_letter,부모의 편지,부모가 보내온 편지를 네모에게 어떻게 전달할지 정리한다.
```

## `card_options.csv`

헤더:

```csv
card_id,option_order,semantic,label,presented_text,interaction_ids
```

예시:

```csv
card_id,option_order,semantic,label,presented_text,interaction_ids
card_001_parentsletter,0,Direct,그대로 전달,편지를 거의 숨김 없이 그대로 읽어 준다.,trust_plus_1|reaction_log_letter_open
card_001_parentsletter,1,Modified,완곡하게 전달,걱정스러운 문장을 덜어내고 정리해서 전한다.,curiosity_plus_1
card_001_parentsletter,2,Blocked,보류한다,편지는 잠시 감춰 두고 오늘은 전하지 않는다.,anxiety_plus_1|letter_suspected_flag
```

주의:

```text
- 같은 card_id 안에서 option_order 중복 금지
- importer는 card_id 기준으로 옵션 배열 전체를 다시 만든다고 생각하면 된다
```

## `weeks.csv`

헤더:

```csv
week_id,week_index,title,summary,preturn_title,preturn_summary,on_week_start_interaction_ids,on_week_end_interaction_ids
```

예시:

```csv
week_id,week_index,title,summary,preturn_title,preturn_summary,on_week_start_interaction_ids,on_week_end_interaction_ids
week_001,1,첫 주의 정리,첫 주에 들어온 정보들을 네모에게 어떤 방식으로 전달할지 정리하고 그 결과로 열리는 이벤트를 본다.,사전 정보 정리,이번 주의 정보를 먼저 전달하고 그 선택 결과에 따라 낮과 밤 이벤트가 열린다.,,week_001_wrapup_log
```

## `week_cards.csv`

헤더:

```csv
week_id,display_order,card_id,is_required
```

예시:

```csv
week_id,display_order,card_id,is_required
week_001,0,card_001_parentsletter,true
week_001,1,card_002_courtyardrumor,true
```

## `events.csv`

헤더:

```csv
event_id,week_id,event_kind,event_order,title,priority,first_step_id,on_completed_interaction_ids,related_information_type_ids,preferred_semantics
```

예시:

```csv
event_id,week_id,event_kind,event_order,title,priority,first_step_id,on_completed_interaction_ids,related_information_type_ids,preferred_semantics
routine_outside_open,week_001,routine,1,바깥 정보 루틴 이벤트,15,step_outside_open_01,,card_type_externalinfo,Direct|Modified
story_letter_suspected,week_001,story,0,편지 의심 스토리 이벤트,30,step_story_letter_01,hidden_info_detected_flag,,
night_outside_interest,week_001,private_dialogue,0,바깥에 대한 밤 대화,50,step_night_outside_01,reaction_log_night_promise,,
```

주의:

```text
- routine이 아니면 related_information_type_ids, preferred_semantics는 비워둔다
- first_step_id는 반드시 event_steps.csv에 있어야 한다
```

## `event_flag_conditions.csv`

헤더:

```csv
event_id,mode,flag_id
```

예시:

```csv
event_id,mode,flag_id
story_letter_suspected,required,letter_suspected
story_letter_suspected,blocked,visitor_remembered
```

## `event_stat_conditions.csv`

헤더:

```csv
event_id,stat_type,use_minimum,minimum_value,use_maximum,maximum_value
```

예시:

```csv
event_id,stat_type,use_minimum,minimum_value,use_maximum,maximum_value
story_letter_suspected,Curiosity,true,2,false,0
night_outside_interest,Trust,true,1,true,4
```

## `event_information_conditions.csv`

헤더:

```csv
event_id,information_type_id,use_semantic_filter,semantic,minimum_count
```

예시:

```csv
event_id,information_type_id,use_semantic_filter,semantic,minimum_count
story_letter_suspected,card_type_letter,true,Blocked,1
night_outside_interest,card_type_externalinfo,false,,1
```

## `event_steps.csv`

용도:

```text
- 이벤트 진행 단계(step) 정의
- 장면 설명, 표정, 진입 효과, 기본 다음 step을 관리
- 실제 대사는 여기 적지 않고 event_step_dialogue_lines.csv 로 분리한다
```

헤더:

```csv
event_id,step_id,title_override,body_text,use_custom_visual_state,visual_state,on_enter_interaction_ids,default_next_step_id
```

예시:

```csv
event_id,step_id,title_override,body_text,use_custom_visual_state,visual_state,on_enter_interaction_ids,default_next_step_id
story_letter_suspected,step_story_letter_01,수상한 질문,네모가 편지 이야기를 다시 묻는다.,true,Conflicted,hidden_info_detected_flag,step_story_letter_02
story_letter_suspected,step_story_letter_02,침묵 이후,짧은 침묵 뒤에도 네모의 시선은 거두어지지 않는다.,false,Neutral,,
```

## `event_step_dialogue_lines.csv`

용도:

```text
- step 안에서 실제로 출력될 대사를 line 단위로 정의
- 누가 말하는지와 순서를 같이 적는 핵심 파일
```

헤더:

```csv
event_id,step_id,line_order,speaker_id,text
```

컬럼 설명:

```text
- event_id
  소속 이벤트

- step_id
  소속 step

- line_order
  대사 순서

- speaker_id
  speakers.csv 참조

- text
  실제 대사 내용
```

예시:

```csv
event_id,step_id,line_order,speaker_id,text
story_letter_suspected,step_story_letter_01,0,speaker_nemo,그 편지 말이야. 왜 조금 이상했어.
story_letter_suspected,step_story_letter_01,1,speaker_maid,오늘은 전부 말씀드리기 어려웠습니다.
```

주의:

```text
- 같은 event_id + step_id 안에서 line_order 중복 금지
- line_order만 봐도 대사 흐름이 읽혀야 한다
```

## `event_choices.csv`

용도:

```text
- 각 step에 붙는 선택지 배열 정의
- 실제 선택 결과 대사는 여기 적지 않고 event_choice_dialogue_lines.csv 로 분리한다
```

헤더:

```csv
event_id,step_id,choice_order,choice_id,label,interaction_ids,next_step_id
```

예시:

```csv
event_id,step_id,choice_order,choice_id,label,interaction_ids,next_step_id
story_letter_suspected,step_story_letter_01,0,choice_story_admit,조금은 숨겼다고 인정한다,curiosity_plus_1|hidden_info_detected_flag,step_story_letter_02
story_letter_suspected,step_story_letter_01,1,choice_story_deny,아무 일 아니라고 넘긴다,anxiety_plus_1,
```

주의:

```text
- 같은 step_id 안에서 choice_order 중복 금지
- importer는 step_id 기준으로 choice 배열 전체를 재구성하는 편이 안전하다
```

## `event_choice_dialogue_lines.csv`

용도:

```text
- choice 선택 직후 출력할 대사를 line 단위로 정의
- 여러 화자가 한 선택 결과에서 이어서 말할 수 있다
```

헤더:

```csv
event_id,step_id,choice_id,line_order,speaker_id,text
```

예시:

```csv
event_id,step_id,choice_id,line_order,speaker_id,text
story_letter_suspected,step_story_letter_01,choice_story_admit,0,speaker_maid,네... 다 말씀드리진 못했습니다.
story_letter_suspected,step_story_letter_01,choice_story_admit,1,speaker_nemo,그래도 지금은 들을래.
```

주의:

```text
- 같은 event_id + step_id + choice_id 안에서 line_order 중복 금지
- next_step_id와 별개로, 이 파일은 "선택 직후 보여줄 대사"만 정의한다
```

## 4. 추천 작업 순서

```text
1. flags.csv
2. card_types.csv
3. speakers.csv
4. interactions.csv
5. cards.csv
6. card_options.csv
7. weeks.csv
8. week_cards.csv
9. events.csv
10. event_flag_conditions.csv
11. event_stat_conditions.csv
12. event_information_conditions.csv
13. event_steps.csv
14. event_step_dialogue_lines.csv
15. event_choices.csv
16. event_choice_dialogue_lines.csv
```

## 5. 자주 하는 실수

```text
1. ID 오타
- 가장 흔한 문제
- 참조하는 ID는 반드시 다른 CSV에 실제 존재해야 한다

2. 여러 값 구분 실수
- 쉼표가 아니라 | 를 써야 한다

3. routine 전용 컬럼을 story에 넣는 실수
- related_information_type_ids, preferred_semantics는 routine 이벤트에서만 사용

4. choice_order / option_order / display_order / line_order 중복
- 배열형 데이터는 순서 중복이 나면 결과가 꼬인다

5. step 연결 누락
- first_step_id, default_next_step_id, next_step_id가 실제 존재하는 step인지 확인

6. 대사를 잘못된 파일에 적는 실수
- step 본문 설명은 event_steps.csv
- 실제 step 대사는 event_step_dialogue_lines.csv
- choice 결과 대사는 event_choice_dialogue_lines.csv
```

## 6. 빠른 체크리스트

```text
[ ] flag_id 오타 없음
[ ] card_type_id 오타 없음
[ ] speaker_id 오타 없음
[ ] interaction_id 오타 없음
[ ] week_id 오타 없음
[ ] event_id 오타 없음
[ ] step_id 연결 확인 완료
[ ] option_order 중복 없음
[ ] display_order 중복 없음
[ ] choice_order 중복 없음
[ ] line_order 중복 없음
[ ] 여러 값 입력 칸은 | 사용
[ ] routine 아닌 이벤트의 related_information_type_ids / preferred_semantics 는 비움
```

## 7. 마지막 요약

```text
- flags.csv: 플래그 사전
- card_types.csv: 카드 타입 사전
- speakers.csv: 화자 사전
- interactions.csv: 재사용 효과 사전
- cards.csv: 카드 본체
- card_options.csv: 카드 선택지
- weeks.csv: 주차 루트 + preturn 기본 정보
- week_cards.csv: 주차에 배치되는 카드 목록
- events.csv: 이벤트 본체
- event_flag_conditions.csv: 플래그 조건
- event_stat_conditions.csv: 스탯 조건
- event_information_conditions.csv: 정보 카드 조건
- event_steps.csv: 이벤트 단계 정보
- event_step_dialogue_lines.csv: step 대사 라인
- event_choices.csv: 단계별 선택지
- event_choice_dialogue_lines.csv: choice 결과 대사 라인
```

이 문서만 보고 작업할 때의 감각은 이렇게 잡으면 된다.

```text
카드 파트는 "무슨 정보를 어떻게 전달하느냐"
이벤트 파트는 "그 전달 결과로 어떤 장면이 열리느냐"
대사 라인 파트는 "그 장면에서 누가 어떤 순서로 말하느냐"
```
