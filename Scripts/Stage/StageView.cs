using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

public class StageView : MonoBehaviour
{
    [SerializeField] private GameObject sStageCellPrefab;
    [SerializeField] private StageManager sStageManager;
    [SerializeField] private GameObject sContents;
    [SerializeField] private Convert3D sConvert3D;
    [SerializeField] private GameObject sGameView;
    [SerializeField] private GameObject sInitView;
    [SerializeField] private GameObject sOptionView;
    [SerializeField] private GameObject sStageLevelView;
    [SerializeField] private GameObject sItemView;
    [SerializeField] private Text sCurrentStarNum;
    [SerializeField] private Image sCurrentStageNum;






    // Start is called before the first frame update
    void Start()
    {
        //MakeStageCells();
    }

    private void OnEnable()
    {
        DestroyStageCells();
        MakeStageCells();

        //현재 스테이지의 별의 총개수 나타내기
        sCurrentStarNum.text = sStageManager.CurrentStarNum.ToString();

        sCurrentStageNum.sprite = Resources.Load<Sprite>($"StageLevelImages/Image_pink/txt/{sStageManager.CurrentStageNum}");

    }


    public void MakeStageCells()
    {
        int TotalStar = 0;
        foreach (StageData sd in sStageManager.StageDatas)
        {
            
            GameObject StageCell = Instantiate(sStageCellPrefab) as GameObject;

            string path = string.Format("StageImages/{0}/{1}", sd.CategoryName, sd.ImageName);
            Sprite sprite = Resources.Load<Sprite>(path);
            var stagecellcom = StageCell.GetComponent<StageCell>();
            stagecellcom.SetImage(sprite);
            stagecellcom.SetName(sd.ImageName);
            stagecellcom.convert3D = sConvert3D;
            stagecellcom.categoryname = sd.CategoryName;
            stagecellcom.imagename = sd.ImageName;
            stagecellcom.dropCount = sd.DropColorCount;
            stagecellcom.stageView = this.gameObject;
            
            
                
            StageCell.transform.SetParent(sContents.transform);

            sStageManager.StageStarNumName = $"Stage{sd.StageNum}Cell{sd.ImageName}StarNum";
            TotalStar += sStageManager.StageStarNum;
            if (sStageManager.StageStarNum != 0)
            {
                for(int i = 0; i < sStageManager.StageStarNum; i++)
                {
                    Sprite starSprite =Resources.Load<Sprite>("StageLevelImages/Image_pink/star");
                    stagecellcom.stars[i].sprite = starSprite;
                }
            }

        }
        // 해당 스테이지의 총 획득 별 수
        sStageManager.CurrentStarNum = TotalStar;
        Debug.Log(TotalStar);

        // 스크롤 뷰의 Y위치값을 보정한다.
        Vector2 pos = sContents.GetComponent<RectTransform>().anchoredPosition;
        int count = sStageManager.StageDatas.Count;

        float ypos = count / 2 * 250;
        sContents.GetComponent<RectTransform>().anchoredPosition =  new Vector2(pos.x, - ypos);
    }

    public void DestroyStageCells()
    {
        for (int i = 0; i < sContents.transform.childCount; i++)
        {
            Destroy(sContents.transform.GetChild(i).gameObject);
        }
    }

    public void ShowGameView()
    {

        this.gameObject.SetActive(false);
        sGameView.SetActive(true);

    }

    public void OnClickStageLevelViewBtn()
    {
        SoundManager.Instance.Play_ButtonClick_Effect_Sound();
        this.gameObject.SetActive(false);
        sStageLevelView.SetActive(true);
    }
    public void onClickOptionViewOn()
    {
        SoundManager.Instance.Play_ButtonClick_Effect_Sound();
        if (!sOptionView.activeSelf)
        {
            sOptionView.SetActive(true);
        }
    }

    public void OnClickItemViewOn()
    {
        SoundManager.Instance.Play_ButtonClick_Effect_Sound();
        if (!sItemView.activeSelf)
        {
            sItemView.SetActive(true);
        }

    }
}
