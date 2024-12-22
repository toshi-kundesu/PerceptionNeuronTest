// VLiveKit is all Unlicense.
// unlicense: https://unlicense.org/
// this comment & namespace can be removed. you can use this code freely.
// last update: 2024/11/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ShaderGlobalValueSetter : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float _AntiPerspectiveIntensity = 0.0f;
    // _LightIntensityMultiplier
    [Range(0.0f, 10.0f)]
    public float _LightIntensityMultiplier = 1.0f;
    [Range(0.0f, 1.0f)]
    public float _SphereNormalIntensity = 0.5f;

    // Update is called once per frame
    void Update()
    {
        Shader.SetGlobalFloat("_AntiPerspectiveIntensity", _AntiPerspectiveIntensity);
        Shader.SetGlobalFloat("_LightIntensityMultiplier", _LightIntensityMultiplier);
        Shader.SetGlobalFloat("_SphereNormalIntensity", _SphereNormalIntensity);
    }
}