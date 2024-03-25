using Lean.Pool;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    [AddComponentMenu("")]
    [FeedbackHelp("Stop the playback of the MMF_Player if the Renderer visibility condition is met.")]
    [FeedbackPath("Stop Condition/Stop Condition Visibility")]
    public class MMF_StopConditionVisibility : MMF_Pause
    {
        [MMFInspectorGroup("Target Renderer", true, 61, true)]
        public Renderer TargetRenderer;

        [MMFInspectorGroup("Parameters", true, 63)]
        public bool StopIfRendererVisibilityEquals = false;
        [Tooltip("More expensive calculation, but will return correct value even if TargetRenderer hasn't been rendered yet this frame")]
        public bool UseManualVisibilityCalculation = false;
        
        /// use this override to specify the duration of your feedback (don't hesitate to look at other feedbacks for reference)
        public override float FeedbackDuration { get { return 0f; } }
        /// pick a color here for your feedback's inspector
#if UNITY_EDITOR
        public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.LooperStartColor; } }
        public override bool EvaluateRequiresSetup() { return (TargetRenderer == null); }
        public override string RequiredTargetText { get { return TargetRenderer != null ? TargetRenderer.name : "";  } }
        public override string RequiresSetupText { get { return $"This feedback requires that a {nameof(TargetRenderer)} be set to be able to work properly. You can set one below."; } }
#endif
        
        static readonly Plane[] MainCameraFrustumPlanes = new Plane[6];

        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (!Active || !FeedbackTypeAuthorized)
            {
                return;
            }

            if (UseManualVisibilityCalculation)
            {
                // These are used instead of Renderer.isVisible so we can determine visibility before frame is rendered
                // See: https://docs.unity3d.com/ScriptReference/Renderer-isVisible.html
                
                TargetRenderer.ResetBounds();
                
                if (Camera.main.AreBoundsInsideCameraFrustum(TargetRenderer.bounds) == StopIfRendererVisibilityEquals)
                {
                    Owner.StopFeedbacks(true);
                }
                else
                {
                    base.CustomPlayFeedback(position, feedbacksIntensity);
                }
            }
            else
            {
                if (TargetRenderer.isVisible == StopIfRendererVisibilityEquals)
                {
                    Owner.StopFeedbacks(true);
                }
                else
                {
                    base.CustomPlayFeedback(position, feedbacksIntensity);
                }
            }
        }
    }
}