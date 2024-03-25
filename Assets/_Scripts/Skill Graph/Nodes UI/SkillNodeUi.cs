using UnityEngine.UI;
using XNode;

public class SkillNodeUi : UiNodeBase
{
    public Dropdown skillDropdown;
    public Dropdown tierDropdown;

    private SkillNode _skillNode;

    public override void Start()
    {
        base.Start();
        _skillNode = node as SkillNode;
        
        skillDropdown.onValueChanged.AddListener(OnChangeSkill);
        skillDropdown.onValueChanged.AddListener(OnChangeTier);
        UpdateGUI();
    }

    public override void UpdateGUI()
    {
        NodePort portA = node.GetInputPort("a");
        NodePort portB = node.GetInputPort("b");
        
        // valA.text = mathNode.a.ToString();
        // valB.text = mathNode.b.ToString();
        // dropDown.value = (int)mathNode.mathType;
    }

    private void OnChangeValA(string val)
    {
        // mathNode.a = float.Parse(valA.text);
    }

    private void OnChangeValB(string val)
    {
        // mathNode.b = float.Parse(valB.text);
    }

    private void OnChangeSkill(int val)
    {
        // mathNode.mathType = (SkillNode.MathType)val;
    }
    
    private void OnChangeTier(int val)
    {
        // mathNode.mathType = (SkillNode.MathType)val;
    }
}