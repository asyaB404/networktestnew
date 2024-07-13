using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class StartPanel : BasePanel<StartPanel>
{
    private void Awake()
    {

    }

    public override void CallBack(bool flag)
    {
        if (flag)
        {
            gameObject.SetActive(true);
        }
        else
        {
            
        }
    }
}
