using Sirenix.OdinInspector;
using UnityEngine;

public class TagToEnabled : MonoBehaviour
{
    [SerializeField, Required] private Tag _targetTag;
    [SerializeField, Required] private ActorTagSet _targetTagSet;

    private void Awake()
    {
        _targetTagSet.OnChangeCurValue += Run;
    }
    
    private void Start()
    {
        Run(_targetTagSet);
    }
    
    private void OnDestroy()
    {
        _targetTagSet.OnChangeCurValue -= Run;
    }
    
    private void Run(ActorTagSet tagSet)
    {
        bool setActive = tagSet.Matches(_targetTag);
        
        gameObject.SetActive(setActive);
    }
}