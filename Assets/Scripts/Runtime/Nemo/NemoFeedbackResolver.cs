public enum ENemoVisualState
{
    Neutral,
    Curious,
    Anxious,
    Trusting,
    Obedient,
    Conflicted
}

public readonly struct NemoFeedbackPresentation
{
    public NemoFeedbackPresentation(string speakerName, ENemoVisualState visualState, string dialogueLine)
    {
        SpeakerName = speakerName;
        VisualState = visualState;
        DialogueLine = dialogueLine;
    }

    public NemoFeedbackPresentation(ENemoVisualState visualState, string dialogueLine)
        : this(NemoFeedbackResolver.DefaultSpeakerName, visualState, dialogueLine)
    {
    }

    public string SpeakerName { get; }
    public ENemoVisualState VisualState { get; }
    public string DialogueLine { get; }
}

public static class NemoFeedbackResolver
{
    public const string DefaultSpeakerName = "네모";

    public static NemoFeedbackPresentation Resolve(
        RuntimeChildState childState,
        RuntimeResolvedCardRecord resolvedCard)
    {
        ENemoVisualState visualState = ResolveVisualState(childState);
        string dialogueLine = ResolveDialogueLine(childState, resolvedCard);
        return new NemoFeedbackPresentation(DefaultSpeakerName, visualState, dialogueLine);
    }

    private static ENemoVisualState ResolveVisualState(RuntimeChildState childState)
    {
        int trust = childState.GetStat(EChildStatusType.Trust);
        int curiosity = childState.GetStat(EChildStatusType.Curiosity);
        int anxiety = childState.GetStat(EChildStatusType.Anxiety);
        int obedience = childState.GetStat(EChildStatusType.Obedience);

        if (curiosity >= 4 && anxiety >= 3)
        {
            return ENemoVisualState.Conflicted;
        }

        if (anxiety >= 4)
        {
            return ENemoVisualState.Anxious;
        }

        if (trust >= 4)
        {
            return ENemoVisualState.Trusting;
        }

        if (curiosity >= 4)
        {
            return ENemoVisualState.Curious;
        }

        if (obedience >= 4)
        {
            return ENemoVisualState.Obedient;
        }

        return ENemoVisualState.Neutral;
    }

    private static string ResolveDialogueLine(
        RuntimeChildState childState,
        RuntimeResolvedCardRecord resolvedCard)
    {
        if (resolvedCard?.CardDefinition?.CardType == null || resolvedCard.SelectedOption == null)
        {
            return ResolveFallbackLine(childState);
        }

        string typeId = resolvedCard.CardDefinition.CardType.Id;
        ECardOptionSemantic semantic = resolvedCard.SelectedOption.Semantic;
        EChildStatusType dominantStat = ResolveDominantStat(childState);

        return typeId switch
        {
            "card_type_letter" => ResolveLetterLine(semantic, dominantStat),
            "card_type_visitor" => ResolveVisitorLine(semantic, dominantStat),
            "card_type_textbook" => ResolveTextbookLine(semantic, dominantStat),
            "card_type_excursion" => ResolveExcursionLine(semantic, dominantStat),
            "card_type_externalinfo" => ResolveExternalInfoLine(semantic, dominantStat),
            "card_type_houseTalk" => ResolveHouseTalkLine(semantic, dominantStat),
            _ => ResolveFallbackLine(childState),
        };
    }

    private static EChildStatusType ResolveDominantStat(RuntimeChildState childState)
    {
        EChildStatusType dominant = EChildStatusType.Trust;
        int maxValue = int.MinValue;

        foreach (EChildStatusType statType in System.Enum.GetValues(typeof(EChildStatusType)))
        {
            int value = childState.GetStat(statType);
            if (value > maxValue)
            {
                maxValue = value;
                dominant = statType;
            }
        }

        return dominant;
    }

    private static string ResolveLetterLine(ECardOptionSemantic semantic, EChildStatusType dominantStat)
    {
        return semantic switch
        {
            ECardOptionSemantic.Direct when dominantStat == EChildStatusType.Trust => "편지를 다 읽으니까 아직 나를 생각해 주는 사람이 있는 것 같아.",
            ECardOptionSemantic.Direct => "편지 속엔 내가 모르는 이야기가 더 있을 것 같아.",
            ECardOptionSemantic.Modified => "부드럽긴 한데, 중간이 조금 비어 있는 것 같아.",
            ECardOptionSemantic.Blocked when dominantStat == EChildStatusType.Curiosity => "내 앞으로 온 편지도 있었을까?",
            ECardOptionSemantic.Blocked => "오늘은 우편 가방이 내 앞에 닿지 않았어.",
            _ => "편지 안엔 뭐가 적혀 있었을까.",
        };
    }

