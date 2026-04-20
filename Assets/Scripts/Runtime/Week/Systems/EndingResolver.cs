using System.Collections.Generic;

public readonly struct EndingPresentation
{
    public EndingPresentation(
        string endingId,
        string title,
        IReadOnlyList<string> detailLines,
        string summary,
        string closingLine,
        string reputationLine,
        ENemoVisualState visualState)
    {
        EndingId = endingId;
        Title = title;
        DetailLines = detailLines;
        Summary = summary;
        ClosingLine = closingLine;
        ReputationLine = reputationLine;
        VisualState = visualState;
    }

    public string EndingId { get; }
    public string Title { get; }
    public IReadOnlyList<string> DetailLines { get; }
    public string Summary { get; }
    public string ClosingLine { get; }
    public string ReputationLine { get; }
    public ENemoVisualState VisualState { get; }
}

public static class EndingResolver
{
    private const int HighAffinityThreshold = 5;

    public static EndingPresentation Resolve(RuntimeChildState childState)
    {
        int trust = childState.GetStat(EChildStatusType.Trust);
        int curiosity = childState.GetStat(EChildStatusType.Curiosity);
        int anxiety = childState.GetStat(EChildStatusType.Anxiety);
        int obedience = childState.GetStat(EChildStatusType.Obedience);
        int affinity = childState.GetStat(EChildStatusType.Affinity);

        bool hasHighAffinity = affinity >= HighAffinityThreshold;
        EChildStatusType dominantStat = ResolveDominantStat(trust, curiosity, anxiety, obedience);

        return dominantStat switch
        {
            EChildStatusType.Curiosity => hasHighAffinity
                ? CreateCuriosityHighAffinityEnding()
                : CreateCuriosityLowAffinityEnding(),

            EChildStatusType.Trust => hasHighAffinity
                ? CreateTrustHighAffinityEnding()
                : CreateTrustLowAffinityEnding(),

            EChildStatusType.Obedience => hasHighAffinity
                ? CreateObedienceHighAffinityEnding()
                : CreateObedienceLowAffinityEnding(),

            EChildStatusType.Anxiety => hasHighAffinity
                ? CreateAnxietyHighAffinityEnding()
                : CreateAnxietyLowAffinityEnding(),

            _ => hasHighAffinity
                ? CreateTrustHighAffinityEnding()
                : CreateTrustLowAffinityEnding(),
        };
    }

    private static EChildStatusType ResolveDominantStat(int trust, int curiosity, int anxiety, int obedience)
    {
        // 동률일 경우 우선순위:
        // Trust > Curiosity > Anxiety > Obedience
        int maxValue = trust;
        EChildStatusType dominantStat = EChildStatusType.Trust;

        if (curiosity > maxValue)
        {
            maxValue = curiosity;
            dominantStat = EChildStatusType.Curiosity;
        }

        if (anxiety > maxValue)
        {
            maxValue = anxiety;
            dominantStat = EChildStatusType.Anxiety;
        }

        if (obedience > maxValue)
        {
            dominantStat = EChildStatusType.Obedience;
        }

        return dominantStat;
    }

    private static EndingPresentation CreateCuriosityHighAffinityEnding()
    {
        return new EndingPresentation(
            "ending_curiosity_high_affinity",
            "ENDING · 다시 돌아올 여행",
            new[]
            {
                "마침내 네모는 백작이 자신에게 숨겨왔던 모든 진실을 알게 되었다.",
                "하지만 네모는 조금도 배신감을 느끼거나 흔들리지 않았다.",
                "자신이 품고 있는 이 세상을 향한 반짝이는 호기심이 어디서 시작되었는지 잘 알고 있었으니까.",
                "지금 네모는 콧노래를 부르며 가벼운 여행 가방을 싸고 있다.",
                "이 저택을 영영 떠나기 위해서가 아니다.",
                "더 넓은 세상을 한가득 눈에 담고, 다시 백작의 곁으로 돌아오기 위해서다.",
            },
            "진실을 마주한 네모는 무너지지 않았다. 오히려 세상을 향한 호기심과 백작을 향한 애정을 함께 품은 채, 더 넓은 세계를 향해 첫발을 내딛는다.",
            "다 보고 나서, 다시 돌아올게.",
            "후견인 평판: 아이의 날개를 꺾지 않고 세상을 향해 보낼 줄 아는 보호자로 기억되었다.",
            ENemoVisualState.Curious);
    }

