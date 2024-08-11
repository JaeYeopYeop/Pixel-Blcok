using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCubes : MonoBehaviour
{
    [SerializeField] private StageManager _sStageManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // 애니메이션 End시 호출
    // Unity Animation에서 호출
    public void ClearAnimationEnd()
    {
        //_sStageManager.Scoure
        Debug.Log("Clear Animation End");
        //_sStageManager.PlayNextStage();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
