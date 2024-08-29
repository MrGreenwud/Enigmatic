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
            // Попробуем получить параметры отсечения для текущей камеры
            if (!camera.TryGetCullingParameters(out ScriptableCullingParameters cullingParameters))
            {
                // Если параметры не удалось получить, пропускаем эту камеру
                continue;
            }

            // Настройка камеры
            context.SetupCameraProperties(camera);
            
            // Очищаем рендер таргет
            var cmd = CommandBufferPool.Get("Clear Render Target");
            cmd.ClearRenderTarget(true, true, Color.clear);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);

            // Настраиваем параметры фильтрации и отрисовки
            var cullResults = context.Cull(ref cullingParameters);
            var drawSettings = new DrawingSettings(shaderTagId, new SortingSettings(camera));
            var filterSettings = new FilteringSettings(RenderQueueRange.all);

            // Отрисовываем объекты
            context.DrawRenderers(cullResults, ref drawSettings, ref filterSettings);

            context.Submit();
        }
    }
}
