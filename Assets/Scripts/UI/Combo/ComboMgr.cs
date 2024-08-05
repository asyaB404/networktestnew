using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///     连击管理器
/// </summary>
public class ComboMgr : MonoBehaviour
{
    [SerializeField] private Sprite[] nums;
    [SerializeField] private GameObject comboNumsPrefabs;
    [SerializeField] private Transform comboParent;
    [SerializeField] private bool combing;
    private float _combingTime;
    public static ComboMgr Instance { get; private set; }
    public int Combo { get; private set; }

    public bool Combing
    {
        get => combing;
        set
        {
            if (combing != value)
            {
                if (value)
                    transform.DOScale(1f, 0.1f);
                else
                    transform.DOScale(0f, 0.1f);
            }

            combing = value;
            if (combing == false)
            {
                Combo = 0;
                _combingTime = 0;
            }
            else
            {
                Combo += 1;
                _combingTime = 1.5f;
                UpdateCombo();
            }
        }
    }

    private void Awake()
    {
        Instance = this;
    }


    private void Update()
    {
        if (_combingTime > 0)
        {
            _combingTime -= Time.deltaTime;
        }
        else
        {
            if (Combing) Combing = false;
        }
    }

    public void UpdateCombo()
    {
        comboParent.DOKill(true);
        comboParent.DOScale(1.25f, 0.25f).OnComplete(() => { comboParent.DOScale(1f, 0.25f); });
        comboParent.DestroyAllChildren();
        var combostring = Combo.ToString();
        var size = combostring.Length;
        for (var i = 0; i < size; i++)
        {
            var combobj = Instantiate(comboNumsPrefabs, comboParent, false);
            combobj.transform.localPosition = new Vector3(-2.0f - (size - 1 - i), 0);
            combobj.GetComponent<Image>().sprite = nums[int.Parse(combostring[i].ToString())];
        }
    }
}