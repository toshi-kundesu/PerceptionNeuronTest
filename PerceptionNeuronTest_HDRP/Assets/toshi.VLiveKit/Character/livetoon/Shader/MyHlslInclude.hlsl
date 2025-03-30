#ifndef MYHLSLINCLUDE_INCLUDED
#define MYHLSLINCLUDE_INCLUDED

void MyFunction_float
(
    float3 A, 
    float B, 
    float2 ScreenPosition, 
    float Depth, 
    float4x4 unity_MatrixInvVP,
    float3 ObjectSpacePos_IN,
    float _AntiPerspectiveIntensity,
    out float3 Out,
    // ObgectSpace
    out float3 OUT_ObjectSpacePos
)
{
    Out = A + B;
    OUT_ObjectSpacePos = ObjectSpacePos_IN;

    // Object空間のものを用意
    // float4 vertex = float4(ObjectSpacePos_IN, 1.0);

    // // いったん、クリップ空間に変更
    // // clipSpacePos = mul(unity_ObjectToWorld, vertex);
    // float4 clipSpacePos_HelperFunc = TransformObjectToHClip(vertex);
    // float3 positionOS = OUT_ObjectSpacePos;
    // float4 clipSpacePos_Scratch = mul(GetWorldToHClipMatrix(), mul(GetObjectToWorldMatrix(), float4(positionOS, 1.0)));
    // f
    // float4 positionClip = mul(GetWorldToHClipMatrix(), mul(GetObjectToWorldMatrix(), float4(positionOS, 1.0)));
    // float4 positionWorld = mul(Inverse(GetWorldToHClipMatrix()), positionClip)
    // float4 positionObject = mul(Inverse(GetObjectToWorldMatrix()), positionWorld);

    //
    // float4 positionClipAgain = mul(GetWorldToHClipMatrix(), mul(GetObjectToWorldMatrix(), positionObject));



    // 一回変換作業
    // float4 vet = TransformObjectToHClip(ObjectSpacePos_IN);
    // float4 vet = TransformObjectToHClip(ObjectSpacePos_IN);
    float4 vet = TransformObjectToHClip(ObjectSpacePos_IN);
    

        // // float _AntiPerspectiveIntensity;
    // float centerVSz = mul(UNITY_MATRIX_V, float4(UNITY_MATRIX_M._m03_m13_m23, 1.0)).z;
    float centerVSz = mul(UNITY_MATRIX_V, float4(UNITY_MATRIX_M._m03_m13_m23, 1.0)).z;

    // o.vertex = TransformObjectToHClip(v.vertex);
    // float _AntiPerspectiveIntensity = 1;
    // absを精度よく算出
    float abs_vet_w = abs(vet.w);
    

    vet.xy *= lerp(1.0, abs_vet_w / - centerVSz, _AntiPerspectiveIntensity);



    // 最後に、またObject空間に変換
    // 仮想コード
    // float4 positionClip = mul(GetWorldToHClipMatrix(), mul(GetObjectToWorldMatrix(), float4(positionOS, 1.0)));
    float4 positionClip = vet;
    float4 positionWorld = mul(Inverse(GetWorldToHClipMatrix()), positionClip);
    float4 positionObject = mul(Inverse(GetObjectToWorldMatrix()), positionWorld);

    // OUT_ObjectSpacePos = positionObject.xyz;
    // 出力
    OUT_ObjectSpacePos = positionObject.xyz;
    // memo: Screen2World
    // スクリーン座標とDepthからクリップ座標を作成
    // float4 positionCS = float4(ScreenPosition * 2.0 - 1.0, Depth, 1.0);

    // #if UNITY_UV_STARTS_AT_TOP
    //     positionCS.y = -positionCS.y;
    // #endif

    // // クリップ座標にView Projection変換を適用し、ワールド座標にする    
    // float4 hpositionWS = mul(unity_MatrixInvVP, positionCS);


    // memo: 下記で、視野角変更を実現していた
    // v.normal = normalize(v.normal);
    // float4 vet = UnityObjectToClipPos(v.vertex);

    // // // float _AntiPerspectiveIntensity;
    // float centerVSz = mul(UNITY_MATRIX_V, float4(UNITY_MATRIX_M._m03_m13_m23, 1.0)).z;

    // // o.vertex = TransformObjectToHClip(v.vertex);
    // // float _AntiPerspectiveIntensity = 1;

    // vet.xy *= lerp(1.0, abs(vet.w) / - centerVSz, _AntiPerspectiveIntensity);

    // return InitializeV2F(v, vet, 0);

    // memo: HDRPのサンプル
    // struct appdata
    //         {
    //             float4 vertex : POSITION;
    //             float2 uv : TEXCOORD0;
    //         };

    //         struct v2f
    //         {
    //             float2 uv : TEXCOORD0;
    //             float4 vertex : SV_POSITION;
    //         };

    //         TEXTURE2D(_MainTex);
    //         SAMPLER(sampler_MainTex);

    //         CBUFFER_START(UnityPerMaterial)
    //         float4 _MainTex_ST;
    //         CBUFFER_END

    //         v2f vert (appdata v)
    //         {
    //             v2f o;
    //             o.vertex = TransformObjectToHClip(v.vertex);
    //             o.uv = TRANSFORM_TEX(v.uv, _MainTex);
    //             return o;
    //         }

}

void PassThrough_float(float IN, out float OUT)
{
    OUT = IN;
}

#endif // MYHLSLINCLUDE_INCLUDED