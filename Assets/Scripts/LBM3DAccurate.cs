using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class LBM3DAccurate : MonoBehaviour
{
    public enum ForceType
    {
        Vortex,
        Center,
    }
    public enum InitType
    {
        Zero,
        Vortex,
    }
    public ForceType forceType;
    public InitType initType;
    VisualEffect vfx;
    public int DIM;
    public bool periodic;
    public ComputeShader compute;
    public float u0;
    public float rho0 = 1.0f;
    public float Re = 150f;
    public float tangentialForceRadius,tangentialForceScaler;
    public float radialForceRadius,radialForceScaler;
    public float touchForceRadius,touchForceScaler;
    float nu,tau,omega;
    int setRandom,initVortex,initZero,collisions,streaming,boundaries,periodicBoundaries;
    ComputeBuffer f;
    Vector3[] velocityArray;
    GraphicsBuffer velocityGraphicsBuffer;
    public int loopCount = 1;
    public bool tangentialForceIsOn;
    public bool radialForceIsOn;
    public bool touchForceIsOn;
    public UICanvas uiCanvas;

    Ray ray;
    RaycastHit hit;
    
    bool isTouched;
    
    private void Start()
    {
        vfx = GetComponent<VisualEffect>();
        velocityGraphicsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, DIM*DIM*DIM, sizeof(float)*3);
        vfx.SetGraphicsBuffer("VelocityBuffer",velocityGraphicsBuffer);
        velocityArray = new Vector3[DIM*DIM*DIM];
        f = new ComputeBuffer(DIM*DIM*DIM*27*2,sizeof(float));

        initVortex = compute.FindKernel("InitVortex");
        initZero = compute.FindKernel("InitZero");
        collisions = compute.FindKernel("Collisions");
        streaming = compute.FindKernel("Streaming");
        boundaries = compute.FindKernel("Boundaries");
        periodicBoundaries = compute.FindKernel("PeriodicBoundaries");

        compute.SetInt("DIM",DIM);

        SetVariables();

        compute.SetBuffer(initVortex,"velocityBuffer",velocityGraphicsBuffer);
        compute.SetBuffer(initVortex,"f",f);

        compute.SetBuffer(initZero,"velocityBuffer",velocityGraphicsBuffer);
        compute.SetBuffer(initZero,"f",f);
        
        compute.SetBuffer(collisions,"velocityBuffer",velocityGraphicsBuffer);
        compute.SetBuffer(collisions,"f",f);
        compute.SetBuffer(streaming,"f",f);
        compute.SetBuffer(boundaries,"f",f);
        compute.SetBuffer(periodicBoundaries,"f",f);

        if(initType == InitType.Zero) compute.Dispatch(initZero,(DIM+7)/8,(DIM+7)/8,(DIM+7)/8);
        if(initType == InitType.Vortex) compute.Dispatch(initVortex,(DIM+7)/8,(DIM+7)/8,(DIM+7)/8);
    }

    public void Initialize()
    {
        compute.Dispatch(initZero,(DIM+7)/8,(DIM+7)/8,(DIM+7)/8);
    }

    private void Update() {
        if(Input.GetMouseButton(0) && touchForceIsOn)
        {
            if(uiCanvas.sideCamera.enabled) ray = uiCanvas.sideCamera.ScreenPointToRay(Input.mousePosition);
            else ray = uiCanvas.topCamera.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit,Mathf.Infinity,LayerMask.GetMask("HitBox")))
            {
                if(!isTouched)
                {
                    isTouched = true;
                    compute.SetBool("isTouched",isTouched);
                }
                compute.SetVector("touchTextureCoord",hit.textureCoord);
            }
            else if(isTouched)
            {
                isTouched = false;
                compute.SetBool("isTouched",isTouched);
            }
        }
        else if(isTouched)
        {
            isTouched = false;
            compute.SetBool("isTouched",isTouched);
        }

        for (int k = 0; k < loopCount; k++)
        {
            compute.Dispatch(collisions,(DIM+7)/8,(DIM+7)/8,(DIM+7)/8);
            compute.Dispatch(streaming,(DIM+7)/8,(DIM+7)/8,(DIM+7)/8);
            if(periodic) compute.Dispatch(periodicBoundaries,(DIM+7)/8,(DIM+7)/8,1);
            else compute.Dispatch(boundaries,(DIM+7)/8,(DIM+7)/8,1);
        }
    }

    private void OnValidate() {
        SetVariables();
    }

    void SetVariables()
    {   
        tangentialForceIsOn = uiCanvas.tangentialForceToggle.isOn;
        tangentialForceScaler = uiCanvas.tangentialForceScaler.value;
        tangentialForceRadius = uiCanvas.tangentialForceRadius.value;

        radialForceIsOn = uiCanvas.radialForceToggle.isOn;
        radialForceScaler = uiCanvas.radialForceScaler.value;
        radialForceRadius = uiCanvas.radialForceRadius.value;

        touchForceIsOn = uiCanvas.touchForceToggle.isOn;
        touchForceScaler = uiCanvas.touchForceScaler.value;
        touchForceRadius = uiCanvas.touchForceRadius.value;

        nu = u0 * DIM / Re;
        tau = 3.0f * nu + 0.5f;
        omega = 1.0f / tau;
        compute.SetFloat("rho0",rho0);
        compute.SetFloat("tau",tau);
        compute.SetFloat("u0",u0);
        compute.SetFloat("tangentialForceRadius",tangentialForceRadius);
        compute.SetFloat("tangentialForceScaler",tangentialForceScaler);
        compute.SetBool("tangentialForceIsOn",tangentialForceIsOn);
        compute.SetFloat("radialForceRadius",radialForceRadius);
        compute.SetFloat("radialForceScaler",radialForceScaler);
        compute.SetBool("radialForceIsOn",radialForceIsOn);
        compute.SetFloat("touchForceRadius",touchForceRadius);
        compute.SetFloat("touchForceScaler",touchForceScaler);
        compute.SetBool("touchForceIsOn",touchForceIsOn);

    }
}
