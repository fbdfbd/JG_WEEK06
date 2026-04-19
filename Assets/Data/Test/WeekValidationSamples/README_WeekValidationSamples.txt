5주 검증용 샘플 SO 생성 완료

생성 위치: Assets/Data/Test/WeekValidationSamples

권장 연결 방식
1. WeekFlowController._weekDefinition 에 1주차 asset 연결
2. WeekFlowController._weekDefinitions 에 1~5주차 asset 전부 연결

기본 선택 안내
- 모든 카드는 기본 선택(첫 번째 옵션)만으로도 검증 포인트가 보이게 구성됨
- W1: 밤 대화에서 Choice A / B 둘 다 비교 권장
- W4: 멀티스텝 스토리에서 Choice A / B 둘 다 비교 권장
- W5: 기본 선택 그대로 두면 Anxiety ending 비교가 쉬움

주차 목록
- W1: [TEST WEEK 1] Semantic + RequiredFlag + ChoiceResult
- W2: [TEST WEEK 2] InformationType + SemanticCondition
- W3: [TEST WEEK 3] WeekStartGroup + StatCondition + EventComplete
- W4: [TEST WEEK 4] MultiStep + ChoiceBranch + BlockedFallback
- W5: [TEST WEEK 5] Final Week + Ending
