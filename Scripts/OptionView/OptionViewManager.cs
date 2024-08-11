using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionViewManager : MonoBehaviour
{

    [SerializeField] private GameObject oOptionView;
    [SerializeField] public Button oBackSoundOnOffBtn;
    [SerializeField] public Button oEffectSoundOnOffBtn;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void onClickoBsoundOnOffBtn()
    {   
        SoundManager.Instance.Play_ButtonClick_Effect_Sound();
        SoundManager.Instance.MusicOnOff=!SoundManager.Instance.MusicOnOff;
        SoundManager.Instance.Play_Backgorund_Music();
        if (SoundManager.Instance.MusicOnOff)
        {
            oBackSoundOnOffBtn.image.color = Color.white;
        }
        else
        {
            oBackSoundOnOffBtn.image.color = Color.gray;
        }
    }

    public void onClickoEffectSoundOnOffBtn()
    {
        SoundManager.Instance.SoundOnOff=!SoundManager.Instance.SoundOnOff;
        SoundManager.Instance.Play_ButtonClick_Effect_Sound();
        if (SoundManager.Instance.SoundOnOff)
        {
            oEffectSoundOnOffBtn.image.color = Color.white;
        }
        else
        {
            oEffectSoundOnOffBtn.image.color = Color.gray;
        }

    }

    public void onClickoOptionViewOff()
    {
        SoundManager.Instance.Play_ButtonClick_Effect_Sound();
        if (oOptionView.activeSelf)
        {
            oOptionView.SetActive(false);
            //���� GameView���� ����� �Դµ� �ð����� ������ �ִٸ� �ٽ� �帣���� �ٲ��ش�.
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
