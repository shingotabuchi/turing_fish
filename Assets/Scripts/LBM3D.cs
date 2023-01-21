using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class LBM3D : MonoBehaviour
{
    VisualEffect vfx;
    Texture3D tex3D;
    public int DIM;
    public bool periodic;
    public ComputeShader compute;
    public float u_lid,v_lid,w_lid;
    public bool vortexMode;
    public float u0;
    public float rho0 = 1.0f;
    public float Re = 150f;
    public int lidWidth;
    public float forceRadius,forceScaler;
    float nu,tau,omega;
    int setRandom,initVortex,initZero,collisions,streaming,boundaries,periodicBoundaries;
    ComputeBuffer pixelBuffer,f;
    Color[] pixels3D;
    public int loopCount = 1;
    
    private void Start() {
        vfx = GetComponent<VisualEffect>();
        tex3D = new Texture3D(DIM,DIM,DIM,TextureFormat.RGBA32,false);
        vfx.SetTexture("FlowField",tex3D);
        pixels3D = new Color[DIM*DIM*DIM];
        pixelBuffer = new ComputeBuffer(DIM*DIM*DIM,sizeof(float)*4);

        f = new ComputeBuffer(DIM*DIM*DIM*27*2,sizeof(float));

        setRandom = compute.FindKernel("SetRandom");
        initVortex = compute.FindKernel("InitVortex");
        initZero = compute.FindKernel("InitZero");

        compute.SetInt("DIM",DIM);

        SetVariables();

        compute.SetBuffer(setRandom,"pixels3D",pixelBuffer);
        compute.SetBuffer(initVortex,"pixels3D",pixelBuffer);
        compute.SetBuffer(initVortex,"f",f);

        compute.SetBuffer(initZero,"pixels3D",pixelBuffer);
        compute.SetBuffer(initZero,"f",f);
        if(vortexMode)
        {
            collisions = compute.FindKernel("Collisions");
            streaming = compute.FindKernel("Streaming");
            boundaries = compute.FindKernel("Boundaries");
            periodicBoundaries = compute.FindKernel("PeriodicBoundaries");
            compute.SetBuffer(collisions,"pixels3D",pixelBuffer);
            compute.SetBuffer(collisions,"f",f);
            compute.SetBuffer(streaming,"f",f);
            compute.SetBuffer(boundaries,"f",f);
            compute.SetBuffer(periodicBoundaries,"f",f);
        }

        // SetRandomPixelsCompute();
        // SetRandomPixels();
        compute.Dispatch(initZero,(DIM+7)/8,(DIM+7)/8,(DIM+7)/8);
        pixelBuffer.GetData(pixels3D);
        tex3D.SetPixels(pixels3D);
        tex3D.Apply();
    }

    private void Update() {
        // SetRandomPixelsCompute();
        for (int k = 0; k < loopCount; k++)
        {
            compute.Dispatch(collisions,(DIM+7)/8,(DIM+7)/8,(DIM+7)/8);
            compute.Dispatch(streaming,(DIM+7)/8,(DIM+7)/8,(DIM+7)/8);
            if(periodic) compute.Dispatch(periodicBoundaries,(DIM+7)/8,(DIM+7)/8,1);
            else compute.Dispatch(boundaries,(DIM+7)/8,(DIM+7)/8,1);
        }
        pixelBuffer.GetData(pixels3D);
        tex3D.SetPixels(pixels3D);
        tex3D.Apply();
    }

    void SetRandomPixels()
    {
        for(int i = 0; i < DIM*DIM*DIM;i++)
        {
            pixels3D[i] = new Color(Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f),1f);
            if(i < 100) print(pixels3D[i]);
        }
        tex3D.SetPixels(pixels3D);
        tex3D.Apply();
    }

    void SetRandomPixelsCompute()
    {
        compute.SetInt("offsetx",Random.Range(0,System.Int32.MaxValue));
        compute.SetInt("offsety",Random.Range(0,System.Int32.MaxValue));
        compute.SetInt("offsetz",Random.Range(0,System.Int32.MaxValue));

        compute.Dispatch(setRandom,(DIM+7)/8,(DIM+7)/8,(DIM+7)/8);

        pixelBuffer.GetData(pixels3D);
        for (int i = 0; i < 100; i++)
        {
            print(pixels3D[i]);
        }
        tex3D.SetPixels(pixels3D);
        tex3D.Apply();
    }

    private void OnValidate() {
        SetVariables();
    }

    void SetVariables()
    {
        nu = u_lid * DIM / Re;
        if(vortexMode) nu = u0 * DIM / Re;
        tau = 3.0f * nu + 0.5f;
        omega = 1.0f / tau;
        compute.SetInt("lidWidth",lidWidth);
        compute.SetFloat("rho0",rho0);
        compute.SetFloat("u_lid",u_lid);
        compute.SetFloat("v_lid",v_lid);
        compute.SetFloat("w_lid",w_lid);
        compute.SetFloat("tau",tau);
        if(vortexMode)
        {
            compute.SetFloat("forceRadius",forceRadius);
            compute.SetFloat("forceScaler",forceScaler);
            compute.SetFloat("u0",u0);
        }
    }
}
