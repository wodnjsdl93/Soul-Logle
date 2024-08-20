using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public AudioMixer masterMixer;
    public Slider audioSlider;
    
    public void AudioControl()
    {
        float sound = audioSlider.value;

        if (sound <= -40f)
        {
            masterMixer.SetFloat("BGM", -80f);
        }
        else
        {
            masterMixer.SetFloat("BGM", sound);
        }
    }
    
    public void ToggleAudioVolume()
    {
        AudioListener.volume = AudioListener.volume == 0 ? 1 : 0;
    }
}
