using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReallyViewManager : MonoBehaviour
{
    [SerializeField] private GameObject rGameView;
    [SerializeField] private GameObject rStageView;
    [SerializeField] private Convert3D rConvert3D;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onClickStageViewYes()
    {
        SoundManager.Instance.Play_ButtonClick_Effect_Sound();
        if (rGameView.activeSelf)
        {
            this.gameObject.SetActive(false);
            rGameView.SetActive(false);
            rStageView.SetActive(true);
            rConvert3D.IsBlockClickOK = true;
            rConvert3D.IsPlayingGame = false;
        }
    }

    public void onClickStageViewNo()
    {
        SoundManager.Instance.Play_ButtonClick_Effect_Sound();

        this.gameObject.SetActive(false);
        rConvert3D.IsBlockClickOK = true;
    }
}
