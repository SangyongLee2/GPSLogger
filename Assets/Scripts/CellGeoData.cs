using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CellGeoData : MonoBehaviour
{
    public int mSequence;
    public STGeoLogData mData;

    public Action<CellGeoData> ACTION_ON_CLICK;
    public Button buttonCell;
    public TextMeshProUGUI textSequence;


    public void SetData ( int _seq, STGeoLogData _data )
    {
        mSequence = _seq;
        mData = _data;
    }


    protected void Start()
    {
        textSequence.text = string.Format("{0}", mSequence);
        buttonCell.onClick.AddListener(onClickThis);
    }


    protected void onClickThis()
    {
        if ( ACTION_ON_CLICK != null )
        {
            ACTION_ON_CLICK(this);
        }
    }
}
