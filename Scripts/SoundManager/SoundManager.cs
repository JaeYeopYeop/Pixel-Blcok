using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    //Effect Sound
    // 0: 버튼 클릭 사운드, 1:블록 클릭, 2: SwapSuccess, 3: SwapFailure
    [SerializeField] private AudioClip[] Effect_Sound;
    [SerializeField] private AudioClip sBackgroundMusic;
    //[SerializeField] private AudioClip sInGameBackgroundMusic;
    //[SerializeField] private AudioClip sGameEndingBackgroundMusic;

    private static SoundManager instance;
    private AudioSource Audio;
    private AudioSource BackGroundMusic;

    // 사운드 on: true, off:false
    private bool _musicOnOff;
    public bool MusicOnOff
    {
        get
        {
            //_musicOnOff = GameDataControl.Instance.MusicOnOff;
            return _musicOnOff;
        }
        set
        {
            if (!value)
            {
                Audio.Stop();   // 배경음악을 멈춘다.
            }

            //GameDataControl.Instance.MusicOnOff = value;
            _musicOnOff = value;
        }
    }

    private bool _soundOnOff = true;
    public bool SoundOnOff
    {
        get
        {
            //_soundOnOff = GameDataControl.Instance.SoundOnOff;
            return _soundOnOff;
        }
        set
        {
            //GameDataControl.Instance.SoundOnOff = value;
            _soundOnOff = value;
        }
    }

    public static SoundManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject projectile = (GameObject)Instantiate((GameObject)Resources.Load("Manager/SoundManager"));
                instance = projectile.GetComponent<SoundManager>();
            }

            return instance;
        }
    }

    void Awake()
    {
        Audio = gameObject.GetComponent<AudioSource>();

        // 저장되어있는 사운드 설정 상태를 가지고 온다.
      //  SoundOnOff = GameDataPlayControl.Instance.SoundOnOff;
     //   MusicOnOff = GameDataPlayControl.Instance.MusicOnOff;
    }

    // 버튼 클릭 사운드
    public void Play_ButtonClick_Effect_Sound()
    {
        // 사운드가 on 상태이면 Play
        if (SoundOnOff)
        {
            Audio.PlayOneShot(Effect_Sound[0]);
        }
    }

    // 블록 슬라이드 사운드
    public void Play_BlockClick_Effect_Sound()
    {
        // 사운드가 on 상태이면 Play
        if (SoundOnOff)
        {
            Audio.PlayOneShot(Effect_Sound[1]);
        }
    }

    // 블럭 맞추기 성공 사운드
    public void Play_SwapSuccess_Effect_Sound()
    {
        // 사운드가 on 상태이면 Play
        if (SoundOnOff)
        {
            Audio.PlayOneShot(Effect_Sound[2]);
        }
    }

    // 블럭 맞추기 실패 사운드
    public void Play_SwapFailure_Effect_Sound()
    {
        // 사운드가 on 상태이면 Play
        if (SoundOnOff)
        {
            Audio.PlayOneShot(Effect_Sound[3]);
        }
    }
    
    // 블럭을 모두 다 맞춘 사운드
    public void Play_Clear_Effect_Sound()
    {
        if (SoundOnOff)
        {
            Audio.PlayOneShot(Effect_Sound[4]);
        }
    }

    // 스테이지 클리어 사운드
    public void Play_StageClear_Effect_Sound()
    {
        // 사운드가 on 상태이면 Play
        if (SoundOnOff)
        {
            Audio.PlayOneShot(Effect_Sound[5]);
        }
    }

    
    // BGM 사운드
    public void Play_Backgorund_Music()
    {

        // 사운드가 on 상태이면 Play
        if (MusicOnOff)
        {
            Audio.clip = sBackgroundMusic;
            Audio.loop = true;

            Audio.Play(0);
        }
        else
        {
            Audio.Stop();
        }
    }

    // 1피버
    public void Play_Combo_Sound()
    {
        if (SoundOnOff)
        {
            Audio.PlayOneShot(Effect_Sound[6]);
        }
    }

    // 2피버
    public void Play_Double_Combo_Sound()
    {
        if (SoundOnOff)
        {
            Audio.PlayOneShot(Effect_Sound[7]);
        }
    }

    /*    // 타이틀 화면 백그라운드 뮤직
        public void Play_IntroBackgorund_Music()
        {
            // 사운드가 on 상태이면 Play
            if (MusicOnOff)
            {
                Audio.clip = sIntroBackgroundMusic;
                Audio.loop = true;

                Audio.Play(0);
            }
            else
            {
                Audio.Stop();
            }
        }

        // 인게임 사운드
        public void Play_InGameBackgorund_Music()
        {
            // 사운드가 on 상태이면 Play
            if (MusicOnOff)
            {
                Audio.clip = sInGameBackgroundMusic;
                Audio.loop = true;

                Audio.Play(0);
            }
            else
            {
                Audio.Stop();
            }
        }

        public void Play_EndingBackgorund_Music()
        {
            // 사운드가 on 상태이면 Play
            if (MusicOnOff)
            {
                Audio.clip = sGameEndingBackgroundMusic;
                Audio.loop = true;

                Audio.Play(0);
            }
            else
            {
                Audio.Stop();
            }
        }*/

    public void Stop_BackgroundMusic()
    {
        Audio.Stop();
    }

    private void OnDestroy()
    {
        Effect_Sound = null;
        Resources.UnloadUnusedAssets();
    }
}
