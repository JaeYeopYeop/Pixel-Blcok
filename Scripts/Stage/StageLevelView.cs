using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageLevelView : MonoBehaviour
{
    [SerializeField] private GameObject slStageLevelCellPrefab;
    [SerializeField] private GameObject slContent;
    [SerializeField] private StageManager slStageManager;
    [SerializeField] private GameObject slGameView;
    [SerializeField] private GameObject slStageView;
    [SerializeField] private GameObject slInitView;
    [SerializeField] private GameObject slOptionView;

    // Start is called before the first frame update
    void Start()
    {
        MakeStageLevelCell();
    }

    private void MakeStageLevelCell()
    {
        // StageCell을 만들 때 쓰던것을 재활용하여 사용.
        
        foreach (StageData sl in slStageManager.StageLevels) // 데이터가 안 읽히고 있다.
        {
            GameObject StageLevelCell = Instantiate(slStageLevelCellPrefab) as GameObject;

            string path = string.Format("StageLevelImages/Image_pink/{0}/{1}", sl.CategoryName, sl.ImageName);
            Sprite sprite = Resources.Load<Sprite>(path);
            var stageLevelcellcom = StageLevelCell.GetComponent<StageLevelCell>();
            stageLevelcellcom.SetImage(sprite);
            stageLevelcellcom.stagenum = sl.StageNum;
            stageLevelcellcom.categoryname = sl.CategoryName;
            stageLevelcellcom.imagename = sl.ImageName;
            
            stageLevelcellcom.stageLevelView = this.gameObject;


            StageLevelCell.transform.SetParent(slContent.transform);

        }

        // 스크롤 뷰의 Y위치값을 보정한다.
        Vector2 pos = slContent.GetComponent<RectTransform>().anchoredPosition;
        int count = slStageManager.StageLevels.Count;

        float ypos = count / 2 * 250;
        slContent.GetComponent<RectTransform>().anchoredPosition = new Vector2(pos.x, -ypos);

    }

    public void ShowStageView()
    {
        this.gameObject.SetActive(false);
        slStageView.SetActive(true);
    }

    public void OnClickInitViewBtn()
    {
        SoundManager.Instance.Play_ButtonClick_Effect_Sound();
        this.gameObject.SetActive(false);
        slInitView.SetActive(true);
    }

    public void OnClickOptionViewBtn()
    {
        SoundManager.Instance.Play_ButtonClick_Effect_Sound();
        if (!slOptionView.activeSelf)
        {
            slOptionView.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

}
