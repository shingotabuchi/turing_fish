#ifndef SHADER_FUNCS
#define SHADER_FUNCS

void AddFishDetails_float(float4 MainTex, float4 FishMask,float4 FishTex,float3 MainNormal,float3 FishNormal,float4 EyeColor, out float4 Out, out float3 OutNormal)
{
    if(FishMask.r*FishMask.g*FishMask.b > 0.9)
    {
        // Out = FishTex;
        Out = EyeColor;
        OutNormal = FishNormal;
    }
    else
    {
        Out = MainTex;
        OutNormal = MainNormal;
    }
}

void Threshold_float(float4 Tex,float thres,out float4 Out)
{
    if(Tex.r < thres) Out = float4(0,0,0,1);
    else Out = Tex;
}

#endif