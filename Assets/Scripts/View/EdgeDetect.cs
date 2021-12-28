using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(EdgeDetectRenderer), PostProcessEvent.AfterStack, "Custom/EdgeDetect")]
public sealed class EdgeDetect : PostProcessEffectSettings
{
    [Range(0f, 1f), Tooltip("Edge detect effect intensity.")]
    public FloatParameter intensity = new FloatParameter { value = 0.5f };
}

public sealed class EdgeDetectRenderer : PostProcessEffectRenderer<EdgeDetect>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/EdgeDetect"));
        sheet.properties.SetFloat("_EdgeDetectIntensity", settings.intensity);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}