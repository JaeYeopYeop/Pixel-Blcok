using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

using Color = UnityEngine.Color;
public enum Direction
{
    LEFT,
    RIGHT,
    UP,
    DOWN
};

public class Convert3D : MonoBehaviour
{
    [SerializeField] private GameObject _sCubePrefab;   // 큐브 Prefab
    [SerializeField] private GameObject _sMainObject;   // 큐브 부모오브젝트
    [SerializeField] private GameObject _sBackObject;   // 백 큐브 부모오브젝트

    [SerializeField] private Material _sCubeMaterial;   // 큐브 오브젝트의 Material
    [SerializeField] private Material _sBackMaterial;   // 백 보드 큐브용 Material

    private GameObject[,] _board = null;    // 게임보드
    private GameObject[,] _backBoard = null;    // 백보드

    private float xBaseWidth = 500; // 이미지가 모두 정방형이라고 간주한다.
    private float yBaseHeight = 500;

    [SerializeField] private GameObject _sRightFence;
    [SerializeField] private GameObject _sLeftFence;
    [SerializeField] private GameObject _sGameView;
    [SerializeField] private GameObject _sStageView;
    [SerializeField] private GameObject _sStarView;
    [SerializeField] private GameObject _sReallyView;
    
    [SerializeField] private ItemManager _sItemManager;
    [SerializeField] private ScoureManager _sScoureManager;
    [SerializeField] private StageManager _sStageManager;
    
    // Color값을 저장할 list 생성
    private List<Color> _colorList = new List<Color>(); // 유니크한 color 테이블 만듦
    private List<Block> _backBlockList = new List<Block>(); // 블럭을 저장한 컬러값을 구별하는 목적으로 생성.
    private Dictionary<string,int> _colorscount = new Dictionary<string,int>(); // 사용된 각color의 개수 저장

    /// <summary>
    ///  20211030
    /// </summary>
    private GameObject _target = null;  // 마우스 클릭시 선택된 블럭
    private bool _isMouseDrag = false;  // 마우스가 드래그 상태인지 체크
    private Vector3 _screenPosition;
    private Vector3 _offset;

    private const float CLICKYOFFEST = 1.3f;
    private const float CLICKZOFFSET = -0.5f;

    private int _currentColumn = 0; // 세로열 갯수
    private int _currentRow = 0; // 가로열 갯수


    // 아이템 처리용 변수(임시)
    private bool _isStepMatch = false;
    private bool _isMagicRod = false;

    private List<GameObject> _matchObjects = new List<GameObject>();    // RangeMatchItem 함수에 사용

    private bool _isAllMadeToggle = false;   // 보드상의 블럭을 모두 맞췄는지 확인하는 변수

    private List<StageData> _stageDatas = new List<StageData>();

    // 랜덤함수 생성
    private System.Random _rng = new System.Random();

    private int _currentToBeMatchedBlockCount = 0;  // 맞춰야 하는 블럭 갯수

    private bool _isBlockClickOK = false;

    public bool IsBlockClickOK { set { _isBlockClickOK = value; } get { return _isBlockClickOK; } }

    private bool _isPlayingGame = false;

    public bool IsPlayingGame { set { _isPlayingGame = value; } get {return _isPlayingGame; } }

    // Start is called before the first frame update
    void Start()
    {        

        // 화면의 사이즈에 맞춰서 Left Fence와 Right Fence의 위치를 변경한다.
        Vector3 p1 = Camera.main.ScreenToWorldPoint(new Vector3(0f, Screen.height / 2.0f, 10.0f));

        Vector3 RightFencePosition = _sRightFence.transform.position;
        Vector3 LeftFencePosition = _sLeftFence.transform.position;

        _sRightFence.transform.position = new Vector3(p1.x, RightFencePosition.y, 0.0f);
        _sLeftFence.transform.position = new Vector3(-p1.x, LeftFencePosition.y, 0.0f);

        //PlayGame("char", "blue_bird");
    }


    // List에 들어가는 값을 섞는 함수
    private void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;

