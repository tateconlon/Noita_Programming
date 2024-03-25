using UnityEngine;

namespace MoreMountains.Feedbacks
{
    [AddComponentMenu("")]
    [FeedbackHelp("Pull a Spring component with a given force multiplied by feedback intensity.")]
    [FeedbackPath("Spring/Pull Spring")]
    public class MMF_SpringPull : MMF_Feedback
    {
        [MMFInspectorGroup("Target Spring", true, 61, true)]
        public Spring TargetSpring;

        [MMFInspectorGroup("Parameters", true, 63)]
        public float PullForce = 1.0f;
        public float IntensityLerpFactor = 1.0f;

        public bool SetStiffnessDamping = false;
        [MMFCondition(nameof(SetStiffnessDamping))] public float NewStiffness = 100;
        [MMFCondition(nameof(SetStiffnessDamping))] public float NewDamping = 10;
        
        /// a static bool used to disable all feedbacks of this type at once
        public static bool FeedbackTypeAuthorized = true;
        /// use this override to specify the duration of your feedback (don't hesitate to look at other feedbacks for reference)
        public override float FeedbackDuration { get { return 0f; } }
        /// pick a color here for your feedback's inspector
#if UNITY_EDITOR
        public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.TransformColor; } }
        public override bool EvaluateRequiresSetup() { return (TargetSpring == null); }
        public override string RequiredTargetText { get { return TargetSpring != null ? TargetSpring.name : "";  } }
        public override string RequiresSetupText { get { return $"This feedback requires that a {nameof(TargetSpring)} be set to be able to work properly. You can set one below."; } }
#endif

        // protected override void CustomInitialization(MMF_Player owner)
        // {
        //     base.CustomInitialization(owner);
        //     // your init code goes here
        // }

        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (!Active || !FeedbackTypeAuthorized)
            {
                return;
            }

            float finalPullForce = PullForce * Mathf.Lerp(1.0f, feedbacksIntensity, IntensityLerpFactor);

            if (!SetStiffnessDamping)
            {
                TargetSpring.Pull(finalPullForce);
            }
            else
            {
                TargetSpring.Pull(finalPullForce, NewStiffness, NewDamping);
            }
        }

        protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1)
        {
            if (!FeedbackTypeAuthorized)
            {
                return;
            }            
            
            TargetSpring.Clear();
        }
    }
}