using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameView : MonoBehaviour
{
    [SerializeField] private GameObject gOptionView;
    [SerializeField] private GameObject gReallyView;
    [SerializeField] private Convert3D gConvert3D;
    [SerializeField] private ItemManager gItemManager;
    [SerializeField] private ScoureManager gScoureManager;

    [SerializeField] public Text scoure;
    [SerializeField] public Text fever;
    [SerializeField] public Button gFeverMagicRodBtn;
    [SerializeField] public Button gStepMagicRodBtn;
    [SerializeField] public Button gRangeMagicBtn;

    private float time;
    // Start is called before the first frame update

    private void OnDisable()
    {
        gScoureManager.StopParticle();
    }
    void OnEnable()
    {
        gStepMagicRodBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>("StageLevelImages/Image_pink/stage");
        gRangeMagicBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>("StageLevelImages/Image_pink/stage");
        gFeverMagicRodBtn.GetComponent<Image>().sprite= Resources.Load<Sprite>("StageLevelImages/Image_pink/stage");
    }

    // Update is called once per frame
    void Update()
    {
        if (gConvert3D.IsPlayingGame)
        {
            scoure.text = ((int)gScoureManager.scoure).ToString();
        }

        if(gConvert3D.IsPlayingGame && (gScoureManager.Isfever||gScoureManager.Ismegafever))
        {
            fever.enabled = true;
            fever.text=((int)gScoureManager.fevertime).ToString();
        }
        else
        {
            fever.enabled = false;
        }


        if (gFeverMagicRodBtn.interactable)
        {
            if (gItemManager.FeverMagicRodCount < 1)
            {
                gFeverMagicRodBtn.interactable = false;
            }
        }
        else
        {
            if (gItemManager.FeverMagicRodCount >= 1)
            {
                gFeverMagicRodBtn.interactable = true;
            }
        }


        if (gStepMagicRodBtn.interactable)
        {
            if (gItemManager.StepMagicRodCount < 1)
            {
                gStepMagicRodBtn.interactable = false;
            }
        }
        else
        {
            if (gItemManager.StepMagicRodCount >= 1)
            {
                gStepMagicRodBtn.interactable = true;
            }
        }

        if (gRangeMagicBtn.interactable)
        {
            if(gItemManager.RangeMatchCount < 1)
            {
                gRangeMagicBtn.interactable = false;
            }
        }
        else
        {
            if(gItemManager.RangeMatchCount >= 1)
            {
                gRangeMagicBtn.interactable= true;
            }
        }

    }

    public void onClickOptionViewOn()
    {
        SoundManager.Instance.Play_ButtonClick_Effect_Sound();
        if (!gOptionView.activeSelf)
        {
            gOptionView.SetActive(true);
            // 만약 시간관련 변수가 있다면 여기서 한 번 멈춰준다.
            // 굳이?
        }
    }

    public void onClickGameViewOff()
    {
        SoundManager.Instance.Play_ButtonClick_Effect_Sound();
        gReallyView.SetActive(true);
        gConvert3D.IsBlockClickOK = false;
    }
}
