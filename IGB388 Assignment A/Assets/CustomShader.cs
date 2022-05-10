using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CustomShader : MonoBehaviour
{
    public Shader replacementShader; // the shader you want to use with this camera
    public string replacementTag;

    void OnValidate()
    {
        GetComponent<Camera>().SetReplacementShader(replacementShader, replacementTag);
    }
}
