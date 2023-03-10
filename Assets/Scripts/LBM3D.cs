using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class LBM3D : MonoBehaviour
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
    Texture3D tex3D;
    public int DIM;
    public bool periodic;
    public ComputeShader compute;
    public float u0;
    public float rho0 = 1.0f;
    public float Re = 150f;
    public float forceRadius,forceScaler;
    float nu,tau,omega;
    int setRandom,initVortex,initZero,collisionsVortex,streaming,boundaries,periodicBoundaries,collisionsCenter;
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
        collisionsVortex = compute.FindKernel("Collisions");
        collisionsCenter = compute.FindKernel("CollisionsCenterForce");
        streaming = compute.FindKernel("Streaming");
        boundaries = compute.FindKernel("Boundaries");
        periodicBoundaries = compute.FindKernel("PeriodicBoundaries");

        compute.SetInt("DIM",DIM);

        SetVariables();

        compute.SetBuffer(setRandom,"pixels3D",pixelBuffer);
        compute.SetBuffer(initVortex,"pixels3D",pixelBuffer);
        compute.SetBuffer(initVortex,"f",f);

        compute.SetBuffer(initZero,"pixels3D",pixelBuffer);
        compute.SetBuffer(initZero,"f",f);
        
        compute.SetBuffer(collisionsVortex,"pixels3D",pixelBuffer);
        compute.SetBuffer(collisionsVortex,"f",f);
        compute.SetBuffer(collisionsCenter,"pixels3D",pixelBuffer);
        compute.SetBuffer(collisionsCenter,"f",f);
        compute.SetBuffer(streaming,"f",f);
        compute.SetBuffer(boundaries,"f",f);
        compute.SetBuffer(periodicBoundaries,"f",f);

        // SetRandomPixelsCompute();
        // SetRandomPixels();
        if(initType == InitType.Zero) compute.Dispatch(initZero,(DIM+7)/8,(DIM+7)/8,(DIM+7)/8);
        if(initType == InitType.Vortex) compute.Dispatch(initVortex,(DIM+7)/8,(DIM+7)/8,(DIM+7)/8);
        
        pixelBuffer.GetData(pixels3D);
        tex3D.SetPixels(pixels3D);
        tex3D.Apply();
    }

    private void Update() {
        // SetRandomPixelsCompute();
        for (int k = 0; k < loopCount; k++)
        {
            if(forceType == ForceType.Vortex) compute.Dispatch(collisionsVortex,(DIM+7)/8,(DIM+7)/8,(DIM+7)/8);
            if(forceType == ForceType.Center) compute.Dispatch(collisionsCenter,(DIM+7)/8,(DIM+7)/8,(DIM+7)/8);
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