        while(n > 1)
        {
            n--;

            int k = _rng.Next(n + 1);

            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }

    }

    /// <summary>
    /// 이미지 파일을 리소스 폴더 로딩
    /// </summary>
    /// <param name="categoryName"></param>
    /// <param name="imageName"></param>
    public void PlayGame(string categoryName, string imageName, int dropColorCount)
    {
        //전 게임에서 중도 퇴장 가능성이 있기 때문에 한 번 비워주는 보험용.
        _colorscount.Clear();
        
        Texture2D texture = null;
        string PATH = "StageImages/" + categoryName + "/" + imageName;
        texture = Resources.Load(PATH, typeof(Texture2D)) as Texture2D;

        Build2DConvert3D(texture, dropColorCount);
        IsPlayingGame = true;
        _sScoureManager.scoure = 101f;
        _sScoureManager.fevertime = 11f;
        _sScoureManager.Isfever = false;
        _sScoureManager.Ismegafever = false;
        _sItemManager.IsStepFever = false;
        _sItemManager.IsFeverMagicRod = false;
        _sItemManager.IsRangeMatch = false;

        Debug.Log(_sScoureManager.Isfever);
        Debug.Log(_sScoureManager.Ismegafever);

        _sStageManager.StageStarNumName= $"Stage{_sStageManager.CurrentStageNum}Cell{imageName}StarNum";
        Debug.Log(_sStageManager.StageStarNumName);

        if(_sStageManager.CurrentStarNum >= (int)((_sStageManager.StageDatas.Count * 3) * 0.2))
            _sItemManager.StepMagicRodCount = 1;
        else
            _sItemManager.StepMagicRodCount = 0;

        if (_sStageManager.CurrentStarNum >= ((_sStageManager.StageDatas.Count * 3) * 0.4))
            _sItemManager.RangeMatchCount = 1;
        else
            _sItemManager.RangeMatchCount = 0;

        if (_sStageManager.CurrentStarNum >= ((_sStageManager.StageDatas.Count * 3) * 0.6))
            _sItemManager.FeverMagicRodCount = 1;
        else
            _sItemManager.FeverMagicRodCount = 0;

    }


    // 받은 픽셀의 color들을 참조가 가능한 상태로 만들어준다.
    private List<Color32> GenerateColors(Color32[] colorBuffer, int height, int width)
    {
        List<Color32> vertexColors = new List<Color32>(height * width);

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Color32 c = colorBuffer[j + i * width];

                vertexColors.Add(c);
            }
        }

        return vertexColors;
    }

    /// <summary>
    /// 로딩한 이미지에서 픽셀 정보를 가지고 온다.
    /// </summary>
    /// <param name="texture"></param>
    private void Build2DConvert3D(Texture2D texture, int dropColorCount)
    {
        texture.filterMode = FilterMode.Point;

        var textureFormat = texture.format;

        if (textureFormat != TextureFormat.RGBA32)
        {
            Debug.Log("32비트 컬러를 사용");
        }

        int height = texture.height;
        int width = texture.width;

        // 이미지의 가로/세로의 값을 가져온다.
        _currentColumn = height;
        _currentRow = width;


        Color32[] colorBuffer = texture.GetPixels32();

        var TextureColors = GenerateColors(colorBuffer, height, width);

        Create3DCube(TextureColors, height, width, dropColorCount);

    }


    /// <summary>
    /// 게임 보드를 구성한다.
    /// </summary>
    /// <param name="colors"></param>
    /// <param name="height"></param>
    /// <param name="width"></param>
    private void Create3DCube(List<Color32> colors, int height, int width, int dropColorCount = 0)
    {
        // 초기화
        if(_board != null)
        {
            foreach(GameObject obj in _board)   // 게임 보드
            {
                if(obj != null)
                {
                    Destroy(obj);
                }
            }

            foreach(GameObject obj in _backBoard)
            {
                if(obj != null)
                {
                    Destroy(obj);
                }
            }

            _board = null;
            _backBoard = null;

            _colorList.Clear();
            _backBlockList.Clear();

            _sMainObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            _sMainObject.transform.position = new Vector3(0.0f, 0.0f, 0.0f);

            _sBackObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            _sBackObject.transform.position = new Vector3(0.0f, 0.0f, 0.0f);

            _isAllMadeToggle = false;
        }

        // 가로/세로 갯수의 게임보들 만든다.
        _board = new GameObject[height, width];
        _backBoard = new GameObject[height, width];

        // 화면사이즈를 계산하기 위한값
        // 이미지파일에서 픽셀의 색깔값이 투명인 픽셀은 제외하고 출력되는
        // 가로열 갯수 와 세로열 갯수를 계산하기 위한 변수
        int columnMin = height; // 세로 최소값
        int columnMax = 0;  // 세로 최대값
        int rowMin = width; // 가로 최소값
        int rowMax = 0;     // 가로 최대값

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Color32 color = colors[i * width + j];

                // 픽셀값이 투명값이 아니면 
                if (color.a != 0.0f)
                {
                    GameObject obj = Instantiate(_sCubePrefab) as GameObject;
                    obj.name = $"Cube[{i}, {j}]";

                    obj.GetComponent<MeshRenderer>().material = _sCubeMaterial;
                    obj.GetComponent<MeshRenderer>().material.color = color;

                    obj.GetComponent<Block>().OriginColor = color;

                    obj.GetComponent<Block>().col = i;
                    obj.GetComponent<Block>().row = j;
                    obj.tag = "CubeBlock";  // 블럭에 태깅을 한다.

                    obj.transform.SetParent(_sMainObject.transform);    // 블럭의 부모오브젝트를 설정

                    obj.transform.position = new Vector3(j, i, 0.0f);

                    // Color 리스트를 만든다.
                    _colorList.Add(color);

                    // 보드에 추가한다.
                    _board[i, j] = obj;

                    // 가로/세로 블럭의 갯수를 계산
                    if (columnMin > i)
                    {
                        columnMin = i;
                    }

                    if (columnMax < i)
                    {
                        columnMax = i;
                    }

                    if (rowMin > j)
                    {
                        rowMin = j;
                    }

                    if (rowMax < j)
                    {
                        rowMax = j;
                    }
                }

                if (color.a != 0.0f)
                {
                    // 백보드 큐브오브젝트 생성
                    GameObject backobj = Instantiate(_sCubePrefab) as GameObject;
                    backobj.name = $"CubeBack[{i}, {j}]";
                    //backobj.name = string.Format("CubeBack[{0}, {1}]", i, j);

                    backobj.GetComponent<MeshRenderer>().material = _sBackMaterial;
                    backobj.GetComponent<MeshRenderer>().material.color = color;

                    backobj.GetComponent<Block>().col = i;  // 블럭의 세로열 값
                    backobj.GetComponent<Block>().row = j;  // 블럭의 가로열 값

                    backobj.GetComponent<Block>().OriginColor = color;

                    Rigidbody body = backobj.GetComponent<Rigidbody>(); // 백큐브오브젝트의 RigidBody 컴포넌트를 삭제
                    Destroy(body);

                    Collider collider = backobj.GetComponent<Collider>();   // 백큐브오브젝트의 Collider 컴포넌트를 삭제
                    Destroy(collider);

                    backobj.transform.parent = _sBackObject.transform;  // 큐브를 _sBackObject의 자식으로 Attach한다.
                    backobj.transform.position = new Vector3(j, i, 0.0f);   // 백큐브의 위치값을 설정


                    _backBlockList.Add(backobj.GetComponent<Block>());
                    _backBoard[i, j] = backobj;
                }
            }
        }

        // 메인오브젝트를 중심으로 생성된 블럭의 중심점을 이동한다.
        float midXvalue = width / 2.0f;
        float midYvalue = height / 2.0f;

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (_board[i, j] != null)
                {
                    Vector3 calposition = _board[i, j].transform.position;

                    // 큐브 보드의 위치를 중심을 기준으로 변경
                    _board[i, j].transform.position =
                        new Vector3(calposition.x - midXvalue + 0.5f, calposition.y - midYvalue + 0.5f, 0.0f);

                    // 백보드의 위치를 중심을 기준으로 변경
                    _backBoard[i, j].transform.position =
                        new Vector3(calposition.x - midXvalue + 0.5f, calposition.y - midYvalue + 0.5f, 0.0f);



                }
            }
        }

        // 화면에 출력비율을 계산한다.
        int calColumnCount = columnMax - columnMin + 1;
        int calRowCount = rowMax - rowMin + 1;

        float xscale = 0.0f;

        if (calColumnCount < calRowCount)
        {
            xscale = xBaseWidth / 100.0f;
            xscale = xscale / calRowCount;
        } else
        {
            xscale = yBaseHeight / 100.0f;
            xscale = xscale / calColumnCount;

        }

        if (xscale > 0.22f)
        {
            xscale = 0.22f;
        }

        _sMainObject.transform.localScale = new Vector3(xscale, xscale, xscale);    // 큐브 보드의 스케일을 변경
        _sBackObject.transform.localScale = new Vector3(xscale, xscale, xscale);    // 백큐브보드의 스케일을 변경

        // 블럭의 스케일 값이 변경된 후에
        // 블럭의 변경된 스케일 값을 Block 컴포넌트에 기록한다.
        for (int i = 0; i < _currentColumn; i++)
        {
            for (int j = 0; j < _currentRow; j++)
            {
                if (_board[i, j] != null)
                {
                    _board[i, j].GetComponent<Block>().OriginScale = _board[i, j].transform.localScale;
                }
            }
        }


        Vector3 mainPosition = _sMainObject.transform.position;

        // CubeBoard의 위치를 상단쪽으로 이동
        _sMainObject.transform.position = new Vector3(mainPosition.x, mainPosition.y + 2.0f, 0.0f); // 큐브보드의 큐브 위치를 중점을 기준으로 변경
        _sBackObject.transform.position = new Vector3(mainPosition.x, mainPosition.y + 2.0f, 1.0f);


        // _colorscount를 통해 각 컬러의 개수 저장.
        for (int i = 0; i < _colorList.Count; i++)
        {
            if (_colorscount.ContainsKey($"{_colorList[i].r}{_colorList[i].g}{_colorList[i].b}"))
            {
                _colorscount[$"{_colorList[i].r}{_colorList[i].g}{_colorList[i].b}"]++;
            }
            else
            {
                _colorscount.Add($"{_colorList[i].r}{_colorList[i].g}{_colorList[i].b}", 1);
            }
        }

        // ColorList에 입력된 컬러값을 중복을 제거한다.
        _colorList = _colorList.Distinct().ToList();

        // 블럭에 넘버링을 한다.
        foreach (var block in _backBlockList)
        {
            int index = _colorList.FindIndex(x => x == block.OriginColor);
            block.BlockNumber = index;
            block.NumberText = index.ToString();

            _board[block.col, block.row].GetComponent<Block>().NumberText = index.ToString();
            _board[block.col, block.row].GetComponent<Block>().BlockNumber = index;

            block.ShowOnOffNumberText(true);
        }

        // 드롭 블럭 기능구현
        List<Color> SelectColorList = null;

        if(dropColorCount > 0)
        {
            Shuffle<Color>(_colorList); // 리스트에 저장된 color값을 섞는다.

            int dropCount = _colorList.Count - dropColorCount;

            // 나중에 dropColorCount를 제어하는 부분이 생기면 지워질 조건문
            if (dropCount <= 0)
            {
                dropCount = _colorList.Count;
            }

            // Drop할 Color를 선별한다.
            SelectColorList = _colorList.GetRange(0, dropCount);

        } else
        {
            SelectColorList = _colorList;
        }


        // OriginPostion를 각 블럭에 기록
        foreach (GameObject obj in _board)
        {
            if (obj != null)
            {
                if(SelectColorList.Exists(x => x == obj.GetComponent<Block>().OriginColor))
                {
                    obj.transform.position =
                        new Vector3(obj.transform.position.x, obj.transform.position.y, -0.001f);
                    obj.GetComponent<Block>().OriginPosition = obj.transform.position;
                    obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                    obj.GetComponent<Rigidbody>().useGravity = true;

                    _backBoard[obj.GetComponent<Block>().col, obj.GetComponent<Block>().row]
                        .GetComponent<Block>().ShowOnOffNumberText(true);

                    _backBoard[obj.GetComponent<Block>().col, obj.GetComponent<Block>().row]
                        .GetComponent<MeshRenderer>().enabled = false;

                    // 맞춰야 하는 블럭의 갯수를 기록
                    _currentToBeMatchedBlockCount++;
                } else
                {
                    // 드롭하지 않을 블럭처리
                    obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, -0.001f);
                    obj.GetComponent<Block>().OriginPosition = obj.transform.position;

                    // RigidBody, Collider 삭제처리
                    Destroy(obj.GetComponent<Rigidbody>());
                    Destroy(obj.GetComponent<Collider>());

                    _backBoard[obj.GetComponent<Block>().col, obj.GetComponent<Block>().row]
                        .GetComponent<Block>().ShowOnOffNumberText(false);

                    _backBoard[obj.GetComponent<Block>().col, obj.GetComponent<Block>().row]
                        .GetComponent<MeshRenderer>().enabled = false;

                    // 해당 블럭에 고정된 블럭 상태 처리
                    obj.GetComponent<Block>().CurrentState = Block.State.FIXED;
                }
                
            }
        }

        foreach (GameObject obj in _backBoard)
        {
            if (obj != null)
            {
                obj.transform.position =
                    new Vector3(obj.transform.position.x, obj.transform.position.y, 0.0f);
                obj.GetComponent<Block>().OriginPosition = obj.transform.position;
            }

        }

       // Invoke("Crash", 1.0f);
    }

    /// <summary>
    /// 블럭의 물리속성을 부여한다.
    /// </summary>
    void Crash()
    {
        foreach (GameObject obj in _board)
        {
            if (obj != null)
            {
                obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, -0.001f);
                obj.GetComponent<Block>().OriginPosition = obj.transform.position;
                obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                obj.GetComponent<Rigidbody>().useGravity = true;

                // 큐브 블럭
                _backBoard[obj.GetComponent<Block>().col, obj.GetComponent<Block>().row]
                    .GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }

    /// <summary>
    /// 백보드에 선택된 블럭의 컬러값을 가진 백보드블럭의
    /// 텍스트 컬러값을 변경한다.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="color"></param>
    private void ChanageBlockTextColor(int index, Color color)
    {
        foreach (GameObject obj in _backBoard) {
            if (obj != null)
            {
                if (obj.GetComponent<Block>().BlockNumber == index)
                {
                    obj.GetComponent<Block>().SetNumberTextColor(color);
                }
            }
        }
    }

    /// <summary>
    /// 해당 컬러의 색이 전부 매칭 되었는지 판단
    /// </summary>
    private bool IsThisColorAllMatched(Color color)
    {
        bool result;

        if (_colorscount[$"{color.r}{color.g}{color.b}"] <= 0)
        {
            result = true;

        }
        else
        {
            result = false;
        }

        return result;
    }

    /// <summary>
    /// 보드상의 블럭이 다 매칭처리가 되었는지 판단
    /// </summary>
    /// <returns></returns>
    private bool IsAllBlockMade()
    {
        for(int i = 0; i < _currentColumn; i++)
        {
            for(int j = 0; j < _currentRow; j++)
            {
                if(_board[i, j] != null)
                {
                    if(_board[i, j].GetComponent<Block>().CurrentState != Block.State.FIXED)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    /// <summary>
    /// 모든블럭이 맞춰졌을때 
    /// </summary>
    private void AllBlockComplete()
    {
        // 파티클 효과 끄기
        _sScoureManager.StopParticle();

        IsPlayingGame = false;
        _sScoureManager.fevertime = 0f;
        ClearAnimation();
        SoundManager.Instance.Play_Clear_Effect_Sound();
        Invoke("StarViewOn", 2.5f);
        Invoke("GameViewOff", 8f);
        // color초기화
        _colorscount.Clear();
        
        
        
    }
    private void StarViewOn()
    {
        _sStarView.SetActive(true);
    }

    private void GameViewOff()
    {
        _sStageView.SetActive(true);
        _sGameView.SetActive(false);
        _sStarView.SetActive(false);
        if (_sReallyView.activeSelf)
        {
            _sReallyView.SetActive(false);
        }
    }

    /// <summary>
    /// 모든 블럭 맞춘 경우 animation처리
    /// </summary>
    private void ClearAnimation()
    {
        float animScale = xBaseWidth / _currentRow;

        animScale /= 100.0f;

        animScale /= 2.0f;

        Vector3 mainScale = new Vector3(animScale, animScale, animScale);

        LeanTween.scale(_sMainObject, mainScale, 0.5f)
            .setEase(LeanTweenType.easeInBack)
            .setOnComplete(ClearAnimationComplete);
    }

    /// <summary>
    /// LeanTween 애니메이션이 끝난 후에 UnityAnimation 처리
    /// </summary>
    public void ClearAnimationComplete()
    {        
        _sMainObject.GetComponent<Animator>().enabled = true;
        _sMainObject.GetComponent<Animator>().SetTrigger("ClearAnimationAnim");        
    }


    /// <summary>
    /// BackBoard상의 큐브블럭과 _target블럭의 위치값과 컬러값을 비교한다.
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    private GameObject IsMatchPositionColorBlock(GameObject target)
    {
        foreach (Block bc in _backBlockList)
        {
            if (bc.CurrentState != Block.State.FIXED && bc.CheckMatchPosition(target) && bc.CheckMatchColor(target))
            {
                return bc.gameObject;
            }
        }

        return null;

    }

    /// <summary>
    /// 블럭을 클릭시 클릭된 블럭이 날아서 보드에 붙도록 처리.
    /// </summary>
    /// <param name="clickObj"></param>
    private void StepBlockMatchedPosition(GameObject clickObj)
    {
        // backboard상에서 fixed되지 않은 블럭을 찾아서 클릭된 블럭을 그 위치로 이동시킴
        for (int i = 0; i < _currentColumn; i++)
        {
            for (int j = 0; j < _currentRow; j++)
            {
                if ((_backBoard[i, j] != null) &&
                    _backBoard[i, j].GetComponent<Block>().CurrentState != Block.State.FIXED &&
                    _backBoard[i, j].GetComponent<Block>().BlockNumber == clickObj.GetComponent<Block>().BlockNumber)
                {
                    Destroy(clickObj.GetComponent<Collider>());
                    clickObj.GetComponent<Block>().MoveToFixedPosition(_backBoard[i, j]);

                    _sScoureManager.scoure += 1;
                    //goto Exit;
                    return;
                }
            }
        }

        //Exit:;

    }


    /// <summary>
    /// 보드상에 선택된 블럭의 컬러에 해당하는 블럭을 전부 찾아서
    /// 이동시킨다.
    /// </summary>
    /// <param name="index"></param>
    private void AllBlockFixedPosition(int index)
    {
        List<GameObject> matchIndexBlockList = new List<GameObject>();

        for (int i = 0; i < _currentColumn; i++)
        {
            for (int j = 0; j < _currentRow; j++)
            {
                if (_backBoard[i, j] != null)
                {
                    var block = _backBoard[i, j].GetComponent<Block>();

                    if (block.BlockNumber == index &&
                        block.CurrentState != Block.State.FIXED)
                    {
                        matchIndexBlockList.Add(_backBoard[i, j]);
                    }
                }
            }
        }

        int count = 0;

        for (int i = 0; i < _currentColumn; i++)
        {
            for (int j = 0; j < _currentRow; j++)
            {
                if (_board[i, j] != null)
                {
                    if (_board[i, j].GetComponent<Block>().BlockNumber == index &&
                        _board[i, j].GetComponent<Block>().CurrentState != Block.State.FIXED)
                    {
                        Destroy(_board[i, j].GetComponent<Collider>());
                        _board[i, j].GetComponent<Block>().MoveToFixedPosition(matchIndexBlockList[count++]);
                        _sScoureManager.scoure += 1;
                    }
                }
            }
        }
    }

    /// <summary>
    ///  선택된 블럭을 백보드상에 위치시키면 주변에 연결되어있는 같은 색깔의 블럭
    ///  채워주는 함수
    /// </summary>
    /// <param name="backboard"></param>
    /// <param name="board"></param>
    /// <param name="matchObject"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    private int RangeMatchItem(GameObject[,] backboard, GameObject[,] board, GameObject matchObject, int width, int height)
    {
        // 이전에 처리에서 들어있는 값을 초기화한다.
        if (_matchObjects.Count > 0)
        {
            _matchObjects.Clear();
        }

        // 보드상의 블럭의 isMatchCheck를 초기화한다.
        foreach (GameObject obj in board)
        {
            if (obj != null)
            {
                obj.GetComponent<Block>().IsMatchCheck = false; // 초기화
            }
        }

        matchObject.GetComponent<Block>().IsMatchCheck = true;
        var matchObjectBlock = matchObject.GetComponent<Block>();
        MatchCheck(backboard, matchObjectBlock.col, matchObjectBlock.row - 1, matchObjectBlock.BlockNumber, width, height, Direction.LEFT);
        MatchCheck(backboard, matchObjectBlock.col, matchObjectBlock.row + 1, matchObjectBlock.BlockNumber, width, height, Direction.RIGHT);
        MatchCheck(backboard, matchObjectBlock.col - 1, matchObjectBlock.row, matchObjectBlock.BlockNumber, width, height, Direction.UP);
        MatchCheck(backboard, matchObjectBlock.col + 1, matchObjectBlock.row, matchObjectBlock.BlockNumber, width, height, Direction.DOWN);

        if (_matchObjects.Count > 0)
        {
            List<GameObject> flyBlocks = new List<GameObject>();

            foreach (GameObject cubeobj in board)
            {
                if (cubeobj != null && cubeobj.GetComponent<Block>().CurrentState != Block.State.FIXED
                    && cubeobj.GetComponent<Block>().BlockNumber == matchObject.GetComponent<Block>().BlockNumber)
                {
                    flyBlocks.Add(cubeobj);

                    // 날려야 하는 블럭을 주변에 같은 색 블럭 만큼 찾으면 
                    if(flyBlocks.Count == _matchObjects.Count)
                    {
                        break;
                    }
                }
            }

            int count = 0;
            foreach(GameObject flyobj in flyBlocks)
            {
                Destroy(flyobj.GetComponent<Collider>());

                flyobj.GetComponent<Block>().MoveToFixedPosition(_matchObjects[count++]);
            }
        }

        foreach(GameObject obj in _matchObjects)
        {
            obj.GetComponent<Block>().CurrentState = Block.State.FIXED;
        }

        return _matchObjects.Count;
    }


    /// <summary>
    /// 주변에 같은 색깔의 블럭을 찾는다..(백보드)
    /// </summary>
    /// <param name="board"></param>
    /// <param name="col"></param>
    /// <param name="row"></param>
    /// <param name="blockNumber"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="dir"></param>
    private void MatchCheck(GameObject[,] board, int col, int row, int blockNumber, int width, int height, Direction dir)
    {
        // 보드상의 범위를 벗어나거나, 같은 색깔이 아니거나
        // 이미 Fixed된 블럭이거나 이미 처리된 블럭이면 return(처리할 필요가 없음)
        if (col < 0 || col >= height || row < 0 || row >= width || board[col, row] == null
            || board[col, row].GetComponent<Block>().CurrentState == Block.State.FIXED
            || board[col, row].GetComponent<Block>().BlockNumber != blockNumber
            || board[col, row].GetComponent<Block>().IsMatchCheck)
        {
            return;
        }

        if(!board[col, row].GetComponent<Block>().IsMatchCheck)
        {
            _matchObjects.Add(board[col, row]);
            board[col, row].GetComponent<Block>().IsMatchCheck = true;
        }

        if(dir == Direction.UP)
        {
            MatchCheck(board, col - 1, row, blockNumber, width, height, Direction.UP);
            MatchCheck(board, col, row + 1, blockNumber, width, height, Direction.RIGHT);
            MatchCheck(board, col, row - 1, blockNumber, width, height, Direction.LEFT);
        } else if (dir == Direction.DOWN)
        {
            MatchCheck(board, col + 1, row, blockNumber, width, height, Direction.DOWN);
            MatchCheck(board, col, row + 1, blockNumber, width, height, Direction.RIGHT);
            MatchCheck(board, col, row - 1, blockNumber, width, height, Direction.LEFT);
        } else if(dir == Direction.LEFT)
        {
            MatchCheck(board, col + 1, row, blockNumber, width, height, Direction.DOWN);
            MatchCheck(board, col - 1, row, blockNumber, width, height, Direction.UP);
            MatchCheck(board, col, row - 1, blockNumber, width, height, Direction.LEFT);

        } else if(dir == Direction.RIGHT)
        {
            MatchCheck(board, col + 1, row, blockNumber, width, height, Direction.DOWN);
            MatchCheck(board, col - 1, row, blockNumber, width, height, Direction.UP);
            MatchCheck(board, col, row + 1, blockNumber, width, height, Direction.RIGHT);
        }
    }

    /// <summary>
    /// 마우스 버튼이 클릭된 경우 클릭된 좌표값으로
    /// 3D 공간상에 좌표값과 일치하는 오브젝트 찾는 함수    
    /// </summary>
    /// <returns></returns>
    GameObject ReturnClickedObjects()
    {
        GameObject target = null;   // 클릭된 오브젝트 저장용

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray.origin, ray.direction * 10);

        foreach(RaycastHit rch in hits)
        {
            if(rch.collider != null && rch.transform.tag == "CubeBlock")
            {
                target = rch.collider.gameObject;
                break;
            }
        }

        return target;        
    }


    /// <summary>
    /// 마우스 입력에 따른 처리
    /// </summary>
    private void MouseEventProcess()
    {
        // 마우스를 마우스버튼을 눌렀을 경우
        if (Input.GetMouseButtonDown(0))
        {
            _target = ReturnClickedObjects();

            if(_target != null)
            {
                /*
                if (_sScoureManager.Isfever)
                    _sItemManager.IsStepFever = true;
                else
                    _sItemManager.IsStepFever = false;

                if(_sScoureManager.Ismegafever)
                    _sItemManager.IsFeverMagicRod = true;
                else
                    _sItemManager.IsFeverMagicRod = false;
                */


                if (_sItemManager.IsStepFever)
                {
                    Color color = _target.GetComponent<Block>().OriginColor;
                    _colorscount[$"{color.r}{color.g}{color.b}"]--;
                    if (IsThisColorAllMatched(color) && _sScoureManager.Isfever)
                    {
                        _sScoureManager.fevertime = 11f;
                        _sScoureManager.Ismegafever = true;
                        _sScoureManager.Isfever = false;
                        _sItemManager.IsStepFever = false;
                        _sItemManager.IsFeverMagicRod = true;
                        _sScoureManager.ChangeParticleColor();
                        SoundManager.Instance.Play_SwapSuccess_Effect_Sound();
                        SoundManager.Instance.Play_Double_Combo_Sound();
                    }

                    StepBlockMatchedPosition(_target);
                    _target = null;
                    _isMouseDrag = false;
                    if (!_sScoureManager.Isfever)
                    {
                        _sItemManager.StepMagicRodCount -= 1;
                        _sItemManager.IsStepFever=false;
                    }

                }
                else if (_sItemManager.IsFeverMagicRod)
                {

                    AllBlockFixedPosition(_target.GetComponent<Block>().BlockNumber);
                    _target = null;
                    _isMouseDrag = false;
                    if (!_sScoureManager.Ismegafever)
                    {
                        _sItemManager.FeverMagicRodCount -= 1;
                        _sItemManager.IsFeverMagicRod=false;
                    }
                }
                else if (_sItemManager.IsRangeMatch)
                {
                    RangeMatchItem(_backBoard, _board, _target, _currentRow, _currentColumn);
                    _target = null;
                    _isMouseDrag = false;
                    _sItemManager.RangeMatchCount -= 1;
                    _sItemManager.IsRangeMatch=false;
                }
                else
                {
                    _isMouseDrag = true;

                    ChanageBlockTextColor(_target.GetComponent<Block>().BlockNumber, Color.red);

                    Vector3 clickObjectPosition = _target.transform.position;

                    _target.transform.position = new Vector3(clickObjectPosition.x, clickObjectPosition.y, 0.0f);

                    Destroy(_target.GetComponent<Rigidbody>()); // 물리영향을 받지 않도록 RigidBody를 제거한다.

                    // 선택된 블럭의 회전을 초기화하여 전면이 보이도록 처리
                    _target.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);


                    // 선택된 블럭이 클릭된 위치의 위쪽에 표시되도록 처리
                    Vector3 pos = _target.transform.position;
                    _target.transform.position =
                        new Vector3(pos.x, pos.y + CLICKYOFFEST, pos.z - CLICKZOFFSET);

                    // 선택된 블럭이 조금 더 커 보이도록 처리
                    _target.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);


                    _screenPosition = Camera.main.WorldToScreenPoint(_target.transform.position);
                    _offset = _target.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPosition.z));

                }
            }

        }

        // 마우스 버튼을 놓은 경우
        if (Input.GetMouseButtonUp(0))
        {
            _isMouseDrag = false;
            _offset = Vector3.zero;
            
            if (_target != null)
            {
                SoundManager.Instance.Play_SwapFailure_Effect_Sound();

                ChanageBlockTextColor(_target.GetComponent<Block>().BlockNumber, Color.black);

                GameObject matchObj = IsMatchPositionColorBlock(_target);

                if (matchObj != null)
                {
                    _target.transform.position = matchObj.GetComponent<Block>().OriginPosition;
                    matchObj.SetActive(false);
                    _target.GetComponent<Block>().CurrentState = Block.State.FIXED;
                    matchObj.GetComponent<Block>().CurrentState = Block.State.FIXED;
                    Destroy(_target.GetComponent<Rigidbody>());
                    Destroy(_target.GetComponent<Collider>());
                    //_target.transform.localScale = _target.GetComponent<Block>().OriginScale;

                    _target.GetComponent<Block>().MatchBlockAnimationStart();

                    _sScoureManager.scoure += 1;

                } else
                {
                    _target.AddComponent<Rigidbody>();  // 컴포넌트를 게임오브젝트에 추가한다.
                    _target.GetComponent<Rigidbody>().useGravity = true;
                    _target.transform.localScale = _target.GetComponent<Block>().OriginScale;

                }
            }
        }

        // 마우스 버튼이 눌린상태에서 움직이는 경우(드래그)
        if (_isMouseDrag)
        {
            Vector3 currentScreenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPosition.z);

            Vector3 currentPosition = Camera.main.ScreenToWorldPoint(currentScreenPos) + _offset;

            currentPosition.z = -0.5f;

            if (_target != null)
            {
                _target.transform.position = currentPosition;

                // 블럭 이동중에 매칭되는 위치를 찾은 경우
                GameObject matchObj = IsMatchPositionColorBlock(_target);

                if(matchObj != null)
                {
                    // 해당블럭을 해당 위치로 이동시키고
                    _target.transform.position = matchObj.GetComponent<Block>().OriginPosition;
                    _target.GetComponent<Block>().ShowOnOffNumberText(false);
                    matchObj.SetActive(false);
                    _target.GetComponent<Block>().CurrentState = Block.State.FIXED;
                    matchObj.GetComponent<Block>().CurrentState = Block.State.FIXED;

                    ChanageBlockTextColor(_target.GetComponent<Block>().BlockNumber, Color.black);

                    Destroy(_target.GetComponent<Rigidbody>());
                    Destroy(_target.GetComponent<Collider>());

                    _target.GetComponent<Block>().MatchBlockAnimationStart();
                    _sScoureManager.scoure += 1;

                    // 한 가지 종류 color를 모두 맞춘경우
                    Color matchedColor = matchObj.GetComponent<Block>().OriginColor;
                    _colorscount[$"{matchedColor.r}{matchedColor.g}{matchedColor.b}"]--;

                    
                    if (IsThisColorAllMatched(matchedColor) && !_sScoureManager.Isfever)
                    {
                        _sScoureManager.fevertime = 11f;
                        _sScoureManager.Isfever = true;
                        _sItemManager.IsStepFever = true;
                        Debug.Log($"Isfever{_sScoureManager.Isfever}");
                        Debug.Log($"IsStepFever{_sItemManager.IsStepFever}");
                        _sScoureManager.ChangeParticleColor();
                        _sScoureManager.StartParticle();
                        SoundManager.Instance.Play_SwapSuccess_Effect_Sound();
                        SoundManager.Instance.Play_Combo_Sound();
                    }

                    /*
                    else if (IsThisColorAllMatched(matchedColor) && _sScoureManager.Isfever)
                    {
                        _sScoureManager.fevertime = 11f;
                        _sScoureManager.Ismegafever = true;
                        _sScoureManager.Isfever = false;
                        _sItemManager.IsStepFever = false;
                        _sScoureManager.ChangeParticleColor();
                    }*/


                    _target = null;
                    _isMouseDrag = false;

                }
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (IsBlockClickOK)
        {
            MouseEventProcess();

            if (!_isAllMadeToggle && IsAllBlockMade())
            {
                // 모든 블럭이 맞았음 맞는 처리...
                _isAllMadeToggle = true;
                _target = null;

                Debug.Log("------- AllBlockMade ----------");
                AllBlockComplete();

            }
        }
    }
}
