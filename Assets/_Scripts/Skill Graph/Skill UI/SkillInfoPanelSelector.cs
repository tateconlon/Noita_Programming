using UnityEngine;

[RequireComponent(typeof(SkillInfoPanel))]
public class SkillInfoPanelSelector : MonoBehaviour
{
    public void SelectSkill()
    {
        // E.Shop.OnSelectNode.Raise(new E.Shop.SelectNodeParams
        // {
        //     SelectedNode = GetComponent<SkillInfoPanel>().BoundTarget
        // });
    }
}
