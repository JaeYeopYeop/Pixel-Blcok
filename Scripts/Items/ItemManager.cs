using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    public enum Item
    {
        BASE,
        ONE,
        TWO,
        THREE,
        FOUR,
        FIVE,
        SIX,
        SEVEN
    };

    public enum Direction
    {
        LEFT,
        RIGHT,
        UP,
        DOWN
    };

    private Item currentItem;
    public Item CurrentItem {
        set
        {
            PlayerPrefs.SetInt("Anime_Item", (int)value);
            currentItem = value;
        }

        get
        {
            currentItem = (Item)PlayerPrefs.GetInt("Anime_Item",0);
            return currentItem;
        } 
        
    }

    public bool IsFeverMagicRod { set; get; } = false;    
    public bool IsRangeMatch { set; get; } = false;
    public bool IsStepFever { set; get; } = false;

    public int FeverMagicRodCount;
    public int RangeMatchCount;
    public int StepMagicRodCount;

    [SerializeField] private Convert3D iConvert3D;
    [SerializeField] private GameView iGameView;
    [SerializeField] private ScoureManager iScoureManager;
    [SerializeField] private StageManager iStageManager;

    // RangeMatch Item 구현용 List
    // private List<GameObject> _matchObjects = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
    }

    private void Update()
    {

    }

    public void OnClickMoveAnimeChange()
    {

    }

    public void onClickFeverMagicRodBtn()
    {
        if (FeverMagicRodCount > 0)
        {
            if (!(iScoureManager.Isfever || iScoureManager.Ismegafever))
            {
                IsFeverMagicRod = !IsFeverMagicRod;

                if (IsFeverMagicRod)
                {
                    SoundManager.Instance.Play_ButtonClick_Effect_Sound();
                    iGameView.gFeverMagicRodBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>("StageLevelImages/Image_purple/stage");
                }
                else
                {
                    iGameView.gFeverMagicRodBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>("StageLevelImages/Image_pink/stage");
                }
            }
        }
    }

    public void onClickStepMagicRodBtn()
    {
        if (StepMagicRodCount > 0)
        {
            if (!(iScoureManager.Isfever || iScoureManager.Ismegafever))
            {
                IsStepFever = !IsStepFever;

                if (IsStepFever)
                {
                    SoundManager.Instance.Play_ButtonClick_Effect_Sound();
                    iGameView.gStepMagicRodBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>("StageLevelImages/Image_purple/stage");
                }
                else
                {
                    iGameView.gStepMagicRodBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>("StageLevelImages/Image_pink/stage");
                }
            }
        }
    }

    public void onClickRangeMatchBtn()
    {
        if (RangeMatchCount > 0)
        {
            if (!(iScoureManager.Isfever || iScoureManager.Ismegafever))
            {
                IsRangeMatch = !IsRangeMatch;

                if (IsRangeMatch)
                {
                    SoundManager.Instance.Play_ButtonClick_Effect_Sound();
                    iGameView.gRangeMagicBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>("StageLevelImages/Image_purple/stage");
                }
                else
                {
                    iGameView.gRangeMagicBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>("StageLevelImages/Image_pink/stage");
                }
            }
        }
    }












    /*
    // RangeMatch 아이템
    public int RangeMatchItem(GameObject[,] backboard, GameObject[,] board, GameObject matchObject,
        int width, int height)
    {
        if (_matchObjects.Count > 0)
        {
            _matchObjects.Clear();
        }

        foreach(GameObject obj in board)
        {
            if(obj != null)
            {
                obj.GetComponent<Block>().IsMatchCheck = false;
            }
        }

        // 
        matchObject.GetComponent<Block>().IsMatchCheck = true;

        var block = matchObject.GetComponent<Block>(); // Block 컴포넌트를 저장해서 사용하는것이 GetComponent의 호출횟수를
                                                        //줄일수 있어서 효율적입니다.

        // 4방향으로 같은 색깔의 인접한 블럭이 있는지 찾는다.
        // 왼쪽
        MatchCheck(backboard, block.col, matchObject.GetComponent<Block>().row - 1,
            matchObject.GetComponent<Block>().BlockNumber, width, height, Direction.LEFT);
        // 오른쪽
        MatchCheck(backboard, matchObject.GetComponent<Block>().col, matchObject.GetComponent<Block>().row + 1,
            matchObject.GetComponent<Block>().BlockNumber, width, height, Direction.RIGHT);
        // 위쪽
        MatchCheck(backboard, matchObject.GetComponent<Block>().col - 1, matchObject.GetComponent<Block>().row,
            matchObject.GetComponent<Block>().BlockNumber, width, height, Direction.UP);
        // 아래쪽
        MatchCheck(backboard, matchObject.GetComponent<Block>().col + 1, matchObject.GetComponent<Block>().row,
            matchObject.GetComponent<Block>().BlockNumber, width, height, Direction.DOWN);

        if(_matchObjects.Count > 0)
        {
            // 인접한 같은 색깔의 블럭을 저장 
            List<GameObject> flyBlocks = new List<GameObject>();

            foreach(GameObject cubeobj in board)
            {
                // dropblock들중에 장착되지않았고, 블럭컬러가 같은 블럭을 matchObject 갯수만큼 찾아서 리스트업한다.
                if(cubeobj != null && cubeobj.GetComponent<Block>().CurrentState != Block.State.FIXED && 
                    cubeobj.GetComponent<Block>().BlockNumber == matchObject.GetComponent<Block>().BlockNumber)
                {
                    flyBlocks.Add(cubeobj);

                    if(flyBlocks.Count == _matchObjects.Count)
                    {
                        break;
                    }
                }
            }

            int count = 0;
            foreach(GameObject flyobj in flyBlocks)
            {
                Destroy(GetComponent<Collider>());
                flyobj.GetComponent<Block>().MoveToFixedPosition(_matchObjects[count++]);
            }

            foreach(GameObject obj in _matchObjects)
            {
                obj.GetComponent<Block>().CurrentState = Block.State.FIXED;
            }            
        }

        return _matchObjects.Count;
    }

    private void MatchCheck(GameObject[,] board, int col, int row, int blockNumber, int width, int height, Direction dir)
    {
        if(col < 0 || col >= height || row < 0 || row >= width || board[col, row] == null || 
            board[col, row].GetComponent<Block>().CurrentState == Block.State.FIXED ||
            board[col, row].GetComponent<Block>().BlockNumber != blockNumber || 
            board[col, row].GetComponent<Block>().IsMatchCheck)
        {
            return;
        }

        if(!board[col, row].GetComponent<Block>().IsMatchCheck)
        {
            _matchObjects.Add(board[col, row]); // 매치된 블럭을 저장한다.
            board[col, row].GetComponent<Block>().IsMatchCheck = true;
        }

        if(dir == Direction.UP)
        {
            MatchCheck(board, col - 1, row, blockNumber, width, height, Direction.UP);
            MatchCheck(board, col, row + 1, blockNumber, width, height, Direction.RIGHT);
            MatchCheck(board, col, row - 1, blockNumber, width, height, Direction.LEFT);
        } else if(dir == Direction.DOWN)
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
    }*/

}
