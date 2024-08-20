using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public AudioMixer masterMixer;
    public Slider audioSlider;
    private const float minVolume = -30f;
    private const float maxVolume = 0f;

    void Start()
    {
        // 슬라이더의 값을 오디오 믹서와 동기화
        float currentVolume;
        if (masterMixer.GetFloat("BGM", out currentVolume))
        {
            audioSlider.value = currentVolume;
        }
        else
        {
            audioSlider.value = maxVolume;  // 기본값 설정
        }

        audioSlider.minValue = minVolume;
        audioSlider.maxValue = maxVolume;
        audioSlider.onValueChanged.AddListener(delegate { AudioControl(); });
    }

    public void AudioControl()
    {
        float sound = audioSlider.value;

        if (sound <= minVolume)
        {
            masterMixer.SetFloat("BGM", minVolume);
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

