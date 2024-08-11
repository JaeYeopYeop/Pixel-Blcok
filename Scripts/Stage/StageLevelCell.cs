using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageLevelCell : MonoBehaviour
{
    [SerializeField] private Image sStageImage;
    public GameObject stageLevelView;

    public string categoryname;
    public int stagenum;
    public string imagename;

    private GameObject gameplay;

    public void SetImage(Sprite sprite)
    {
        sStageImage.sprite = sprite;
    }


    public void OnClickLevelCell()
    {
        SoundManager.Instance.Play_ButtonClick_Effect_Sound();
       
        gameplay = GameObject.Find("GamePlay");
        gameplay.GetComponent<StageManager>().CurrentStageNum = stagenum;
        List<StageData> slcStageData = gameplay.GetComponent<StageManager>().StageDatas;
        slcStageData.Clear();
        
        CsvFileLoad.OnLoadCSV($"StageDatas{stagenum}", slcStageData);

        stageLevelView.GetComponent<StageLevelView>().ShowStageView();

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
