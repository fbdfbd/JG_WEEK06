public readonly struct WeekFlowActionResult
{
    private WeekFlowActionResult(bool shouldRefreshUi, bool shouldReplaceScreen, WeekFlowScreen nextScreen)
    {
        ShouldRefreshUi = shouldRefreshUi;
        ShouldReplaceScreen = shouldReplaceScreen;
        NextScreen = nextScreen;
    }

    public bool ShouldRefreshUi { get; }
    public bool ShouldReplaceScreen { get; }
    public WeekFlowScreen NextScreen { get; }

    public static WeekFlowActionResult None => new(false, false, null);
    public static WeekFlowActionResult RefreshOnly() => new(true, false, null);
    public static WeekFlowActionResult ReplaceScreen(WeekFlowScreen nextScreen) => new(true, true, nextScreen);
    public static WeekFlowActionResult ClearScreen() => new(true, true, null);
}
