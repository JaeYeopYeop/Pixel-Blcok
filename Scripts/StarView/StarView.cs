using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarView : MonoBehaviour
{
    [SerializeField] private ScoureManager stScoureManager;
    [SerializeField] private StageManager stStageManager;
    [SerializeField] private Image star1;
    [SerializeField] private Image star2;
    [SerializeField] private Image star3;
    [SerializeField] private Text scoure;
    [SerializeField] private Text totalStar;
    [SerializeField] private Text plusStar;

    private Animator star1anime;
    private Animator star2anime;
    private Animator star3anime;

    private void MakeStars()
    {
        if (stScoureManager.scoure > 199)
        {
            Debug.Log("star3");
            star1.enabled = true;
            star2.enabled = true;
            star3.enabled = true;
            star1anime.Play("star1anime");
            star2anime.Play("star2anime");
            star3anime.Play("star3anime");

            if (stStageManager.StageStarNum < 3)
            {
                plusStar.enabled = true;
                plusStar.text = $"+{(3 - stStageManager.StageStarNum)}";
                stStageManager.StageStarNum = 3;

            }

        }
        else if (stScoureManager.scoure > 100)
        {
            Debug.Log("star2");
            star1.enabled = true;
            star2.enabled = true;
            star1anime.Play("star1anime");
            star2anime.Play("star2anime");

            if (stStageManager.StageStarNum < 2)
            {
                plusStar.enabled = true;
                plusStar.text = $"+{(2 - stStageManager.StageStarNum)}";
                stStageManager.StageStarNum = 2;
            }
        }
        else
        {
            Debug.Log("star1");
            star1.enabled = true;
            star1anime.Play("star1anime");

            if (stStageManager.StageStarNum < 1)
            {
                plusStar.enabled = true;
                plusStar.text = "+1";
                stStageManager.StageStarNum = 1;
            }
        }
    }
    private void OnEnable()
    {
        scoure.text = ((int)stScoureManager.scoure).ToString();
        totalStar.text= stStageManager.CurrentStarNum.ToString();
        Invoke("MakeStars", 1f);
    }

    private void OnDisable()
    {
        star1.enabled = false;
        star2.enabled = false;
        star3.enabled = false;
        plusStar.enabled= false;
    }
    // Start is called before the first frame update
    void Start()
    {
        star1.enabled = false;
        star2.enabled = false;
        star3.enabled = false;
        plusStar.enabled= false;
        star1anime=star1.GetComponent<Animator>();
        star2anime=star2.GetComponent<Animator>();
        star3anime=star3.GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
