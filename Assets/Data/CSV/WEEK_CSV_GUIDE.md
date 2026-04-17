# WEEK CSV 분리 가이드

이 문서는 `WEEK` 관련 CSV가 왜 여러 장으로 나뉘어 있는지 빠르게 이해하기 위한 설명서다.

## 한 줄 요약

```text
weeks.csv 는 "주차 뼈대"
week_cards.csv 는 "그 주차에 보여줄 정보 카드"
events.csv 는 "그 주차에서 열릴 이벤트 본체"
event_steps.csv 는 "이벤트 장면 단계"
event_step_dialogue_lines.csv / event_choice_dialogue_lines.csv 는 "실제 대사"
```

## 1. `weeks.csv`

역할:

```text
- 주차 루트 SO를 만든다
- 주차 제목, 요약, PreTurn 제목/설명, 시작/종료 훅을 넣는다
```

즉 이 파일은

```text
"이번 주가 무엇인지"
"사전 정보 정리 파트 이름이 무엇인지"
```

를 정의한다.

여기에는 카드 목록도 없고, 이벤트 본문도 없다.

## 2. `week_cards.csv`

역할:

```text
- 해당 주차 PreTurn 에 어떤 카드가 들어가는지 정의한다
- 표시 순서와 필수 여부를 관리한다
```

즉 이 파일은

```text
"1주차에 어떤 정보 카드들을 먼저 검토하느냐"
```

를 담당한다.

## 3. `events.csv`

역할:

```text
- 주차에서 열릴 routine / story / private_dialogue 이벤트의 본체를 정의한다
- 우선순위, 시작 step, 완료 후 인터랙션을 넣는다
```

즉 이 파일은

```text
"이 주차에 어떤 이벤트 후보들이 존재하느냐"
```

를 정의한다.

하지만 이 파일만으로는 이벤트 내용이 아직 없다.
이벤트 내용은 다음 파일들로 내려간다.

## 4. `event_flag_conditions.csv`
## 5. `event_stat_conditions.csv`
## 6. `event_information_conditions.csv`

역할:

```text
- 이벤트가 열리는 조건을 분리해서 적는다
```

이 3개 파일은 전부

```text
"이 이벤트가 언제 열리는가"
```

를 담당한다.

## 7. `event_steps.csv`

역할:

```text
- 이벤트 장면의 단계(step)를 정의한다
- 장면 설명, 표정, 진입 인터랙션, 기본 다음 step을 넣는다
```

즉 이 파일은

```text
"이 이벤트가 몇 단계 장면으로 흘러가느냐"
```

를 담당한다.

중요:

```text
여기에는 실제 대사를 넣지 않는다.
```

`body_text` 는 장면 설명용이다.

## 8. `event_step_dialogue_lines.csv`

역할:

```text
- 각 step 안에서 누가 어떤 순서로 말하는지 적는다
```

즉 이 파일은

```text
"장면 안에서 실제로 출력될 대사"
```

를 담당한다.

예:

```text
네모: 그 편지 말이야. 왜 조금 이상했어.
하인: 오늘은 전부 말씀드리기 어려웠습니다.
```

## 9. `event_choices.csv`

역할:

```text
- step 에 붙는 선택지 버튼 자체를 정의한다
- 선택 시 인터랙션과 다음 step 이동을 적는다
```

즉 이 파일은

```text
"플레이어가 무엇을 고를 수 있느냐"
```

를 담당한다.

## 10. `event_choice_dialogue_lines.csv`

역할:

```text
- 특정 choice 를 고른 직후 출력할 대사를 적는다
```

즉 이 파일은

```text
"선택 결과로 누가 뭐라고 말하느냐"
```

를 담당한다.

## 전체 흐름 예시

```text
1. weeks.csv
   1주차를 만든다

2. week_cards.csv
   1주차 PreTurn 에 카드 4장을 넣는다

3. events.csv
   1주차에 편지 의심 이벤트, 밤 대화 이벤트를 등록한다

4. event_*_conditions.csv
   어떤 조건이면 그 이벤트가 열리는지 적는다

5. event_steps.csv
   편지 의심 이벤트가 step 2개로 이루어진다고 적는다

6. event_step_dialogue_lines.csv
   step 1에서 네모/하인이 무슨 말을 하는지 적는다

7. event_choices.csv
   step 1에서 어떤 선택지를 고를 수 있는지 적는다

8. event_choice_dialogue_lines.csv
   선택지마다 선택 직후 나오는 대사를 적는다
```

## 왜 이렇게 나눴는가

이유는 간단하다.

```text
주차 뼈대
카드 배치
이벤트 본체
이벤트 조건
이벤트 단계
실제 대사
```

이 6가지가 서로 수정 주기가 다르기 때문이다.

예를 들어

```text
카드 배치만 바꾸고 싶으면 week_cards.csv 만 수정하면 되고,
대사만 바꾸고 싶으면 event_step_dialogue_lines.csv 만 수정하면 된다.
```

그래서 한 파일에 다 몰아넣는 것보다, 지금처럼 역할별로 나눠두는 쪽이 훨씬 읽기 쉽고 수정도 편하다.
