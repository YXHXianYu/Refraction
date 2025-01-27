using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RayBloomFeature : ScriptableRendererFeature {
    class SelectiveBloomPass : ScriptableRenderPass
    {
        private Material bloomMaterial;
        private RenderTargetHandle tempTexture;
        private LayerMask bloomLayerMask;

        public SelectiveBloomPass(Material bloomMaterial, LayerMask bloomLayerMask)
        {
            this.bloomMaterial = bloomMaterial;
            this.bloomLayerMask = bloomLayerMask;
            tempTexture.Init("_TempTexture");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("SelectiveBloom");

            // 获取当前相机的渲染目标
            var source = renderingData.cameraData.renderer.cameraColorTarget;

            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            cmd.GetTemporaryRT(tempTexture.id, opaqueDesc);

            // 使用Bloom材质渲染指定层
            cmd.SetGlobalTexture("_MainTex", source);
            cmd.SetGlobalInt("_BloomLayerMask", bloomLayerMask.value);
            Blit(cmd, source, tempTexture.Identifier(), bloomMaterial);

            // 将结果复制回源纹理
            Blit(cmd, tempTexture.Identifier(), source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(tempTexture.id);
        }
    }

    [System.Serializable]
    public class SelectiveBloomSettings
    {
        public Material bloomMaterial;
        public LayerMask bloomLayerMask;
    }

    public SelectiveBloomSettings settings = new SelectiveBloomSettings();

    private SelectiveBloomPass selectiveBloomPass;

    public override void Create()
    {
        selectiveBloomPass = new SelectiveBloomPass(settings.bloomMaterial, settings.bloomLayerMask)
        {
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(selectiveBloomPass);
    }
}