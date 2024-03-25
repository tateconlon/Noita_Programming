using System;
using Coffee.UIEffects;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    [AddComponentMenu("")]
    [FeedbackHelp("Show or hide a UITransitionEffect.")]
    [FeedbackPath("UIEffect/UI Transition Effect")]
    public class MMF_UITransitionEffect : MMF_Feedback
    {
        [MMFInspectorGroup("Target Transition", true, 61, true)]
        public UITransitionEffect TargetTransition;

        [MMFInspectorGroup("Parameters", true, 63)]
        public TransitionMode Mode = TransitionMode.Show;
        public bool Reset = true;
        
        public enum TransitionMode
        {
            Show,
            Hide
        }
        
        /// a static bool used to disable all feedbacks of this type at once
        public static bool FeedbackTypeAuthorized = true;
        /// use this override to specify the duration of your feedback (don't hesitate to look at other feedbacks for reference)
        public override float FeedbackDuration => TargetTransition != null ? TargetTransition.effectPlayer.duration : 0f;
        
        /// pick a color here for your feedback's inspector
#if UNITY_EDITOR
        public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.UIColor; } }
        public override bool EvaluateRequiresSetup() { return (TargetTransition == null); }
        public override string RequiredTargetText { get { return TargetTransition != null ? TargetTransition.name : "";  } }
        public override string RequiresSetupText { get { return $"This feedback requires that a {nameof(TargetTransition)} be set to be able to work properly. You can set one below."; } }
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

            switch (Mode)
            {
                case TransitionMode.Show:
                    TargetTransition.Show(Reset);
                    break;
                case TransitionMode.Hide:
                    TargetTransition.Hide(Reset);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        // protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1)
        // {
        //     if (!FeedbackTypeAuthorized)
        //     {
        //         return;
        //     }            
        //     
        //     // Add your code here
        // }
    }
}