    private static string ResolveVisitorLine(ECardOptionSemantic semantic, EChildStatusType dominantStat)
    {
        return semantic switch
        {
            ECardOptionSemantic.Direct when dominantStat == EChildStatusType.Trust => "그 사람은 나를 제대로 보고 말해 준 것 같아.",
            ECardOptionSemantic.Direct => "새로운 사람을 만나면 바깥 공기가 따라와.",
            ECardOptionSemantic.Blocked when dominantStat == EChildStatusType.Anxiety => "문 앞에서 돌려보내면 괜히 더 무서워져.",
            ECardOptionSemantic.Blocked => "나를 보러 온 사람은 왜 그냥 돌아갈까?",
            _ => "방문객이 지나가고 나면 공기까지 바뀌는 것 같아.",
        };
    }

    private static string ResolveTextbookLine(ECardOptionSemantic semantic, EChildStatusType dominantStat)
    {
        return semantic switch
        {
            ECardOptionSemantic.Direct => "책에 적힌 이름들엔 다 이유가 있는 것 같아.",
            ECardOptionSemantic.Modified when dominantStat == EChildStatusType.Obedience => "책에 적힌 대로 기억하면 되는 거지?",
            ECardOptionSemantic.Modified => "책이 너무 반듯하면 오히려 더 이상해.",
            ECardOptionSemantic.Blocked => "책장이 갑자기 건너뛰면, 그 앞뒤가 더 궁금해져.",
            _ => "교재는 많은 걸 말해 주는 것 같으면서도 아니야.",
        };
    }

    private static string ResolveExcursionLine(ECardOptionSemantic semantic, EChildStatusType dominantStat)
    {
        return semantic switch
        {
            ECardOptionSemantic.Direct when dominantStat == EChildStatusType.Curiosity => "밖은 생각보다 더 가까웠어.",
            ECardOptionSemantic.Direct => "짧은 외출이었는데도 마음이 오래 남아.",
            ECardOptionSemantic.Blocked when dominantStat == EChildStatusType.Anxiety => "나가지 못하면 바깥 소리만 더 커져.",
            ECardOptionSemantic.Blocked => "조금만 더 가 보면 뭐가 있었을 텐데.",
            _ => "바깥으로 향하는 길이 또 조금 멀어졌어.",
        };
    }

    private static string ResolveExternalInfoLine(ECardOptionSemantic semantic, EChildStatusType dominantStat)
    {
        return semantic switch
        {
            ECardOptionSemantic.Direct => "모두가 아는 이야기라면 나도 알아야 하지 않을까?",
            ECardOptionSemantic.Modified when dominantStat == EChildStatusType.Trust => "괜찮다고 하니까 믿고 보고 싶어.",
            ECardOptionSemantic.Modified => "같은 말을 하는데도 어딘가 이상해.",
            ECardOptionSemantic.Blocked when dominantStat == EChildStatusType.Anxiety => "숨길수록 소문은 더 크게 들려.",
            ECardOptionSemantic.Blocked => "아무 말도 안 해 주면, 더 많은 걸 상상하게 돼.",
            _ => "밖에서는 무슨 말이 돌고 있는 걸까.",
        };
    }

    private static string ResolveHouseTalkLine(ECardOptionSemantic semantic, EChildStatusType dominantStat)
    {
        return semantic switch
        {
            ECardOptionSemantic.Direct => "누가 지나간 뒤에 말이 멈추면 더 들리게 돼.",
            ECardOptionSemantic.Modified when dominantStat == EChildStatusType.Obedience => "다들 조심하는 데는 이유가 있겠지.",
            ECardOptionSemantic.Modified => "부드럽게 말해도 숨기는 건 느껴져.",
            ECardOptionSemantic.Blocked when dominantStat == EChildStatusType.Curiosity => "왜 내가 오면 다들 입을 닫지?",
            ECardOptionSemantic.Blocked => "조용한데도 뭔가 남아 있는 기분이야.",
            _ => "집 안의 침묵은 생각보다 무거워.",
        };
    }

    private static string ResolveFallbackLine(RuntimeChildState childState)
    {
        EChildStatusType dominantStat = ResolveDominantStat(childState);
        return dominantStat switch
        {
            EChildStatusType.Trust => "아직은 여기 말을 믿고 싶어.",
            EChildStatusType.Curiosity => "조금만 더 보면 알 수 있을 것 같아.",
            EChildStatusType.Anxiety => "모르는 게 많아질수록 더 조용해져.",
            EChildStatusType.Obedience => "지금은 들은 대로 기억할래.",
            _ => "오늘은 그냥 조용히 있고 싶어.",
        };
    }
}
