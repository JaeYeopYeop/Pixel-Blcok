using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] private Convert3D _sConvert3D;
    [SerializeField] private StageView _sStageView;
    [SerializeField] private ScoureManager _sScoureManager;

    public List<StageData> StageDatas { set; get; } // 스테이지 데이타 저장용;
    public List<StageData> StageLevels {  set; get; } // 스테이지 레벨 저장
    private int _stageMaxIndex=3;

    public int StageMaxIndex
    {
        get 
        {
            return _stageMaxIndex; 
        }
    }

    private int _currentStarNum;

    public int CurrentStarNum
    {
        set
        {
            PlayerPrefs.SetInt($"PLAYERSTAR{CurrentStageNum}", value);
            _currentStarNum = value;
        }
        get
        {
            _currentStarNum = PlayerPrefs.GetInt($"PLAYERSTAR{CurrentStageNum}", 1);
            return _currentStarNum;
        }
    }

    private int _currentStageNum = 1;   // 스테이지 번호
    public int CurrentStageNum
    {
        set
        {
            PlayerPrefs.SetInt("PLAYSTAGENUM", value);  // 디스크에 저장
            _currentStageNum = value;
        }

        get
        {
            _currentStageNum = PlayerPrefs.GetInt("PLAYSTAGENUM", 1);
            return _currentStageNum;
        }
    }

    public string StageStarNumName { get; set; }

    public int StageStarNum
    {
        set { PlayerPrefs.SetInt(StageStarNumName, value); }
        get { return PlayerPrefs.GetInt(StageStarNumName, 0); }

    }



    // Start is called before the first frame update
    void Start()
    {
        StageDatas = new List<StageData>();
        
        StageLevels = new List<StageData>();
        CsvFileLoad.OnLoadCSV("StageLevels", StageLevels);
         //PlayGame();
    }

    public void StartUp(float scoure)
    {
        CurrentStarNum += 3;
        /*
        if (scoure > 100)
        {
            CurrentStarNum += 3;
        }
        else if (scoure > 50) 
        {
            CurrentStarNum += 2;
        }
        else
        {
            CurrentStarNum += 1;
        }
        */
    }

    public void PlayNextStage()
    {
        
        if (_sScoureManager.scoure > 0)
        {
            StartUp(_sScoureManager.scoure);
            Debug.Log(_sScoureManager.scoure);
        }

        if (CurrentStarNum > 12)
        {
            CurrentStageNum += 1;

        }
        else
        {
            CurrentStageNum = 1;
        }
        _sStageView.gameObject.SetActive(true);

        /*
        _sConvert3D.PlayGame(StageDatas[CurrentStageNum - 1].CategoryName
            , StageDatas[CurrentStageNum - 1].ImageName, StageDatas[CurrentStageNum - 1].DropColorCount);
        */

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
