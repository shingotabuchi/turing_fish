using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class LBM3D : MonoBehaviour
{
    VisualEffect vfx;
    Texture3D tex3D;
    public int DIM;
    public ComputeShader compute;
    public float ux_lid,uy_lid,uz_lid;
    public float rho0 = 1.0f;
    public float Re = 150f;
    float nu,tau,omega;
    int setRandom,init;
    ComputeBuffer pixelBuffer,f;
    Color[] pixels3D;
    const float t1 = 8f / 27f;
    const float t2 = 2f / 27f;
    const float t3 = 1f / 54f;
    const float t4 = 1f / 216f;
    // Discrete velocities
    int[] ex = new int[27]{0, 1,-1, 0, 0, 0, 0, 1,-1, 1,-1, 1,-1, 1,-1, 0, 0, 0, 0, 1,-1, 1,-1, 1,-1, 1,-1};
    int[] ey = new int[27]{0, 0, 0, 1,-1, 0, 0, 1, 1,-1,-1, 0, 0, 0, 0, 1,-1, 1,-1, 1, 1,-1,-1, 1, 1,-1,-1};
    int[] ez = new int[27]{0, 0, 0, 0, 0, 1,-1, 0, 0, 0, 0, 1, 1,-1,-1, 1, 1,-1,-1, 1, 1, 1, 1,-1,-1,-1,-1};
    
    float[] w;
    
    private void Start() {
        vfx = GetComponent<VisualEffect>();
        tex3D = new Texture3D(DIM,DIM,DIM,TextureFormat.RGBA32,false);
        vfx.SetTexture("FlowField",tex3D);
        pixels3D = new Color[DIM*DIM*DIM];
        pixelBuffer = new ComputeBuffer(DIM*DIM*DIM,sizeof(float)*4);
        f = new ComputeBuffer(DIM*DIM*DIM*27,sizeof(float));

        setRandom = compute.FindKernel("SetRandom");
        
        compute.SetInt("DIM",DIM);
        compute.SetBuffer(setRandom,"pixels3D",pixelBuffer);

        nu = ux_lid * nx / Re;
        tau = 3.0f * nu + 0.5f;
        omega = 1.0f / tau;

        w = new float[27]{t1,t2,t2,t2,t2,t2,t2,t3,t3,t3,t3,t3,t3,t3,t3,t3,t3,t3,t3,t4,t4,t4,t4,t4,t4,t4,t4};

    }

    private void Update() {
        SetRandomPixelsCompute();
    }

    void SetRandomPixels()
    {
        for(int i = 0; i < DIM*DIM*DIM;i++)
        {
            pixels3D[i] = new Color(Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f),1f);
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
        tex3D.SetPixels(pixels3D);
        tex3D.Apply();
    }


}