    private static EndingPresentation CreateCuriosityLowAffinityEnding()
    {
        return new EndingPresentation(
            "ending_curiosity_low_affinity",
            "ENDING · 저택을 등진 아이",
            new[]
            {
                "마침내 네모는 백작이 자신을 어떻게 통제해 왔는지 모든 진실을 마주하게 되었다.",
                "충격보다는 오히려 묵은 체증이 내려가듯 후련한 마음이 앞섰다.",
                "새장 같던 저택을 벗어나, 늘 저 너머의 낯선 세상을 꿈꿔왔기 때문이다.",
                "네모는 미련 없이 가벼운 발걸음으로 짐을 꾸리기 시작했다.",
                "이제 진짜 세상으로 나아갈 시간이다.",
                "수많은 비밀과 억압을 품었던 이 저택은 영영 등진 채로.",
            },
            "네모는 마침내 저택의 진실을 꿰뚫어 보았고, 미련 없이 바깥세상으로 떠난다. 그 호기심은 이제 관계를 붙드는 힘이 아니라, 완전히 새로운 삶을 향한 추진력이 되었다.",
            "이제는 정말, 내 세상으로 갈 거야.",
            "후견인 평판: 아이를 끝내 붙잡아 두지 못한 통제적인 보호자로 남았다.",
            ENemoVisualState.Curious);
    }

    private static EndingPresentation CreateTrustHighAffinityEnding()
    {
        return new EndingPresentation(
            "ending_trust_high_affinity",
            "ENDING · 차기 가주",
            new[]
            {
                "요즘 네모의 아침은 눈코 뜰 새 없이 분주하게 시작된다.",
                "차기 가주로서 백작에게 인수인계받아야 할 영지의 업무가 산더미 같기 때문이다.",
                "그럼에도 네모의 발걸음에는 조금의 두려움이나 망설임도 없었다.",
                "가장 완벽한 스승인 백작이 늘 곁에서 훌륭한 가르침을 줄 것을 굳게 믿고 있으니까.",
                "쉴 틈 없이 쏟아지는 서류 작업 속에서도 네모의 입가에는 여유로운 미소가 떠나질 않는다.",
            },
            "네모는 백작에 대한 굳건한 신뢰와 깊은 친밀감을 바탕으로, 흔들림 없이 후계자의 길을 받아들인다. 저택의 미래는 이제 두 사람의 신뢰 위에 놓여 있다.",
            "백작님이 곁에 계신다면, 나는 잘 해낼 수 있어.",
            "후견인 평판: 아이에게 신뢰와 안정감을 심어준 훌륭한 보호자로 칭송받았다.",
            ENemoVisualState.Trusting);
    }

    private static EndingPresentation CreateTrustLowAffinityEnding()
    {
        return new EndingPresentation(
            "ending_trust_low_affinity",
            "ENDING · 완벽한 후계자",
            new[]
            {
                "무거운 눈꺼풀을 들어 올린 네모가 자신도 모르게 짧은 한숨을 내쉬었다.",
                "오늘도 한 치의 오차도 허용하지 않는 백작의 후계자 수업이 기다리고 있기 때문이다.",
                "네모는 백작을 진심으로 존경하지만, 늘 보이지 않는 거대한 압박감에 짓눌려 왔다.",
                "문득 창밖을 보던 네모는 이곳을 도망치고 싶다는 충동에 스스로 화들짝 놀라고 말았다.",
                "부족함 없이 완벽하고 명예로운 삶인데, 대체 왜 도망치고 싶은 걸까?",
                "스스로도 알 수 없는 모순된 감정에 네모는 조용히 고개를 저었다.",
            },
            "네모는 백작을 신뢰하고 그 길을 따르지만, 그 관계는 따뜻한 친밀감보다는 무거운 책임과 압박에 더 가깝다. 완벽한 삶처럼 보이지만, 마음속 균열은 조용히 남아 있다.",
            "나는 믿고 있어. 그런데 왜 이렇게 숨이 막힐까.",
            "후견인 평판: 훌륭한 후계자를 길러냈지만, 아이의 마음을 충분히 돌보지 못한 보호자로 비쳤다.",
            ENemoVisualState.Trusting);
    }

    private static EndingPresentation CreateObedienceHighAffinityEnding()
    {
        return new EndingPresentation(
            "ending_obedience_high_affinity",
            "ENDING · 고요한 안식",
            new[]
            {
                "오늘도 네모는 조용히 식사를 마치고 자신의 방으로 돌아왔다.",
                "안전하고 고요한, 매일 똑같이 반복되는 일상의 끝이었다.",
                "유일한 삶의 낙이라고는 가끔 찾아오는 백작과의 다정한 대화 시간뿐이다.",
                "사실 네모는 바깥세상의 무서운 소문과 마주할 기력을 잃은 지 오래였다.",
                "나를 둘러싼 저 판도라의 상자를 열어젖힐 용기도, 자신도 없었다.",
                "그저 이 험악한 세상에서 나를 지켜줄 사람은 오직 백작뿐이라고 굳게 믿기로 했다.",
            },
            "네모는 더 이상 진실을 파헤치거나 바깥을 꿈꾸지 않는다. 대신 백작과의 정서적 유대를 붙든 채, 안전하고 반복되는 일상 속에 자신을 조용히 눕힌다.",
            "여기 있으면 괜찮아. 백작님이 있으니까.",
            "후견인 평판: 아이에게 안식을 주었지만, 세상으로 나아갈 힘까지는 남겨주지 못한 보호자로 남았다.",
            ENemoVisualState.Obedient);
    }

