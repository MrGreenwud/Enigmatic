using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Rendering/CustomRenderPipelineAsset")]
public class CustomRenderPipelineAsset : RenderPipelineAsset
{
    protected override RenderPipeline CreatePipeline()
    {
        return new CustomRenderPipeline();
    }
}

public class CustomRenderPipeline : RenderPipeline
{
    private static readonly ShaderTagId shaderTagId = new ShaderTagId("UniversalForward");

    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        foreach (var camera in cameras)
        {
            // ��������� �������� ��������� ��������� ��� ������� ������
            if (!camera.TryGetCullingParameters(out ScriptableCullingParameters cullingParameters))
            {
                // ���� ��������� �� ������� ��������, ���������� ��� ������
                continue;
            }

            // ��������� ������
            context.SetupCameraProperties(camera);
            
            // ������� ������ ������
            var cmd = CommandBufferPool.Get("Clear Render Target");
            cmd.ClearRenderTarget(true, true, Color.clear);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);

            // ����������� ��������� ���������� � ���������
            var cullResults = context.Cull(ref cullingParameters);
            var drawSettings = new DrawingSettings(shaderTagId, new SortingSettings(camera));
            var filterSettings = new FilteringSettings(RenderQueueRange.all);

            // ������������ �������
            context.DrawRenderers(cullResults, ref drawSettings, ref filterSettings);

            context.Submit();
        }
    }
}
