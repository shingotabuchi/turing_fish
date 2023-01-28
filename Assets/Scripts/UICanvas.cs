using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICanvas : MonoBehaviour
{
    public LBM3DAccurate lbm3d;
    public Toggle tangentialForceToggle;
    public Slider tangentialForceScaler;
    public Slider tangentialForceRadius;

    public void OnToggleChange(int forceType)
    {
        lbm3d.tangentialForceIsOn = tangentialForceToggle.isOn;
        lbm3d.compute.SetBool("tangentialForceIsOn",lbm3d.tangentialForceIsOn);
    }
}
