using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitView : MonoBehaviour
{
    [SerializeField] private GameObject StageView;
    [SerializeField] private GameObject StageLevelView;
    [SerializeField] private GameObject OptionView;
    [SerializeField] private Image MainImage;
    // Start is called before the first frame update
    void Start()
    {
        SoundManager.Instance.MusicOnOff = true;
        SoundManager.Instance.Play_Backgorund_Music();
        int imageNum = Random.Range(0, 3);
        string imageName;
        switch (imageNum)
        {
            case 0:
                imageName = "yellow_bird";
                break;

            case 1:
                imageName = "GamePlug1";
                break;

            case 2:
                imageName = "GamePlug2";
                break;

            default:
                imageName = "yellow_bird";
                break;
        }

        MainImage.sprite = Resources.Load<Sprite>($"Main_Image/{imageName}");
    }

    public void onClickStartButton()
    {
        SoundManager.Instance.Play_ButtonClick_Effect_Sound();
        this.gameObject.SetActive(false);
        StageView.SetActive(true);
    }

    public void onClickOptionViewOn()
    {
        SoundManager.Instance.Play_ButtonClick_Effect_Sound();
        if (!OptionView.activeSelf)
        {
            OptionView.SetActive(true);
        }
    }

    public void onClickStageLevelViewBtn()
    {
        SoundManager.Instance.Play_ButtonClick_Effect_Sound();
        this.gameObject.SetActive(false);
        StageLevelView.SetActive(true);
    }

    public void OnClickExit()
    {
        Application.Quit();
    }
}
