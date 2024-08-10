using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUIScript : MonoBehaviour
{

    Slider sensitivitySlider;
    Slider FOVSlider;
    Slider volumeSlider;

    TMP_InputField sensitivityText;
    TMP_InputField FOVText;
    TMP_InputField volumeText;

    private float maxSensitivity = 5f;
    private float minSensitivity = 0.1f;
    private float maxFOV = 110f;
    private float minFOV = 50f;
    private float maxVolume = 1f;
    private float minVolume = 0f;



    void OnEnable()
    {
        sensitivitySlider = transform.Find("Sensitivity").GetComponent<Slider>();
        FOVSlider = transform.Find("FOV").GetComponent<Slider>();
        volumeSlider = transform.Find("Volume").GetComponent<Slider>();

        sensitivityText = transform.Find("Sensitivity/SensitivityInput").GetComponent<TMP_InputField>();
        FOVText = transform.Find("FOV/FOVInput").GetComponent<TMP_InputField>();
        volumeText = transform.Find("Volume/VolumeInput").GetComponent<TMP_InputField>();

        sensitivitySlider.value = PlayerPrefs.GetFloat("sensitivity");
        FOVSlider.value = PlayerPrefs.GetFloat("FOV");
        volumeSlider.value = PlayerPrefs.GetFloat("volume");

        sensitivityText.text = (Mathf.Round(PlayerPrefs.GetFloat("sensitivity")*10f)/10f).ToString();
        FOVText.text = (Mathf.Round(PlayerPrefs.GetFloat("FOV")*1f)/1f).ToString();
        volumeText.text = (Mathf.Round(PlayerPrefs.GetFloat("volume")*100f)/100f).ToString();
    }

    public void sensitivitySliderChanged()
    {
        PlayerPrefs.SetFloat("sensitivity", sensitivitySlider.value);
        sensitivityText.text = (Mathf.Round(PlayerPrefs.GetFloat("sensitivity")*10f)/10f).ToString();
    }

    public void FOVSliderChanged()
    {
        PlayerPrefs.SetFloat("FOV", FOVSlider.value);
        FOVText.text = (Mathf.Round(PlayerPrefs.GetFloat("FOV")*1f)/1f).ToString();
    }

    public void VolumeSliderChanged()
    {
        PlayerPrefs.SetFloat("volume", volumeSlider.value);
        volumeText.text = (Mathf.Round(PlayerPrefs.GetFloat("volume")*100f)/100f).ToString();
    }

    public void sensitivityInputClosed()
    {
        if(sensitivityText.text.EndsWith(".")) return;
        if(sensitivityText.text.Trim() == "") return;

        float val = float.Parse(sensitivityText.text);
        if(val > maxSensitivity) val = maxSensitivity;
        if(val < minSensitivity) val = minSensitivity;
        PlayerPrefs.SetFloat("sensitivity", val);
        
        if(sensitivitySlider.value == val) sensitivitySliderChanged();
        sensitivitySlider.value = val;
    }

    public void FOVInputClosed()
    {
        if(FOVText.text.EndsWith(".")) return;
        if(FOVText.text.Trim() == "") return;

        float val = float.Parse(FOVText.text);
        if(val > maxFOV) val = maxFOV;
        if(val < minFOV) val = minFOV;
        PlayerPrefs.SetFloat("FOV", val);
        
        if(FOVSlider.value == val) FOVSliderChanged();
        FOVSlider.value = val;
    }

    public void VolumeInputClosed()
    {
        if(volumeText.text.EndsWith(".")) return;
        if(volumeText.text.Trim() == "") return;

        float val = float.Parse(volumeText.text);
        if(val > maxVolume) val = maxVolume;
        if(val < minVolume) val = minVolume;
        PlayerPrefs.SetFloat("volume", val);
        
        if(volumeSlider.value == val) VolumeSliderChanged();
        volumeSlider.value = val;
    }
}
