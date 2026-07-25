using UnityEngine;

public class AudioController : MonoBehaviour
{
  [SerializeField] private AudioSource bg_adudio;
  [SerializeField] internal AudioSource audioPlayer_wl;
  [SerializeField] internal AudioSource audioPlayer_button;
  [SerializeField] internal AudioSource audioSpin_button;
  [SerializeField] private AudioClip[] clips;
  [SerializeField] private AudioSource bg_audioBonus;
  [SerializeField] private AudioSource audioPlayer_Bonus;

  private bool soundMuted = false;
  private bool musicMuted = false;
  private bool focusForceMuted = false;

  private void Start()
  {
    if (bg_adudio) bg_adudio.Play();
    audioPlayer_button.clip = clips[clips.Length - 1];
    audioSpin_button.clip = clips[clips.Length - 2];
  }

  internal void CheckFocusFunction(bool focus, bool IsSpinning)
  {
    if (!focus)
    {
      bg_adudio.Pause();
      audioPlayer_wl.Pause();
      audioPlayer_button.Pause();
    }
    else
    {
      if (!bg_adudio.mute) bg_adudio.UnPause();
      if (IsSpinning)
      {
        if (!audioPlayer_wl.mute) audioPlayer_wl.UnPause();
      }
      else
      {
        StopWLAaudio();
      }
      if (!audioPlayer_button.mute) audioPlayer_button.UnPause();
    }
  }

  internal void SwitchBGSound(bool isbonus)
  {
    if (isbonus)
    {
      if (bg_audioBonus) bg_audioBonus.enabled = true;
      if (bg_adudio) bg_adudio.enabled = false;
    }
    else
    {
      if (bg_audioBonus) bg_audioBonus.enabled = false;
      if (bg_adudio) bg_adudio.enabled = true;
    }
  }

  internal void PlayWLAudio(string type)
  {
    audioPlayer_wl.loop = false;
    int index = 0;
    switch (type)
    {
      case "bigwin":
        index = 0;
        break;
      case "win":
        index = 1;
        break;
      case "lose":
        index = 2;
        break;
      case "spinStop":
        index = 3;
        break;
      case "megaWin":
        index = 4;
        break;
    }
    StopWLAaudio();
    audioPlayer_wl.clip = clips[index];
    audioPlayer_wl.Play();
  }

  internal void PlayButtonAudio()
  {
    audioPlayer_button.Play();
  }

  internal void PlaySpinButtonAudio()
  {
    audioSpin_button.Play();
  }

  internal void StopWLAaudio()
  {
    audioPlayer_wl.Stop();
    audioPlayer_wl.loop = false;
  }

  internal void ToggleMute(bool toggle, string type)
  {
    // A direct UI toggle proves the game currently has real interactive focus,
    // so it must win over a stale/stuck forced-mute from a focus signal that
    // never got its matching regain event (can happen in WebView embeds).
    focusForceMuted = false;

    switch (type)
    {
      case "music":
        musicMuted = toggle;
        break;
      case "sound":
        soundMuted = toggle;
        break;
    }
    ApplyMuteState();
  }

  // Regaining focus must never un-mute audio the user muted, so this only
  // layers a forced mute on top of the existing sound/music toggle state.
  internal void SetMuteAll(bool mute)
  {
    focusForceMuted = mute;
    ApplyMuteState();
  }

  private void ApplyMuteState()
  {
    ApplySourceMute(bg_adudio, focusForceMuted || musicMuted);
    ApplySourceMute(bg_audioBonus, focusForceMuted || musicMuted);
    ApplySourceMute(audioPlayer_button, focusForceMuted || soundMuted);
    ApplySourceMute(audioPlayer_wl, focusForceMuted || soundMuted);
    ApplySourceMute(audioSpin_button, focusForceMuted || soundMuted);
    ApplySourceMute(audioPlayer_Bonus, focusForceMuted || soundMuted);
  }

  // CheckFocusFunction (the native/editor OnApplicationFocus path) pauses sources
  // independently of this mute flag. Un-pausing here whenever a source becomes
  // audible keeps the two mechanisms from desyncing — otherwise a source paused
  // by a focus event stays silent even after this unmutes it, until the next
  // focus-regain event happens to call UnPause() itself.
  private void ApplySourceMute(AudioSource source, bool mute)
  {
    source.mute = mute;
    if (!mute) source.UnPause();
  }

}
