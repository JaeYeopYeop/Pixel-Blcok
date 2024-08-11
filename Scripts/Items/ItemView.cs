using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


public class ItemView : MonoBehaviour
{

    [SerializeField] private GameObject ivStageView;
    [SerializeField] private Button[] ivItems;
    [SerializeField] private ItemManager ivItemManager;

    private int Now_Item;


    private void OnEnable()
    {

        for (int i = 0; i < ivItems.Length; i++)
        {
            ivItems[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("StageLevelImages/Image_pink/stage");
            ivItems[i].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>($"StageLevelImages/Image_pink/txt/{i}");
        }

        Now_Item = (int)ivItemManager.CurrentItem;
        ivItems[Now_Item].GetComponent<Image>().sprite = Resources.Load<Sprite>("StageLevelImages/Image_purple/stage");
        ivItems[Now_Item].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>($"StageLevelImages/Image_purple/Text/{Now_Item}");
    }

    public void OnClickItem()
    {
        SoundManager.Instance.Play_ButtonClick_Effect_Sound();

        Now_Item = int.Parse(EventSystem.current.currentSelectedGameObject.name);
        if (Now_Item != (int)ivItemManager.CurrentItem)
        {
            for (int i = 0; i < ivItems.Length; i++)
            {
                ivItems[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("StageLevelImages/Image_pink/stage");
                ivItems[i].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>($"StageLevelImages/Image_pink/txt/{i}");
            }
        }
        ivItemManager.CurrentItem = (ItemManager.Item)Now_Item;
        ivItems[Now_Item].GetComponent<Image>().sprite = Resources.Load<Sprite>("StageLevelImages/Image_purple/stage");
        ivItems[Now_Item].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>($"StageLevelImages/Image_purple/Text/{Now_Item}");
    }

    public void OnClickStageViewOn()
    {
        SoundManager.Instance.Play_ButtonClick_Effect_Sound();
        this.gameObject.SetActive(false);

    }

}
