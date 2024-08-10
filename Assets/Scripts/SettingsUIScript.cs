using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUIScript : MonoBehaviour
{

    Slider sensitivitySlider;
    Slider FOVSlider;
    Slider VolumeSlider;


    void OnEnable()
    {
        sensitivitySlider = transform.Find("Sensitivity").GetComponent<Slider>();
        FOVSlider = transform.Find("FOV").GetComponent<Slider>();
        VolumeSlider = transform.Find("Volume").GetComponent<Slider>();

        sensitivitySlider.value = PlayerPrefs.GetFloat("sensitivity");
        FOVSlider.value = PlayerPrefs.GetFloat("FOV");
        VolumeSlider.value = PlayerPrefs.GetFloat("volume");
    }

    public void sensitivitySliderChanged()
    {
        PlayerPrefs.SetFloat("sensitivity", sensitivitySlider.value);
    }

    public void FOVSliderChanged()
    {
        PlayerPrefs.SetFloat("FOV", FOVSlider.value);
    }

    public void VolumeSliderChanged()
    {
        PlayerPrefs.SetFloat("volume", VolumeSlider.value);
    }


}
