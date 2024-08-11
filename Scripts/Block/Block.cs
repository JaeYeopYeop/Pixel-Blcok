using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public enum State
    {
        STOP,   // 정지
        MOVE,   // 움직일려고 하는상태
        MOVING, // 움직이는 상태
        FIXED   // 백보드에 위치해 있는 상태.
    };

    public int col { set; get; }    // 세로열 위치
    public int row { set; get; }    // 가로열 위치

    public Vector3 OriginPosition { set; get; } // 블럭 생성시 위치를 기록
    public Vector3 OriginScale { set; get; }    // 블럭 생성시 스케일
    [SerializeField] private GameObject _sEdge;  // 테두리

    public Color OriginColor { set; get; }  // 블럭 컬러값을 저장 (material의 color를 저장)

    public int BlockNumber { set; get; }    // 블럭의 색깔 번호 저장
    [SerializeField] private TextMesh _sNumberText; // 부여받은 번호를 표시용 Text

    public bool IsMatchCheck { set; get; }  // 체크한 블럭인지 아닌지 확인.

    public string NumberText
    {
        set
        {
            BlockNumber = int.Parse(value);
            _sNumberText.text = value;
        }

        get
        {
            return _sNumberText.text;
        }
    }

    public State CurrentState { set; get; } // 블럭의 현재 상태 값 저장

    private const float CHECKPOSITIONRANGE = 0.1f;

    private ItemManager bItemManger;

    // Start is called before the first frame update
    void Start()
    {
        bItemManger = GameObject.Find("GamePlay").GetComponent<ItemManager>();
    }

    /// <summary>
    /// 인자로 들어온 블럭과 위치가 같은지 체크한다.
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool CheckMatchPosition(GameObject target)
    {
        if((OriginPosition.x - CHECKPOSITIONRANGE) < target.transform.position.x &&
            (OriginPosition.x + CHECKPOSITIONRANGE) > target.transform.position.x &&
            (OriginPosition.y - CHECKPOSITIONRANGE) < target.transform.position.y &&
            (OriginPosition.y + CHECKPOSITIONRANGE) > target.transform.position.y)
        {
            return true;
        }

        return false;
    }

    public bool CheckMatchColor(GameObject target)
    {
        if(OriginColor == target.GetComponent<Block>().OriginColor)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 컬러번호 표지/ 비표시
    /// </summary>
    /// <param name="onOff"></param>
    public void ShowOnOffNumberText(bool onOff)
    {
        _sNumberText.gameObject.SetActive(onOff);
        _sEdge.SetActive(onOff);
    }

    /// <summary>
    /// 전달된 컬럽값으로 텍스트의 칼라 값을 변경한다.
    /// </summary>
    /// <param name="color"></param>
    public void SetNumberTextColor(Color color)
    {
        _sNumberText.color = color;
    }

    public void MatchBlockAnimationStart()
    {
        LeanTween.scale(this.gameObject, new Vector3(1.8f, 1.8f, 1.8f), 0.3f)
            .setEase(LeanTweenType.easeInBounce)
            .setOnComplete(MatchBlockAnimationEnd);
    }

    public void MatchBlockAnimationEnd()
    {
        LeanTween.scale(this.gameObject, Vector3.one, 0.3f)
            .setEase(LeanTweenType.easeInBounce)
            .setOnComplete(() => { SoundManager.Instance.Play_BlockClick_Effect_Sound(); });

    }

    /// <summary>
    /// 인자로 전달된 obj(back보드상의 블럭)의 위치로 클릭된 블럭을 이동시킨다.
    /// </summary>
    /// <param name="obj"></param>
    public void MoveToFixedPosition(GameObject obj)
    {
        obj.SetActive(false);
        obj.GetComponent<Block>().CurrentState = State.FIXED;

        switch (bItemManger.CurrentItem)
        {
            case ItemManager.Item.BASE:
                LeanTween.move(this.gameObject, obj.transform, 0.8f)
                .setEase(LeanTweenType.easeInBack)
                .setOnComplete(MoveToFixedPositionComplete);

                break;

            case ItemManager.Item.ONE:
                LeanTween.move(this.gameObject, obj.transform, 0.8f)
                .setEase(LeanTweenType.easeOutBack)
                .setOnComplete(MoveToFixedPositionComplete);

                break;

            case ItemManager.Item.TWO:
                LeanTween.move(this.gameObject, obj.transform, 1.0f)
                .setEase(LeanTweenType.easeInOutBack)
                .setOnComplete(MoveToFixedPositionComplete);

                break;

            case ItemManager.Item.THREE:
                LeanTween.move(this.gameObject, obj.transform, 1.4f)
                .setEase(LeanTweenType.easeOutBounce)
                .setOnComplete(MoveToFixedPositionComplete);

                break;

            case ItemManager.Item.FOUR:
                LeanTween.move(this.gameObject, obj.transform, 1.4f)
                .setEase(LeanTweenType.easeOutElastic)
                .setOnComplete(MoveToFixedPositionComplete);

                break;

            case ItemManager.Item.FIVE:
                LeanTween.move(this.gameObject, obj.transform, 1.5f)
                .setEase(LeanTweenType.easeOutCirc)
                .setOnComplete(MoveToFixedPositionComplete);

                break;

            default:
                LeanTween.move(this.gameObject, obj.transform, 0.8f)
                .setEase(LeanTweenType.easeInBack)
                .setOnComplete(MoveToFixedPositionComplete);

                break;
        }

        
    }

    /// <summary>
    /// 이동이 완료된 후에 처리.
    /// </summary>
    public void MoveToFixedPositionComplete()
    {
        SoundManager.Instance.Play_BlockClick_Effect_Sound();

        this.gameObject.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
        this.GetComponent<MeshRenderer>().material.renderQueue = 2900;
        this.gameObject.transform.localScale = OriginScale;
        CurrentState = State.FIXED; // 맞은 상태로 설정
        ShowOnOffNumberText(false);
        Destroy(this.gameObject.GetComponent<Rigidbody>());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
