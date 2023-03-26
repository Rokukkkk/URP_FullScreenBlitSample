using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RayMarchingRenderFeature : ScriptableRendererFeature
{
    private sealed class Pass : ScriptableRenderPass
    {
        ProfilingSampler m_ProfilingSampler = new ProfilingSampler("RayMarching Pass");
        Material m_Material;
        RTHandle m_CameraColorTarget;

        public Pass(Material material)
        {
            m_Material = material;
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        public void SetTarget(RTHandle colorHandle)
        {
            m_CameraColorTarget = colorHandle;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            ConfigureTarget(m_CameraColorTarget);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType != CameraType.Game) return;
            if (m_Material == null) return;

            var stack = VolumeManager.instance.stack;
            var param = stack.GetComponent<RayMarching>();
            if (!param.IsActive()) return;

            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, m_ProfilingSampler))
            {
                m_Material.SetColor("_BaseColor", param.baseColor.value);
                Blitter.BlitCameraTexture(cmd, m_CameraColorTarget, m_CameraColorTarget, m_Material, 0);
            }

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            CommandBufferPool.Release(cmd);
        }
    }

    Pass m_RenderPass;
    Shader m_Shader;
    Material m_Material;

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType != CameraType.Game) return;
        renderer.EnqueuePass(m_RenderPass);
    }

    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType != CameraType.Game) return;
        // Calling ConfigureInput with the ScriptableRenderPassInput.Color argument
        // ensures that the opaque texture is available to the Render Pass.
        m_RenderPass.ConfigureInput(ScriptableRenderPassInput.Color);
        m_RenderPass.SetTarget(renderer.cameraColorTargetHandle);
    }

    public override void Create()
    {
        m_Shader = Shader.Find("Custom/RayMarching");
        m_Material = CoreUtils.CreateEngineMaterial(m_Shader);
        m_RenderPass = new Pass(m_Material);
    }

    protected override void Dispose(bool disposing)
    {
        CoreUtils.Destroy(m_Material);
    }
}
