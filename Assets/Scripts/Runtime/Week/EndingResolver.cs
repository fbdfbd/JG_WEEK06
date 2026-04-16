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
    public static EndingPresentation Resolve(RuntimeChildState childState)
    {
        int trust = childState.GetStat(EChildStatusType.Trust);
        int curiosity = childState.GetStat(EChildStatusType.Curiosity);
        int anxiety = childState.GetStat(EChildStatusType.Anxiety);
        int obedience = childState.GetStat(EChildStatusType.Obedience);

        bool sensedHiddenTruth = childState.HasFlag(EChildFlagType.HiddenInfoDetected) || childState.HasFlag(EChildFlagType.LetterSuspected);
        bool interestedInOutside = childState.HasFlag(EChildFlagType.ExternalInterest);

        if (curiosity >= 4 && interestedInOutside && sensedHiddenTruth && trust <= 2)
        {
            return new EndingPresentation(
                "ending_search_parent",
                "ENDING · 세모를 찾아 나서는 아이",
                new[]
                {
                    "네모는 저택 밖을 향한 막연한 동경을 넘어, 이제는 누군가를 직접 찾아 나서야 한다는 목적을 갖게 되었다.",
                    "숨겨졌던 말들과 비어 있던 자리를 스스로 이어 붙인 끝에, 네모는 답이 집 밖에 있다고 판단했다.",
                    "당신이 다듬어 보여준 세계는 네모를 묶어 두기보다, 오히려 세모의 흔적을 따라가게 만들었다.",
                },
                "네모는 더 이상 이 집 안의 설명만으로 멈추지 않는다. 이제는 세모를 찾아 자신의 시작을 직접 확인하려 한다.",
                "나한테서 멀어지더라도, 진짜 답이 있는 쪽으로 가 볼 거야.",
                "후견인 평판: 아이를 지키려 했지만 끝내 진실을 붙잡아 두지 못한 보호자로 남았다.",
                ENemoVisualState.Curious);
        }

        if (anxiety >= 4)
        {
            return new EndingPresentation(
                "ending_anxious",
                "ENDING · 흔들리는 아이",
                new[]
                {
                    "네모는 무사히 자라났지만 설명되지 않은 공백을 오래 끌어안게 되었다.",
                    "막힌 정보와 끊긴 대화는 네모 안에 궁금함보다 먼저 긴장을 남겼다.",
                    "보호받고 있다는 감각과 불안은 끝내 완전히 분리되지 못했다.",
                },
                "네모는 이 집 안에서 살아가는 법은 배웠지만, 세상을 편안하게 믿는 법은 아직 배우지 못했다.",
                "괜찮다고 말해도 마음 한쪽은 아직 조용히 떨리고 있어.",
                "후견인 평판: 아이를 지나치게 조심시켜 결국 불안을 남긴 보호자로 기억되었다.",
                ENemoVisualState.Anxious);
        }

        if (curiosity >= 4 && interestedInOutside)
        {
            return new EndingPresentation(
                "ending_outward",
                "ENDING · 바깥을 향하는 아이",
                new[]
                {
                    "네모는 들은 이야기보다 들리지 않은 빈칸을 오래 바라보았다.",
                    "막힌 문과 멈춘 말은 오히려 바깥을 향한 마음을 더 선명하게 만들었다.",
                    "이제 네모는 누가 답을 주기보다, 스스로 확인하고 싶어 한다.",
                },
                "당신이 조정한 환경은 네모를 가두는 대신, 결국 바깥을 향한 질문을 더 또렷하게 만들었다.",
                "이제는 내가 직접 보고, 직접 물어보고 싶어.",
                "후견인 평판: 아이를 얌전히 키우는 데는 실패했지만, 스스로 움직일 용기를 준 보호자로도 비쳤다.",
                ENemoVisualState.Curious);
        }

        if (trust >= 4 && obedience >= 4)
        {
            return new EndingPresentation(
                "ending_trusting",
                "ENDING · 믿고 따르는 아이",
                new[]
                {
                    "네모는 이 집의 말과 질서를 자신만의 세계로 받아들였다.",
                    "들려준 이야기의 결을 그대로 기억하며, 설명받은 만큼 믿는 법을 먼저 익혔다.",
                    "조용하고 안정된 하루들이 네모에게는 가장 먼저 익숙한 모양이 되었다.",
                },
                "당신의 관계와 환경은 네모를 안정된 방향으로 이끌었고, 네모는 그 안에서 보호받는 법을 먼저 익혔다.",
                "아직은 여기 있는 말들이 내 세상의 모양이야.",
                "후견인 평판: 아이를 안정적으로 돌본 훌륭한 보호자라는 좋은 평판이 따라붙었다.",
                ENemoVisualState.Trusting);
        }

        if (sensedHiddenTruth || curiosity >= 3)
        {
            return new EndingPresentation(
                "ending_doubtful",
                "ENDING · 조용히 의심하는 아이",
                new[]
                {
                    "네모는 크게 반항하지 않았지만, 흘리지 않은 말의 자리를 기억했다.",
                    "순화된 설명과 비워진 문장은 네모 안에 작은 의심들을 남겼다.",
                    "겉으로는 조용하지만 이미 스스로 이어 붙인 조각들이 생겨났다.",
                },
                "네모는 아직 이 집을 떠나지 않았지만, 이제는 더 이상 말해지지 않은 부분을 잊지 않을 것이다.",
                "말해 주지 않은 것도, 언젠가는 다 이어질 것 같아.",
                "후견인 평판: 아이 앞에서 지나치게 많은 것을 숨긴, 믿기 어려운 어른이라는 평판이 남았다.",
                ENemoVisualState.Conflicted);
        }

        return new EndingPresentation(
            "ending_neutral",
            "ENDING · 고요한 유년",
            new[]
            {
                "네모의 하루는 조용했고, 큰 균열 없이 흘러갔다.",
                "무엇을 믿고 무엇을 의심해야 할지는 아직 분명하게 정해지지 않았다.",
                "당신의 관계와 환경은 네모 안에 잠시 머무는 고요 같은 시간을 남겼다.",
            },
            "지금의 네모는 아직 어느 방향으로 완전히 기울지 않았다. 하지만 그 고요 역시 언젠가 모양을 갖게 될 것이다.",
            "아직은 잘 모르겠어. 그래도 기억은 남아 있을 거야.",
            "후견인 평판: 큰 흉도 큰 찬사도 없는, 무난한 보호자라는 평판에 머물렀다.",
            ENemoVisualState.Neutral);
    }
}
