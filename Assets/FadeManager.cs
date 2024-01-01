using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FadeManager : MonoBehaviour
{
    public float whiteRatio = 0f;

    private Material[] _materials;

    void Start()
    {
        Shader targetShader = Shader.Find("Shader Graphs/DisappearShader");
        _materials = Resources.FindObjectsOfTypeAll<Material>()
            .Where(mat => mat.shader == targetShader)
            .ToArray();
    }

    void Update()
    {
        foreach (Material mat in _materials)
        {
            mat.SetFloat("_WhiteRatio", whiteRatio);   
        }
    }
}
