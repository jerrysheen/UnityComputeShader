using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestComputeShader : MonoBehaviour
{
    public Text tipsText;
    public RawImage image;

    public Texture2D texture;

    public int BlurRadius;
    private Vector4 TextureSize;

    public ComputeShader shader;
    private RenderTexture destRt, tempRt;

    private int _kernelHandle;
    void Start()
    {
        DoGPU();
    }

    public void DoGPU()
    {
        //tipsText.text = $"Compute Shader support: {SystemInfo.supportsComputeShaders}";
        destRt = new RenderTexture(texture.width, texture.height, 1);
        destRt.enableRandomWrite = true;
        destRt.Create();
        tempRt = new RenderTexture(texture.width, texture.height, 1);
        tempRt.enableRandomWrite = true;
        tempRt.Create();
        image.texture = destRt;
        TextureSize = new Vector4(texture.width, texture.height);

        DoGaussianBlur("GaussianBlurHorizontalMain", texture, tempRt);
        DoGaussianBlur("GaussianBlurVerticalMain", tempRt, destRt);
    }

    private void DoGaussianBlur(string kernelName, Texture input, Texture output)
    {
        _kernelHandle = shader.FindKernel(kernelName);
        shader.GetKernelThreadGroupSizes(_kernelHandle, out uint x, out uint y, out uint z);
        shader.SetTexture(_kernelHandle, "_OutputTexture", output);
        shader.SetTexture(_kernelHandle, "_InputTexture", input);
        shader.SetFloat("_BlurRadius", BlurRadius);
        shader.SetVector("_TextureSize", TextureSize);
        shader.Dispatch(_kernelHandle, Mathf.CeilToInt(destRt.height / x), Mathf.CeilToInt(destRt.height / y), 1);
    }
}