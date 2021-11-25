using UnityEngine;
using UnityEngine.Rendering;

namespace CustomRP.Runtime
{
    [CreateAssetMenu(menuName = "Rendering/Custom Render Pipeline")]
    public class CustomRenderPipelineAsset : RenderPipelineAsset
    {
        [SerializeField] private bool useGPUInstansing;
        [SerializeField] private bool useSPPBatching;
        [SerializeField] private bool useDynamicBatching;
        [SerializeField] ShadowSettings shadows = default;
        
        protected override RenderPipeline CreatePipeline()
        {
            return new CustomRenderPipeline(useDynamicBatching, useGPUInstansing, useSPPBatching, shadows);
        }
    }
}