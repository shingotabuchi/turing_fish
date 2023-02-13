using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UICanvas : MonoBehaviour
{
    public LBM3DAccurate lbm3d;

    public Toggle tangentialForceToggle;
    public TextMeshProUGUI tangentialForceScalerText;
    public Slider tangentialForceScaler;
    public TextMeshProUGUI tangentialForceRadiusText;
    public Slider tangentialForceRadius;

    public Toggle radialForceToggle;
    public TextMeshProUGUI radialForceScalerText;
    public Slider radialForceScaler;
    public TextMeshProUGUI radialForceRadiusText;
    public Slider radialForceRadius;

    public Toggle touchForceToggle;
    public TextMeshProUGUI touchForceScalerText;
    public Slider touchForceScaler;
    public TextMeshProUGUI touchForceRadiusText;
    public Slider touchForceRadius;

    public Camera sideCamera;
    public Camera topCamera;
    public GameObject constantForceMenu;
    public GameObject touchForceMenu;

    void Start()
    {
        tangentialForceScalerText.text = "Force Scaler : " + lbm3d.tangentialForceScaler.ToString();
        radialForceScalerText.text = "Force Radius : " + lbm3d.radialForceScaler.ToString();
        tangentialForceRadiusText.text = "Force Scaler : " + lbm3d.tangentialForceRadius.ToString();
        radialForceRadiusText.text = "Force Radius : " + lbm3d.radialForceRadius.ToString();
        touchForceScalerText.text = "Force Scaler : " + lbm3d.touchForceScaler.ToString();
        touchForceRadiusText.text = "Force Radius : " + lbm3d.touchForceRadius.ToString();
    }

    public void OnToggleChange(int forceType)
    {
        if(forceType == 0)
        {
            lbm3d.tangentialForceIsOn = tangentialForceToggle.isOn;
            lbm3d.compute.SetBool("tangentialForceIsOn",lbm3d.tangentialForceIsOn);
        }
        else if(forceType == 1)
        {
            lbm3d.radialForceIsOn = radialForceToggle.isOn;
            lbm3d.compute.SetBool("radialForceIsOn",lbm3d.radialForceIsOn);
        }
        else
        {
            lbm3d.touchForceIsOn = touchForceToggle.isOn;
            lbm3d.compute.SetBool("touchForceIsOn",lbm3d.touchForceIsOn);
        }
    }
    
    public void OnScalerSliderChange(int forceType)
    {
        if(forceType == 0)
        {
            lbm3d.tangentialForceScaler = tangentialForceScaler.value;
            lbm3d.compute.SetFloat("tangentialForceScaler",lbm3d.tangentialForceScaler);
            tangentialForceScalerText.text = "Force Scaler : " + lbm3d.tangentialForceScaler.ToString();
        } 
        else if(forceType == 1)
        {
            lbm3d.radialForceScaler = radialForceScaler.value;
            lbm3d.compute.SetFloat("radialForceScaler",lbm3d.radialForceScaler);
            radialForceScalerText.text = "Force Scaler : " + lbm3d.radialForceScaler.ToString();
        }
        else
        {
            lbm3d.touchForceScaler = touchForceScaler.value;
            lbm3d.compute.SetFloat("touchForceScaler",lbm3d.touchForceScaler);
            touchForceScalerText.text = "Force Scaler : " + lbm3d.touchForceScaler.ToString();
        }
    }

    public void OnRadiusSliderChange(int forceType)
    {
        if(forceType == 0)
        {
            lbm3d.tangentialForceRadius = tangentialForceRadius.value;
            lbm3d.compute.SetFloat("tangentialForceRadius",lbm3d.tangentialForceRadius);
            tangentialForceRadiusText.text = "Force Radius : " + lbm3d.tangentialForceRadius.ToString();
        }
        else if(forceType == 1)
        {
            lbm3d.radialForceRadius = radialForceRadius.value;
            lbm3d.compute.SetFloat("radialForceRadius",lbm3d.radialForceRadius);
            radialForceRadiusText.text = "Force Radius : " + lbm3d.radialForceRadius.ToString();
        }
        else
        {
            lbm3d.touchForceRadius = touchForceRadius.value;
            lbm3d.compute.SetFloat("touchForceRadius",lbm3d.touchForceRadius);
            touchForceRadiusText.text = "Force Radius : " + lbm3d.touchForceRadius.ToString();
        }
    }

    public void Initialize()
    {
        lbm3d.Initialize();
    }

    public void SwitchCamera()
    {
        sideCamera.enabled = topCamera.enabled;
        topCamera.enabled = !topCamera.enabled;
    }

    public void SwitchForceMenu()
    {
        constantForceMenu.SetActive(touchForceMenu.activeSelf);
        touchForceMenu.SetActive(!touchForceMenu.activeSelf);
    }
}
