using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
// using System.Runtime.InteropServices;
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
    public float forceRadius,forceScaler;
    float nu,tau,omega;
    int setRandom,initVortex,initZero,collisionsVortex,streaming,boundaries,periodicBoundaries,collisionsCenter;
    ComputeBuffer f;
    Vector3[] velocityArray;
    GraphicsBuffer velocityGraphicsBuffer;
    public int loopCount = 1;
    public bool tangentialForceIsOn;
    
    private void Start() {
        vfx = GetComponent<VisualEffect>();
        velocityGraphicsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, DIM*DIM*DIM, sizeof(float)*3);
        vfx.SetGraphicsBuffer("VelocityBuffer",velocityGraphicsBuffer);
        velocityArray = new Vector3[DIM*DIM*DIM];
        f = new ComputeBuffer(DIM*DIM*DIM*27*2,sizeof(float));

        initVortex = compute.FindKernel("InitVortex");
        initZero = compute.FindKernel("InitZero");
        collisionsVortex = compute.FindKernel("Collisions");
        collisionsCenter = compute.FindKernel("CollisionsCenterForce");
        streaming = compute.FindKernel("Streaming");
        boundaries = compute.FindKernel("Boundaries");
        periodicBoundaries = compute.FindKernel("PeriodicBoundaries");

        compute.SetInt("DIM",DIM);
        compute.SetBool("tangentialForceIsOn",tangentialForceIsOn);

        SetVariables();

        compute.SetBuffer(initVortex,"velocityBuffer",velocityGraphicsBuffer);
        compute.SetBuffer(initVortex,"f",f);

        compute.SetBuffer(initZero,"velocityBuffer",velocityGraphicsBuffer);
        compute.SetBuffer(initZero,"f",f);
        
        compute.SetBuffer(collisionsVortex,"velocityBuffer",velocityGraphicsBuffer);
        compute.SetBuffer(collisionsVortex,"f",f);
        compute.SetBuffer(collisionsCenter,"velocityBuffer",velocityGraphicsBuffer);
        compute.SetBuffer(collisionsCenter,"f",f);
        compute.SetBuffer(streaming,"f",f);
        compute.SetBuffer(boundaries,"f",f);
        compute.SetBuffer(periodicBoundaries,"f",f);

        if(initType == InitType.Zero) compute.Dispatch(initZero,(DIM+7)/8,(DIM+7)/8,(DIM+7)/8);
        if(initType == InitType.Vortex) compute.Dispatch(initVortex,(DIM+7)/8,(DIM+7)/8,(DIM+7)/8);
    }

    private void Update() {
        // SetRandomPixelsCompute();
        for (int k = 0; k < loopCount; k++)
        {
            if(forceType == ForceType.Vortex){
                // compute.SetBuffer(collisionsVortex,"velocityBuffer",velocityGraphicsBuffer);
                compute.Dispatch(collisionsVortex,(DIM+7)/8,(DIM+7)/8,(DIM+7)/8);
            }
            if(forceType == ForceType.Center){
                compute.Dispatch(collisionsCenter,(DIM+7)/8,(DIM+7)/8,(DIM+7)/8);
            }
            compute.Dispatch(streaming,(DIM+7)/8,(DIM+7)/8,(DIM+7)/8);
            if(periodic) compute.Dispatch(periodicBoundaries,(DIM+7)/8,(DIM+7)/8,1);
            else compute.Dispatch(boundaries,(DIM+7)/8,(DIM+7)/8,1);
        }
        // tex3D.SetPixels(velocityGraphicsBuffer);
        // tex3D.Apply();
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
