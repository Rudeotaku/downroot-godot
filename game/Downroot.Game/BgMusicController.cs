using Godot;

namespace Downroot.Game.Runtime;

[GlobalClass]
public partial class BgMusicController : Node
{
    public enum BgmState
    {
        MainMenu,
        OverworldDay,
        OverworldNight,
        PocketWorld
    }

    private const float CrossfadeDurationSec = 2f;
    private const float PausedVolumeRatio = 0.3f;
    private const string BgmBasePath = "res://../../packs/basegame/assets/audio/bgm/";

    private AudioStreamPlayer? _current;
    private AudioStreamPlayer? _fade;
    private BgmState _currentState = BgmState.MainMenu;
    private bool _wasPaused;
    private float _normalVolumeDb;
    private float _fadeTimer;
    private bool _isCrossfading;
    private BgmState _fadeTargetState;

    public BgmState CurrentState => _currentState;

    public override void _Ready()
    {
        _current = new AudioStreamPlayer
        {
            Name = "BgmCurrent",
            Bus = "Master"
        };
        _current.Finished += OnCurrentBgmFinished;
        AddChild(_current);

        _fade = new AudioStreamPlayer
        {
            Name = "BgmFade",
            Bus = "Master"
        };
        _fade.Finished += OnFadeBgmFinished;
        AddChild(_fade);
    }

    public override void _Process(double delta)
    {
        var tree = GetTree();
        var isPaused = tree is not null && tree.Paused;

        if (isPaused != _wasPaused)
        {
            _wasPaused = isPaused;
            ApplyPausedVolume(isPaused);
        }

        if (_isCrossfading)
        {
            UpdateCrossfade((float)delta);
        }
    }

    public void PlayMainMenuBgm()
    {
        HardCutTo(BgmState.MainMenu);
    }

    public void PlayOverworldBgm(bool isNight)
    {
        HardCutTo(isNight ? BgmState.OverworldNight : BgmState.OverworldDay);
    }

    public void PlayPocketWorldBgm()
    {
        HardCutTo(BgmState.PocketWorld);
    }

    public void HardCutTo(BgmState state)
    {
        if (_currentState == state && _current is { Playing: true })
        {
            return;
        }

        StopCrossfade();
        _currentState = state;
        var stream = LoadStreamForState(state);

        if (_current is not null)
        {
            _current.Stop();
            _current.Stream = stream;
            if (stream is not null)
            {
                _current.Play();
            }
        }
    }

    public void CrossfadeTo(BgmState state, float durationSec = CrossfadeDurationSec)
    {
        if (_currentState == state || _isCrossfading)
        {
            return;
        }

        var stream = LoadStreamForState(state);
        if (stream is null)
        {
            // Silent degrade: just switch state without playing
            _currentState = state;
            _current?.Stop();
            return;
        }

        _fadeTargetState = state;
        _fadeTimer = durationSec;
        _isCrossfading = true;

        // Swap: fade becomes current (playing new track), current becomes fade (will be ducked)
        (_current, _fade) = (_fade, _current);

        if (_current is not null)
        {
            _current.Stream = stream;
            _current.VolumeDb = _normalVolumeDb;
            _current.Play();
        }

        if (_fade is not null)
        {
            _fade.VolumeDb = _normalVolumeDb;
        }
    }

    public void Stop()
    {
        StopCrossfade();
        _current?.Stop();
        _fade?.Stop();
    }

    private void UpdateCrossfade(float delta)
    {
        if (_fadeTimer <= 0f)
        {
            FinishCrossfade();
            return;
        }

        _fadeTimer -= delta;
        var t = 1f - Mathf.Clamp(_fadeTimer / CrossfadeDurationSec, 0f, 1f);

        if (_current is not null)
        {
            _current.VolumeDb = Mathf.Lerp(_normalVolumeDb, _normalVolumeDb, t);
        }

        if (_fade is not null)
        {
            _fade.VolumeDb = Mathf.Lerp(_normalVolumeDb, -80f, t);
        }

        if (_fadeTimer <= 0f)
        {
            FinishCrossfade();
        }
    }

    private void FinishCrossfade()
    {
        _isCrossfading = false;
        _currentState = _fadeTargetState;
        _fade?.Stop();
    }

    private void StopCrossfade()
    {
        if (!_isCrossfading)
        {
            return;
        }

        _isCrossfading = false;
        _fade?.Stop();
    }

    private void ApplyPausedVolume(bool paused)
    {
        var targetDb = paused
            ? _normalVolumeDb + Mathf.LinearToDb(PausedVolumeRatio)
            : _normalVolumeDb;

        if (_current is not null)
        {
            _current.VolumeDb = targetDb;
        }

        if (_fade is not null)
        {
            _fade.VolumeDb = targetDb;
        }
    }

    private void OnCurrentBgmFinished()
    {
        // Loop: restart the current track
        if (_current is { Stream: not null })
        {
            _current.Play();
        }
    }

    private void OnFadeBgmFinished()
    {
        // Fade player finishing during crossfade is normal (old track ending).
        // No action needed — the fade player should be silent by then.
    }

    private static AudioStream? LoadStreamForState(BgmState state)
    {
        var fileName = state switch
        {
            BgmState.MainMenu => "Easy Lemon.mp3",
            BgmState.OverworldDay => "Earth Prelude.mp3",
            BgmState.OverworldNight => "River Flute.mp3",
            BgmState.PocketWorld => "Tranquility.mp3",
            _ => null
        };

        if (fileName is null)
        {
            return null;
        }

        try
        {
            var path = BgmBasePath + fileName;

            // Try Godot import system first (if .import files exist)
            var imported = GD.Load<AudioStream>(path);
            if (imported is not null)
            {
                return imported;
            }
        }
        catch
        {
            // Import not available, fall through to raw file load
        }

        try
        {
            // Load raw MP3 bytes via FileAccess (bypasses Godot import system)
            var path = BgmBasePath + fileName;
            using var file = Godot.FileAccess.Open(path, Godot.FileAccess.ModeFlags.Read);
            if (file is null)
            {
                GD.PushWarning($"[BgMusic] File not found: {path}");
                return null;
            }

            var bytes = file.GetBuffer((long)file.GetLength());
            var stream = new AudioStreamMP3 { Data = bytes };
            return stream;
        }
        catch (Exception ex)
        {
            GD.PushWarning($"[BgMusic] Failed to load '{fileName}': {ex.Message}");
            return null;
        }
    }
}