    private static EndingPresentation CreateObedienceLowAffinityEnding()
    {
        return new EndingPresentation(
            "ending_obedience_low_affinity",
            "ENDING · 건조한 쳇바퀴",
            new[]
            {
                "책상에 쌓인 가문의 서류를 처리하다 보니 오늘도 훌쩍 자정을 넘겨 야근을 해버렸다.",
                "언제부턴가 네모는 이렇게 기계처럼 일에만 모든 신경을 몰두하고 있었다.",
                "숨 막히는 업무에 집중할 때만 간신히 살아있다는 감각을 느낄 수 있었기 때문이다.",
                "펜을 내려놓고 고요함이 찾아오면, 마음속이 텅 비어버린 듯한 지독한 공허함이 밀려왔다.",
                "개인적인 감정도, 삶의 여유도 남지 않은 삭막한 일상.",
                "네모는 그렇게 완벽하지만 건조한 쳇바퀴 속으로 스스로를 밀어 넣었다.",
            },
            "네모는 순응하는 법을 완벽히 익혔지만, 그 안에는 온기도 애정도 남아 있지 않았다. 남은 것은 그저 멈추지 않는 업무와 공허뿐이다.",
            "생각하지 않으면, 버틸 수 있어.",
            "후견인 평판: 아이를 효율적으로 길러냈지만, 결국 메마른 삶으로 몰아넣은 보호자로 평가되었다.",
            ENemoVisualState.Obedient);
    }

    private static EndingPresentation CreateAnxietyHighAffinityEnding()
    {
        return new EndingPresentation(
            "ending_anxiety_high_affinity",
            "ENDING · 그늘 아래의 안도",
            new[]
            {
                "네모는 손톱을 만지작거리며 초조함에 발을 동동 굴렀다.",
                "백작이 돌아오기로 한 예정된 시간을 훌쩍 넘겼는데도 저택이 고요했기 때문이다.",
                "바깥세상의 끔찍한 소문들이 머릿속을 헤집으며 불안감이 눈덩이처럼 커져만 갔다.",
                "그때, 창문 너머로 익숙한 백작의 마차가 저택 안으로 들어서는 것이 보였다.",
                "그제야 네모는 가슴을 쓸어내리며 깊은 안도의 숨을 내쉬었다.",
                "백작의 넓은 그늘 아래에 찰싹 붙어있을 때만, 비로소 세상이 안전하게 느껴졌다.",
            },
            "네모는 세상을 두려워하지만, 백작이라는 존재 안에서만은 안정을 찾는다. 불안은 사라지지 않았지만, 적어도 기대어 숨 쉴 수 있는 사람은 남아 있다.",
            "백작님이 돌아오시면, 이제 괜찮아.",
            "후견인 평판: 아이에게 의지처는 되어주었지만, 세상을 견딜 힘까지 길러주지는 못한 보호자로 남았다.",
            ENemoVisualState.Anxious);
    }

    private static EndingPresentation CreateAnxietyLowAffinityEnding()
    {
        return new EndingPresentation(
            "ending_anxiety_low_affinity",
            "ENDING · 닫힌 방",
            new[]
            {
                "네모는 방문을 아주 살짝 열고 복도에 아무도 없는지 숨을 죽여 살폈다.",
                "텅 빈 것을 확인하고서야, 빠르게 식당으로 내려가 남은 저녁거리를 챙겨 돌아왔다.",
                "서둘러 방으로 돌아와 무거운 자물쇠를 걸어 잠그고 나서야 굳었던 어깨가 조금 풀렸다.",
                "백작도, 하인들도, 이 저택의 그 누구도 더 이상 믿을 수 없었다.",
                "네모는 굳게 닫힌 방문처럼 세상 밖으로 나갈 마음의 문마저 영영 잠가버렸다.",
                "아무도 침범할 수 없는, 자신만의 서늘한 방구석에서 평화를 찾기로 한 것이다.",
            },
            "네모는 불안 끝에 세상과 관계 모두로부터 자신을 차단해 버린다. 남은 것은 누구도 들이지 않는 차가운 안전지대뿐이다.",
            "아무도 들어오지 마. 여기가 제일 안전하니까.",
            "후견인 평판: 아이를 끝내 세상과 단절시켜 버린 실패한 보호자로 기억되었다.",
            ENemoVisualState.Anxious);
    }
}
