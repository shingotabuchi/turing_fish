using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LBMCompute : MonoBehaviour
{
    public bool speedMode;
    public Image plotImage;
    public int DIM;
    public float pr = 0.71f;
    public float ra =   10000.0f;
    public float tauf = 0.8f;
    public float u0 = 0.01f;
    public int loopCount = 1;

    public float minTemp = 0f;
    public float maxTemp = 1f;

    public float minSpeed = 0f;
    public float maxSpeed = 1f;
    public bool debugMode = false;
    public bool debugFrame = false;

    Texture2D plotTexture;
    RenderTexture renderTexture;
    int init,collisions,streaming,boundaries,plotTemperature,plotSpeed;
    ComputeBuffer uv,f,g;
    public ComputeShader compute;

    float umax, umin, tmp, u2, nu, chi, norm, taug, rbetag, h;
    // Start is called before the first frame update
    void Start()
    {
        plotTexture = new Texture2D(DIM,DIM);
        plotTexture.filterMode = FilterMode.Point;
        plotImage.sprite = Sprite.Create(plotTexture, new Rect(0,0,DIM,DIM),UnityEngine.Vector2.zero);
        renderTexture = new RenderTexture(DIM,DIM,24);
        renderTexture.enableRandomWrite = true;
        h = (float)(DIM-1 - 1);
        nu = (tauf - 0.5f)/3.0f;
        chi = nu/pr;
        taug = 3.0f*chi + 0.5f;
        rbetag = ra*nu*chi/h/h/h;

        uv = new ComputeBuffer(DIM*DIM*2,sizeof(float));
        f = new ComputeBuffer(9*DIM*DIM*2,sizeof(float));
        g = new ComputeBuffer(5*DIM*DIM*2,sizeof(float));

        compute.SetInt("DIM",DIM);
        compute.SetFloat("minTemp",minTemp);
        compute.SetFloat("maxTemp",maxTemp);
        compute.SetFloat("minSpeed",minSpeed);
        compute.SetFloat("maxSpeed",maxSpeed);
        compute.SetFloat("u0",u0);
        compute.SetFloat("rbetag",rbetag);
        compute.SetFloat("taug",taug);
        compute.SetFloat("tauf",tauf);

        init = compute.FindKernel("Init");
        compute.SetBuffer(init,"uv",uv);
        compute.SetBuffer(init,"f",f);
        compute.SetBuffer(init,"g",g);

        plotTemperature = compute.FindKernel("PlotTemperature");
        compute.SetBuffer(plotTemperature,"g",g);
        compute.SetTexture(plotTemperature,"renderTexture",renderTexture);

        plotSpeed = compute.FindKernel("PlotSpeed");
        compute.SetBuffer(plotSpeed,"uv",uv);
        compute.SetTexture(plotSpeed,"renderTexture",renderTexture);

        collisions = compute.FindKernel("Collisions");
        compute.SetBuffer(collisions,"f",f);
        compute.SetBuffer(collisions,"g",g);
        compute.SetBuffer(collisions,"uv",uv);

        streaming = compute.FindKernel("Streaming");
        compute.SetBuffer(streaming,"f",f);
        compute.SetBuffer(streaming,"g",g);

        boundaries = compute.FindKernel("Boundaries");
        compute.SetBuffer(boundaries,"f",f);
        compute.SetBuffer(boundaries,"g",g);

        compute.Dispatch(init,(DIM+7)/8,(DIM+7)/8,1);
        // compute.Dispatch(plotTemperature,(DIM+7)/8,(DIM+7)/8,1);

        // RenderTexture.active = renderTexture;
        // plotTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        // plotTexture.Apply();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(debugMode)
        {
            if(debugFrame)
            {
                compute.Dispatch(collisions,(DIM+7)/8,(DIM+7)/8,1);
                compute.Dispatch(streaming,(DIM+7)/8,(DIM+7)/8,1);
                compute.Dispatch(boundaries,(DIM+63)/64,1,1);
                debugFrame = false;
            }
        }
        else
        {
            for (int i = 0; i < loopCount; i++)
            {
                compute.Dispatch(collisions,(DIM+7)/8,(DIM+7)/8,1);
                compute.Dispatch(streaming,(DIM+7)/8,(DIM+7)/8,1);
                compute.Dispatch(boundaries,(DIM+63)/64,1,1);
            }
        }
        
        // compute.Dispatch(plotTemperature,(DIM+7)/8,(DIM+7)/8,1);
        if(speedMode) compute.Dispatch(plotSpeed,(DIM+7)/8,(DIM+7)/8,1);
        else compute.Dispatch(plotTemperature,(DIM+7)/8,(DIM+7)/8,1);

        RenderTexture.active = renderTexture;
        plotTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        plotTexture.Apply();
        
    }

    private void OnValidate() 
    {
        nu = (tauf - 0.5f)/3.0f;
        chi = nu/pr;
        taug = 3.0f*chi + 0.5f;
        rbetag = ra*nu*chi/h/h/h;

        compute.SetFloat("minTemp",minTemp);
        compute.SetFloat("maxTemp",maxTemp);
        compute.SetFloat("minSpeed",minSpeed);
        compute.SetFloat("maxSpeed",maxSpeed);
        compute.SetFloat("u0",u0);
        compute.SetFloat("rbetag",rbetag);
        compute.SetFloat("taug",taug);
        compute.SetFloat("tauf",tauf);
    }
}
