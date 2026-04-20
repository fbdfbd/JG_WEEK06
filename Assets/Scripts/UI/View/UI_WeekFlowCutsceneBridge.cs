using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_WeekFlowCutsceneBridge : WeekFlowCutsceneBridgeBase
{
    [SerializeField] private SO_WeekFlowCutsceneCatalogGroup _catalogGroup;
    [SerializeField] private SO_WeekFlowCutsceneCatalog _catalog;
    [SerializeField] private WeekFlowCutscenePlayerBase[] _players;

    private readonly Dictionary<string, WeekFlowCutscenePlayerBase> _playerLookup = new();
    private readonly WeekFlowCutsceneResolver _resolver = new();

    private WeekFlowCutscenePlayerBase _activeSessionPlayer;
    private WeekFlowCutscenePlayerBase _activeTransientPlayer;

    public override bool IsPlaying =>
        (_activeSessionPlayer != null && _activeSessionPlayer.IsPlaying) ||
        (_activeTransientPlayer != null && _activeTransientPlayer.IsPlaying);

    public override bool IsBlocking =>
        (_activeSessionPlayer != null && _activeSessionPlayer.IsPlaying && _activeSessionPlayer.IsBlocking) ||
        (_activeTransientPlayer != null && _activeTransientPlayer.IsPlaying && _activeTransientPlayer.IsBlocking);

    private void Awake()
    {
        CachePlayers();
    }

    private void OnValidate()
    {
        CachePlayers();
    }

    public override IEnumerator Play(WeekFlowCutsceneRequest request)
    {
        CachePlayers();

        if (request.Moment == EWeekFlowCutsceneMoment.EventExit)
        {
            yield return HandleEventExit(request);
            yield break;
        }

        if (!TryResolveCutsceneId(request, out string cutsceneId))
        {
            yield break;
        }

        if (!_playerLookup.TryGetValue(cutsceneId, out WeekFlowCutscenePlayerBase player) || player == null)
        {
            yield break;
        }

        if (!player.CanPlay(request))
        {
            yield break;
        }

        if (request.Moment == EWeekFlowCutsceneMoment.EventEnter || player.PersistsForEvent)
        {
            yield return PlaySessionPlayer(player, request);
            yield break;
        }

        yield return PlayTransientPlayer(player, request);
    }

    public override bool TrySkip()
    {
        if (_activeTransientPlayer != null && _activeTransientPlayer.TrySkip())
        {
            return true;
        }

        return _activeSessionPlayer != null && _activeSessionPlayer.TrySkip();
    }

    public override void StopImmediate()
    {
        StopPlayer(ref _activeTransientPlayer);
        StopPlayer(ref _activeSessionPlayer);
    }

    private void CachePlayers()
    {
        _playerLookup.Clear();

        if (_players == null || _players.Length == 0)
        {
            _players = GetComponentsInChildren<WeekFlowCutscenePlayerBase>(true);
        }

        for (int index = 0; index < _players.Length; index++)
        {
            WeekFlowCutscenePlayerBase player = _players[index];
            if (player == null || string.IsNullOrWhiteSpace(player.CutsceneId))
            {
                continue;
            }

            _playerLookup[player.CutsceneId] = player;
        }
    }

    private bool TryResolveCutsceneId(WeekFlowCutsceneRequest request, out string cutsceneId)
    {
        cutsceneId = string.Empty;

        if (_catalogGroup != null)
        {
            SO_WeekFlowCutsceneCatalog weekCatalog = _catalogGroup.GetWeekCatalog(request.WeekId);
            if (_resolver.TryResolveCutsceneId(request, weekCatalog, out cutsceneId))
            {
                return true;
            }

            if (_resolver.TryResolveCutsceneId(request, _catalogGroup.DefaultCatalog, out cutsceneId))
            {
                return true;
            }
        }

        return _resolver.TryResolveCutsceneId(request, _catalog, out cutsceneId);
    }

    private IEnumerator PlaySessionPlayer(WeekFlowCutscenePlayerBase player, WeekFlowCutsceneRequest request)
    {
        if (_activeSessionPlayer != null && _activeSessionPlayer != player)
        {
            _activeSessionPlayer.StopImmediate();
            _activeSessionPlayer = null;
        }

        _activeSessionPlayer = player;
        yield return player.Play(request);
    }

    private IEnumerator PlayTransientPlayer(WeekFlowCutscenePlayerBase player, WeekFlowCutsceneRequest request)
    {
        if (_activeTransientPlayer != null && _activeTransientPlayer != player)
        {
            _activeTransientPlayer.StopImmediate();
            _activeTransientPlayer = null;
        }

        _activeTransientPlayer = player;
        yield return player.Play(request);

        if (_activeTransientPlayer == player)
        {
            _activeTransientPlayer = null;
        }
    }

    private IEnumerator HandleEventExit(WeekFlowCutsceneRequest request)
    {
        StopPlayer(ref _activeTransientPlayer);

        if (_activeSessionPlayer == null)
        {
            yield break;
        }

        if (_activeSessionPlayer.CanPlay(request))
        {
            yield return _activeSessionPlayer.Play(request);
        }

        StopPlayer(ref _activeSessionPlayer);
    }

    private static void StopPlayer(ref WeekFlowCutscenePlayerBase player)
    {
        if (player == null)
        {
            return;
        }

        player.StopImmediate();
        player = null;
    }
}
