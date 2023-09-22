using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CellGeoData : MonoBehaviour
{
    public int index;
    public List<STGeoData> data;

    public Action<CellGeoData> ACTION_ON_CLICK;
    public Button buttonCell;
    public TextMeshProUGUI textSequence;


    public void SetData ( int _index, List<STGeoData> _data )
    {
        index = _index;
        data = _data;
    }


    protected void Start()
    {
        textSequence.text = string.Format("{0}", index);
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
