using NodeCanvas.Framework;
using ParadoxNotion.Design;
using ThirteenPixels.Soda;

namespace NodeCanvas.Tasks.Conditions
{
    [Category("✫ Blackboard")]
    [Description("Check whether or not the value of a GlobalGameObject is assigned")]
    public class CheckGlobalGameObject : ConditionTask
    {
        [BlackboardOnly]
        public BBParameter<GlobalGameObject> variable;
        
        protected override string info => $"{variable}.value != null";

        protected override bool OnCheck() {
            return variable.value.value != null;
        }
    }
}