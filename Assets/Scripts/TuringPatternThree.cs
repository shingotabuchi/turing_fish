using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class TuringPatternThree : MonoBehaviour
{
    public bool loadData;
    public ComputeShader compute;
    public List<Renderer> renderList = new List<Renderer>();
    public int DIM;
    public Texture2D maskTexture;
    public Texture2D spawnPointTexture;
    Texture2D plotTextureA;
    Texture2D plotTextureB;
    RenderTexture renderTextureA;
    RenderTexture renderTextureB;
    int init,step,plot;
    public float dx,dt,DA,DB;
    float setDA,setDB;
    [Range(0.001f,0.1f)]
    public float feedR,killR;
    float setFeedR,setKillR;
    [Range(0.001f,0.1f)]
    public float feedG,killG;
    float setFeedG,setKillG;
    [Range(0.001f,0.1f)]
    public float feedB,killB;
    float setFeedB,setKillB;
    public int loopCount;
    ComputeBuffer A,B,laplacianWeights;
    public bool initButton;
    public bool saveButton;
    string dataName;
    public VisualEffect particles;
    public Light light;
    public Color fishColor;
    public float fishColorIntensity;
    public float particleColorIntensity;
    public float lightIntensity;
    float[] karlSimWeights = new float[9]
    {
        0.05f,  0.2f,  0.05f,
        0.2f,  -1.0f,  0.2f,
        0.05f,  0.2f,  0.05f
    };
    RenderTexture testTex;
    public bool testBool;
    private void Start() 
    {
        compute = Instantiate(compute);
        plotTextureA = new Texture2D(DIM,DIM);
        plotTextureA.filterMode = FilterMode.Point;
        renderTextureA = new RenderTexture(DIM,DIM,24);
        renderTextureA.enableRandomWrite = true;
        plotTextureB = new Texture2D(DIM,DIM);
        plotTextureB.filterMode = FilterMode.Point;
        renderTextureB = new RenderTexture(DIM,DIM,24);
        renderTextureB.enableRandomWrite = true;
        Color setColor = fishColor * Mathf.Pow(2f,particleColorIntensity);
        particles.SetVector4("Color",setColor);
        setColor = fishColor * Mathf.Pow(2f,fishColorIntensity);
        foreach (var render in renderList)
        {
            render.material.SetTexture("_TextureA", plotTextureA);
            render.material.SetTexture("_TextureB", plotTextureB);
            render.material.SetColor("_ColorB",setColor);
        }
        light.color = fishColor; 
        light.intensity = lightIntensity;
        testTex = new RenderTexture(DIM,DIM,24);

        Texture2D maskTextureCopy = new Texture2D(DIM, DIM);
        Texture2D spawnTextureCopy = new Texture2D(DIM, DIM);
        Graphics.ConvertTexture(maskTexture, maskTextureCopy);
        Graphics.ConvertTexture(spawnPointTexture, spawnTextureCopy);

        A = new ComputeBuffer(DIM*DIM*2,sizeof(float));
        B = new ComputeBuffer(DIM*DIM*2,sizeof(float));
        dataName = "DIM" + DIM.ToString() 
        + "_FeedR" + feedR.ToString() + "_KillR" + killR.ToString()
        + "_FeedG" + feedG.ToString() + "_KillG" + killG.ToString()
        + "_FeedB" + feedB.ToString() + "_KillB" + killB.ToString();
        if (!File.Exists(dataName + "A.txt")) loadData = false;

        if(loadData)
        {
            float[] savedData = new float[DIM*DIM*2];
            string[] savedDataStr = File.ReadAllLines(dataName + "A.txt");
            for (int i = 0; i < DIM*DIM*2; i++)
            {
                savedData[i] = float.Parse(savedDataStr[i]);
            }
            A.SetData(savedData);
            savedDataStr = File.ReadAllLines(dataName + "B.txt");
            for (int i = 0; i < DIM*DIM*2; i++)
            {
                savedData[i] = float.Parse(savedDataStr[i]);
            }
            B.SetData(savedData);
        }
        laplacianWeights = new ComputeBuffer(9,sizeof(float));
        laplacianWeights.SetData(karlSimWeights);
            
        compute.SetFloat("feedR",feedR);
        setFeedR = feedR;
        compute.SetFloat("killR",killR);
        setKillR = killR;
        compute.SetFloat("feedG",feedG);
        setFeedG = feedG;
        compute.SetFloat("killG",killG);
        setKillG = killG;
        compute.SetFloat("feedB",feedB);
        setFeedB = feedB;
        compute.SetFloat("killB",killB);
        setKillB = killB;
        compute.SetFloat("DA",DA);
        setDA = DA;
        compute.SetFloat("DB",DB);
        setDB = DB;
        compute.SetFloat("dx",dx);
        compute.SetFloat("dt",dt);
        compute.SetInt("DIM",DIM);
        compute.SetInt("offsetA",(int)Random.Range(0,int.MaxValue));
        compute.SetInt("offsetB",(int)Random.Range(0,int.MaxValue));
        
        init = compute.FindKernel("Init");
        compute.SetBuffer(init,"A",A);
        compute.SetBuffer(init,"B",B);
        compute.SetTexture(init,"renderTextureA",renderTextureA);
        compute.SetTexture(init,"renderTextureB",renderTextureB);
        compute.SetTexture(init,"spawnTexture",spawnTextureCopy);

        step = compute.FindKernel("Step");
        compute.SetBuffer(step,"A",A);
        compute.SetBuffer(step,"B",B);
        compute.SetBuffer(step,"laplacianWeights",laplacianWeights);
        compute.SetTexture(step,"maskTexture",maskTextureCopy);

        plot = compute.FindKernel("Plot");
        compute.SetBuffer(plot,"A",A);
        compute.SetBuffer(plot,"B",B);
        compute.SetTexture(plot,"renderTextureA",renderTextureA);
        compute.SetTexture(plot,"renderTextureB",renderTextureB);

        if(!loadData) compute.Dispatch(init,(DIM+7)/8,(DIM+7)/8,1);
        else compute.Dispatch(plot,(DIM+7)/8,(DIM+7)/8,1);
        RenderTexture.active = renderTextureA;
        plotTextureA.ReadPixels(new Rect(0, 0, renderTextureA.width, renderTextureA.height), 0, 0);
        plotTextureA.Apply();
        RenderTexture.active = renderTextureB;
        plotTextureB.ReadPixels(new Rect(0, 0, renderTextureB.width, renderTextureB.height), 0, 0);
        plotTextureB.Apply();
    }

    public void Initialize()
    {
        compute.Dispatch(init,(DIM+7)/8,(DIM+7)/8,1);
    }

    public Color GetMaterialColor()
    {
        return fishColor;
    }

    public void SetMaterialColor(Color color)
    {
        fishColor = color;
        Color setColor = fishColor * Mathf.Pow(2f,particleColorIntensity);
        particles.SetVector4("Color",setColor);
        setColor = fishColor * Mathf.Pow(2f,fishColorIntensity);
        foreach (var render in renderList)
        {
            render.material.SetColor("_ColorB",setColor);
        }
        light.color = fishColor; 
        light.intensity = lightIntensity;
    }

    static Texture2D ResizeTexture(Texture2D srcTexture, int newWidth, int newHeight) {
        var resizedTexture = new Texture2D(newWidth, newHeight);
        Graphics.ConvertTexture(srcTexture, resizedTexture);
        return resizedTexture;
    }

    private void Update() 
    {
        if(initButton)
        {
            compute.Dispatch(init,(DIM+7)/8,(DIM+7)/8,1);
            initButton = false;
        }
        if(saveButton)
        {
            SaveData();
            saveButton = false;
        }
        if(setDA != DA)
        {
            compute.SetFloat("DA",DA);
            setDA = DA;
        }
        if(setDB != DB)
        {
            compute.SetFloat("DB",DB);
            setDB = DB;
        }
        if(setFeedR != feedR)
        {
            compute.SetFloat("feedR",feedR);
            setFeedR = feedR;
        }
        if(setKillR != killR)
        {
            compute.SetFloat("killR",killR);
            setKillR = killR;
        }
        if(setFeedG != feedG)
        {
            compute.SetFloat("feedG",feedG);
            setFeedG = feedG;
        }
        if(setKillG != killG)
        {
            compute.SetFloat("killG",killG);
            setKillG = killG;
        }if(setFeedB != feedB)
        {
            compute.SetFloat("feedB",feedB);
            setFeedB = feedB;
        }
        if(setKillB != killB)
        {
            compute.SetFloat("killB",killB);
            setKillB = killB;
        }
        for (int i = 0; i < loopCount; i++)
        {
            compute.Dispatch(step,(DIM+7)/8,(DIM+7)/8,1);
            // compute.Dispatch(plot,(DIM+7)/8,(DIM+7)/8,1);
        }
        compute.Dispatch(plot,(DIM+7)/8,(DIM+7)/8,1);
        if(testBool)
        {
            RenderTexture.active = testTex;
            plotTextureA.ReadPixels(new Rect(0, 0, renderTextureA.width, renderTextureA.height), 0, 0);
            plotTextureA.Apply();
            RenderTexture.active = testTex;
            plotTextureB.ReadPixels(new Rect(0, 0, renderTextureB.width, renderTextureB.height), 0, 0);
            plotTextureB.Apply();
        }
        else
        {
            RenderTexture.active = renderTextureA;
            plotTextureA.ReadPixels(new Rect(0, 0, renderTextureA.width, renderTextureA.height), 0, 0);
            plotTextureA.Apply();
            RenderTexture.active = renderTextureB;
            plotTextureB.ReadPixels(new Rect(0, 0, renderTextureB.width, renderTextureB.height), 0, 0);
            plotTextureB.Apply();
        }
    }

    void SaveData()
    {
        float[] saveData = new float[DIM*DIM*2];
        A.GetData(saveData);
        string metaStr = "DIM" + DIM.ToString() 
        + "_FeedR" + feedR.ToString() + "_KillR" + killR.ToString()
        + "_FeedG" + feedG.ToString() + "_KillG" + killG.ToString()
        + "_FeedB" + feedB.ToString() + "_KillB" + killB.ToString();
        File.WriteAllLines(metaStr + "A.txt", System.Array.ConvertAll(saveData, x => x.ToString()));
        B.GetData(saveData);
        File.WriteAllLines(metaStr + "B.txt", System.Array.ConvertAll(saveData, x => x.ToString()));
    }

    private void OnDestroy() {
        A.Release();
        B.Release();
        laplacianWeights.Release();
    }
}
