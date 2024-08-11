using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageCell : MonoBehaviour
{
    [SerializeField] private Image sStageImage;
    [SerializeField] private Text sStageName;
    [SerializeField] public Image[] stars;
    public GameObject stageView;
    public Convert3D convert3D;

    public string categoryname;

    public string imagename;

    public int dropCount;


    // Start is called before the first frame update
    void Start()
    {

    }

    public void SetImage(Sprite sprite)
    {
        sStageImage.sprite = sprite;
    }
    public void SetName(string name)
    {
        sStageName.text = name;
    }

    public void OnClickCell()
    {
        SoundManager.Instance.Play_ButtonClick_Effect_Sound();
        stageView.GetComponent<StageView>().ShowGameView();
        convert3D.enabled = true;
        convert3D.IsBlockClickOK=true;
        convert3D.PlayGame(categoryname, imagename, dropCount);
        
        Debug.Log("Click");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
