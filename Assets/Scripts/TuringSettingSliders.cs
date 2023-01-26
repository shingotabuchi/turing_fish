using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TuringSettingSliders : MonoBehaviour
{
    public TuringPatternThree turing;
    public float killFeedMin,killFeedMax;
    public Button initButton;
    public Button quitButton;
    public enum RGB
    {
        R,
        G,
        B
    }
    public enum KillFeed
    {
        Kill,
        Feed,
    }
    void OnEnable()
    {
        initButton.onClick.RemoveAllListeners();
        initButton.onClick.AddListener(delegate{
            turing.Initialize();
        });
        quitButton.onClick.RemoveAllListeners();
        quitButton.onClick.AddListener(delegate{
            Application.Quit();
        });
        foreach(Transform child in transform)
        {
            Slider killSlider = child.Find("KillSlider").GetComponent<Slider>();
            Slider feedSlider = child.Find("FeedSlider").GetComponent<Slider>();
            killSlider.maxValue = killFeedMax;
            killSlider.minValue = killFeedMin;
            feedSlider.maxValue = killFeedMax;
            feedSlider.minValue = killFeedMin;
            killSlider.onValueChanged.RemoveAllListeners();
            feedSlider.onValueChanged.RemoveAllListeners();
            if(child.gameObject.name == "FinsR")
            {
                child.Find("Kill").GetComponent<TextMeshProUGUI>().text = "Kill : " + turing.killR.ToString("0.0000");
                child.Find("Feed").GetComponent<TextMeshProUGUI>().text = "Feed : " + turing.feedR.ToString("0.0000");
                killSlider.value = turing.killR;
                feedSlider.value = turing.feedR;

                killSlider.onValueChanged.AddListener(delegate {OnSliderValueChanged(RGB.R,KillFeed.Kill,killSlider); });
                feedSlider.onValueChanged.AddListener(delegate {OnSliderValueChanged(RGB.R,KillFeed.Feed,feedSlider); });
            }
            if(child.gameObject.name == "FaceG")
            {
                child.Find("Kill").GetComponent<TextMeshProUGUI>().text = "Kill : " + turing.killG.ToString("0.0000");
                child.Find("Feed").GetComponent<TextMeshProUGUI>().text = "Feed : " + turing.feedG.ToString("0.0000");
                killSlider.value = turing.killG;
                feedSlider.value = turing.feedG;

                killSlider.onValueChanged.AddListener(delegate {OnSliderValueChanged(RGB.G,KillFeed.Kill,killSlider); });
                feedSlider.onValueChanged.AddListener(delegate {OnSliderValueChanged(RGB.G,KillFeed.Feed,feedSlider); });
            }
            if(child.gameObject.name == "BodyB")
            {
                child.Find("Kill").GetComponent<TextMeshProUGUI>().text = "Kill : " + turing.killB.ToString("0.0000");
                child.Find("Feed").GetComponent<TextMeshProUGUI>().text = "Feed : " + turing.feedB.ToString("0.0000");
                killSlider.value = turing.killB;
                feedSlider.value = turing.feedB;

                killSlider.onValueChanged.AddListener(delegate {OnSliderValueChanged(RGB.B,KillFeed.Kill,killSlider); });
                feedSlider.onValueChanged.AddListener(delegate {OnSliderValueChanged(RGB.B,KillFeed.Feed,feedSlider); });
            }
        }

        Slider speedSlider = transform.parent.Find("SpeedSlider").GetComponent<Slider>();
        speedSlider.onValueChanged.RemoveAllListeners();
        speedSlider.value = turing.loopCount;
        transform.parent.Find("SpeedText").GetComponent<TextMeshProUGUI>().text = "Simulation speed : " + turing.loopCount.ToString();
        speedSlider.onValueChanged.AddListener(delegate {
            turing.loopCount = (int)speedSlider.value;
            transform.parent.Find("SpeedText").GetComponent<TextMeshProUGUI>().text = "Simulation speed : " + turing.loopCount.ToString();
        });

        FlexibleColorPicker fcp = transform.parent.Find("FlexibleColorPicker").GetComponent<FlexibleColorPicker>();
        fcp.color = turing.GetMaterialColor();
        fcp.onColorChange.RemoveAllListeners();
        fcp.onColorChange.AddListener(delegate {
            turing.SetMaterialColor(fcp.color);
        });

        Slider diffusionSlider = transform.parent.Find("DiffusionCoeff").GetComponent<Slider>();
        diffusionSlider.onValueChanged.RemoveAllListeners();
        diffusionSlider.value = turing.DA;
        transform.parent.Find("DiffusionText").GetComponent<TextMeshProUGUI>().text = "Diffusion Coeff : " + turing.DA.ToString("0.0000");
        diffusionSlider.onValueChanged.AddListener(delegate {
            turing.DA = diffusionSlider.value;
            turing.DB = diffusionSlider.value/2f;
            transform.parent.Find("DiffusionText").GetComponent<TextMeshProUGUI>().text = "Diffusion Coeff : " + turing.DA.ToString("0.0000");
        });

        Slider fishGlowSlider = transform.parent.Find("GlowSliders/FishGlow").GetComponent<Slider>();
        fishGlowSlider.onValueChanged.RemoveAllListeners();
        fishGlowSlider.value = turing.fishColorIntensity;
        fishGlowSlider.onValueChanged.AddListener(delegate {
            turing.fishColorIntensity = fishGlowSlider.value;
            turing.SetMaterialColor(turing.GetMaterialColor());
        });

        Slider particleGlowSlider = transform.parent.Find("GlowSliders/ParticleGlow").GetComponent<Slider>();
        particleGlowSlider.onValueChanged.RemoveAllListeners();
        particleGlowSlider.value = turing.particleColorIntensity;
        particleGlowSlider.onValueChanged.AddListener(delegate {
            turing.particleColorIntensity = particleGlowSlider.value;
            turing.SetMaterialColor(turing.GetMaterialColor());
        });

        Slider lightIntensity = transform.parent.Find("GlowSliders/LightIntensity").GetComponent<Slider>();
        lightIntensity.onValueChanged.RemoveAllListeners();
        lightIntensity.value = turing.lightIntensity;
        lightIntensity.onValueChanged.AddListener(delegate {
            turing.lightIntensity = lightIntensity.value;
            turing.SetMaterialColor(turing.GetMaterialColor());
        });
    }

    public void OnSliderValueChanged(RGB rgb, KillFeed kf, Slider slider)
    {
        switch (rgb)
        {
            case RGB.R:
            if(kf == KillFeed.Kill) 
            {
                slider.transform.parent.Find("Kill").GetComponent<TextMeshProUGUI>().text = "Kill : " + turing.killR.ToString("0.0000");
                turing.killR = slider.value;
            }
            else
            {
                slider.transform.parent.Find("Feed").GetComponent<TextMeshProUGUI>().text = "Feed : " + turing.feedR.ToString("0.0000");
                turing.feedR = slider.value;
            }
            break;

            case RGB.G:
            if(kf == KillFeed.Kill) 
            {
                slider.transform.parent.Find("Kill").GetComponent<TextMeshProUGUI>().text = "Kill : " + turing.killG.ToString("0.0000");
                turing.killG = slider.value;
            }
            else
            {
                slider.transform.parent.Find("Feed").GetComponent<TextMeshProUGUI>().text = "Feed : " + turing.feedG.ToString("0.0000");
                turing.feedG = slider.value;
            }
            break;

            case RGB.B:
            if(kf == KillFeed.Kill) 
            {
                slider.transform.parent.Find("Kill").GetComponent<TextMeshProUGUI>().text = "Kill : " + turing.killB.ToString("0.0000");
                turing.killB = slider.value;
            }
            else
            {
                slider.transform.parent.Find("Feed").GetComponent<TextMeshProUGUI>().text = "Feed : " + turing.feedB.ToString("0.0000");
                turing.feedB = slider.value;
            }
            break;
        }

        
    }

}
