using UnityEngine;

public class ParticleEffect : MonoBehaviour
{
    public ComputeShader computeShader;
    public Material material;

    ComputeBuffer mParticleDataBuffer;
    const int mParticleCount = 20000;
    int kernelId;

    struct ParticleData
    {
        public Vector3 pos;
        public Color color;
    }

    void Start()
    {
        //struct中一共7个float，size=28
        mParticleDataBuffer = new ComputeBuffer(mParticleCount, 28);
        ParticleData[] particleDatas = new ParticleData[mParticleCount];
        mParticleDataBuffer.SetData(particleDatas);
        kernelId = computeShader.FindKernel("UpdateParticle");
    }

    void Update()
    {
        computeShader.SetBuffer(kernelId, "ParticleBuffer", mParticleDataBuffer);
        computeShader.SetFloat("Time", Time.time);
        // 简单一维寻址， 只需要保证kernal为128或者64的倍数即可。
        computeShader.Dispatch(kernelId, mParticleCount / 64, 1, 1);
        material.SetBuffer("_particleDataBuffer", mParticleDataBuffer);
    }

    void OnRenderObject()
    {
        material.SetPass(0);
        Graphics.DrawProceduralNow(MeshTopology.Points, mParticleCount);
    }

    void OnDestroy()
    {
        mParticleDataBuffer.Release();
        mParticleDataBuffer = null;
    }
}