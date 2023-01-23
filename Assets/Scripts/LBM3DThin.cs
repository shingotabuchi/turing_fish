using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class LBM3DThin : MonoBehaviour
{
    VisualEffect vfx;
    Texture3D tex3D;
    public int DIM;
    public int DIMWidth;
    public bool periodic;
    public ComputeShader compute;
    public float u0;
    public float rho0 = 1.0f;
    public float Re = 150f;
    public float forceRadius,forceScaler;
    float nu,tau,omega;
    int initVortex,initZero,collisions,streaming,boundaries,periodicBoundaries;
    ComputeBuffer pixelBuffer,f;
    Color[] pixels3D;
    public int loopCount = 1;
    
    private void Start() {
        vfx = GetComponent<VisualEffect>();
        tex3D = new Texture3D(DIM,DIM,DIMWidth,TextureFormat.RGBA32,false);
        vfx.SetTexture("FlowField",tex3D);
        pixels3D = new Color[DIM*DIM*DIMWidth];
        pixelBuffer = new ComputeBuffer(DIM*DIM*DIMWidth,sizeof(float)*4);

        f = new ComputeBuffer(DIM*DIM*DIMWidth*27*2,sizeof(float));

        initVortex = compute.FindKernel("InitVortex");
        initZero = compute.FindKernel("InitZero");

        compute.SetInt("DIM",DIM);
        compute.SetInt("DIMWidth",DIMWidth);

        SetVariables();

        compute.SetBuffer(initVortex,"pixels3D",pixelBuffer);
        compute.SetBuffer(initVortex,"f",f);

        compute.SetBuffer(initZero,"pixels3D",pixelBuffer);
        compute.SetBuffer(initZero,"f",f);

        collisions = compute.FindKernel("Collisions");
        streaming = compute.FindKernel("Streaming");
        boundaries = compute.FindKernel("Boundaries");
        periodicBoundaries = compute.FindKernel("PeriodicBoundaries");
        compute.SetBuffer(collisions,"pixels3D",pixelBuffer);
        compute.SetBuffer(collisions,"f",f);
        compute.SetBuffer(streaming,"f",f);
        compute.SetBuffer(boundaries,"f",f);
        compute.SetBuffer(periodicBoundaries,"f",f);

        // SetRandomPixelsCompute();
        // SetRandomPixels();
        compute.Dispatch(initZero,(DIM+7)/8,(DIM+7)/8,(DIMWidth+7)/8);
        pixelBuffer.GetData(pixels3D);
        tex3D.SetPixels(pixels3D);
        tex3D.Apply();
    }

    private void Update() {
        // SetRandomPixelsCompute();
        for (int k = 0; k < loopCount; k++)
        {
            compute.Dispatch(collisions,(DIM+7)/8,(DIM+7)/8,(DIMWidth+7)/8);
            compute.Dispatch(streaming,(DIM+7)/8,(DIM+7)/8,(DIMWidth+7)/8);
            if(periodic) compute.Dispatch(periodicBoundaries,(DIM+7)/8,(DIM+7)/8,1);
            else compute.Dispatch(boundaries,(DIM+7)/8,(DIM+7)/8,1);
        }
        pixelBuffer.GetData(pixels3D);
        tex3D.SetPixels(pixels3D);
        tex3D.Apply();
    }

    private void OnValidate() {
        SetVariables();
    }

    void SetVariables()
    {
        nu = u0 * DIM / Re;
        tau = 3.0f * nu + 0.5f;
        omega = 1.0f / tau;
        compute.SetFloat("rho0",rho0);
        compute.SetFloat("tau",tau);
        compute.SetFloat("forceRadius",forceRadius);
        compute.SetFloat("forceScaler",forceScaler);
        compute.SetFloat("u0",u0);
    }
}
