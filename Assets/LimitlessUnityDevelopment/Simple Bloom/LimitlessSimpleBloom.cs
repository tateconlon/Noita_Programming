using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class LimitlessSimpleBloom : ScriptableRendererFeature
{
	LimitlessSimpleBloomPass BloomPass;
	public RenderPassEvent Event = RenderPassEvent.BeforeRenderingPostProcessing;

	public override void Create()
	{
		BloomPass = new LimitlessSimpleBloomPass(Event);
	}

	public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
	{
#if UNITY_2019 || UNITY_2020
		BloomPass.Setup(renderer.cameraColorTarget);
#else

#endif		
		renderer.EnqueuePass(BloomPass);
	}
	public class LimitlessSimpleBloomPass : ScriptableRenderPass
	{
		static readonly string k_RenderTag = "Render Simple Bloom Effect";

		static readonly int k_BlurAmount = Shader.PropertyToID("BlurAmount");
		static readonly int k_BloomColor = Shader.PropertyToID("BloomColor");
		static readonly int k_BlAmount = Shader.PropertyToID("BloomAmount");
		static readonly int k_Threshold = Shader.PropertyToID("Threshold");

		static readonly int k_blurTemp = Shader.PropertyToID("_BlurTemp");
		static readonly int k_blurTemp1 = Shader.PropertyToID("_BlurTemp2");
		static readonly int k_blurTex = Shader.PropertyToID("_BlurTex");
		static readonly int k_tempCopy = Shader.PropertyToID("_TempCopy");

		RenderTargetIdentifier bloomTemp = new RenderTargetIdentifier(k_blurTemp);
		RenderTargetIdentifier bloomTemp1 = new RenderTargetIdentifier(k_blurTemp1);
		RenderTargetIdentifier bloomTex = new RenderTargetIdentifier(k_blurTex);
		RenderTargetIdentifier tempCopy = new RenderTargetIdentifier(k_tempCopy);

		SimpleBloom m_SimpleBloom;
		Material LimitlessSimpleBloomMaterial;
		RenderTargetIdentifier currentTarget;


		public LimitlessSimpleBloomPass(RenderPassEvent evt)
		{
			renderPassEvent = evt;
			var shader = Shader.Find("Limitless/SimpleBloom");
			if (shader == null)
			{
				Debug.LogError("Shader not found.");
				return;
			}
			LimitlessSimpleBloomMaterial = CoreUtils.CreateEngineMaterial(shader);

		}
#if UNITY_2019 || UNITY_2020

#elif UNITY_2021
		public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
		{
			var renderer = renderingData.cameraData.renderer;
			currentTarget = renderer.cameraColorTarget;
		}
#else
		public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
		{
			var renderer = renderingData.cameraData.renderer;
			currentTarget = renderer.cameraColorTargetHandle;
		}
#endif
		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			if (LimitlessSimpleBloomMaterial == null)
			{
				Debug.LogError("Material not created.");
				return;
			}
			if (m_SimpleBloom == null)
			{
				var stack = VolumeManager.instance.stack;
				m_SimpleBloom = stack.GetComponent<SimpleBloom>();
			}

			if (m_SimpleBloom == null) { return; }
			if (!m_SimpleBloom.IsActive()) { return; }
			int ScrWidth = Screen.width;
			int ScrHeight = Screen.height;
			var opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
			opaqueDesc.depthBufferBits = 0;

			CommandBuffer cmd = CommandBufferPool.Get(k_RenderTag);
			cmd.GetTemporaryRT(k_tempCopy, opaqueDesc, FilterMode.Bilinear);
			cmd.CopyTexture(currentTarget, tempCopy);

			LimitlessSimpleBloomMaterial.SetFloat(k_BlurAmount, m_SimpleBloom.blurAmount.value);
			LimitlessSimpleBloomMaterial.SetFloat(k_Threshold, m_SimpleBloom.bloomThreshold.value);
			LimitlessSimpleBloomMaterial.SetFloat(k_BlAmount, m_SimpleBloom.bloomAmount.value);
			LimitlessSimpleBloomMaterial.SetColor(k_BloomColor, m_SimpleBloom.bloomColor.value);
			
				switch (m_SimpleBloom.BlurPassAmount.value)
				{
					case 1:
						cmd.GetTemporaryRT(k_blurTex, ScrWidth / 2, ScrHeight / 2, 0, FilterMode.Bilinear);
						cmd.Blit(tempCopy, bloomTex, LimitlessSimpleBloomMaterial, 0);
						break;
					case 2:
						cmd.GetTemporaryRT(k_blurTemp, ScrWidth / 4, ScrHeight / 4, 0, FilterMode.Bilinear);
						cmd.GetTemporaryRT(k_blurTex, ScrWidth / 2, ScrHeight / 2, 0, FilterMode.Bilinear);
						cmd.Blit(tempCopy, bloomTemp, LimitlessSimpleBloomMaterial, 0);
						cmd.Blit(bloomTemp, bloomTex, LimitlessSimpleBloomMaterial, 1);
						break;
					case 3:
						cmd.GetTemporaryRT(k_blurTemp, ScrWidth / 8, ScrHeight / 8, 0, FilterMode.Bilinear);
						cmd.GetTemporaryRT(k_blurTex, ScrWidth / 4, ScrHeight / 4, 0, FilterMode.Bilinear);
						cmd.Blit(tempCopy, bloomTex, LimitlessSimpleBloomMaterial, 0);
						cmd.Blit(bloomTex, bloomTemp, LimitlessSimpleBloomMaterial, 1);
						cmd.Blit(bloomTemp, bloomTex, LimitlessSimpleBloomMaterial, 1);
						break;
					case 4:
					cmd.GetTemporaryRT(k_blurTemp1, ScrWidth / 16, ScrHeight / 16, 0, FilterMode.Bilinear);
					cmd.GetTemporaryRT(k_blurTemp, ScrWidth / 8, ScrHeight / 8, 0, FilterMode.Bilinear);
					cmd.GetTemporaryRT(k_blurTex, ScrWidth / 4, ScrHeight / 4, 0, FilterMode.Bilinear);
					cmd.Blit(tempCopy, bloomTex, LimitlessSimpleBloomMaterial, 0);
					cmd.Blit(bloomTex, bloomTemp, LimitlessSimpleBloomMaterial, 1);
					cmd.Blit(bloomTemp, bloomTemp1, LimitlessSimpleBloomMaterial, 1);
					cmd.Blit(bloomTemp1, bloomTemp, LimitlessSimpleBloomMaterial, 1);
					cmd.Blit(bloomTemp, bloomTex, LimitlessSimpleBloomMaterial, 1);
					break;
				}

			cmd.SetGlobalTexture(k_blurTex, bloomTex);
			cmd.Blit(tempCopy, currentTarget, LimitlessSimpleBloomMaterial, 2);

			context.ExecuteCommandBuffer(cmd);
			CommandBufferPool.Release(cmd);
		}

		public void Setup(in RenderTargetIdentifier currentTarget)
		{

			this.currentTarget = currentTarget;
		}
		public override void FrameCleanup(CommandBuffer cmd)
		{
			cmd.ReleaseTemporaryRT(k_blurTex);
			cmd.ReleaseTemporaryRT(k_tempCopy);
			cmd.ReleaseTemporaryRT(k_blurTemp);
			cmd.ReleaseTemporaryRT(k_blurTemp1);
		}
	}
}


