using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GPSLoggerUI : MonoBehaviour
{
    public GPSModule gpsModule;
    protected GPSLogger gpsLogger;

    public Image imageContents;
    public TextMeshProUGUI textStatus;
    public TextMeshProUGUI textSeq;

    public TextMeshProUGUI textTimestamp;
    public TextMeshProUGUI textGeocoord;
    public TextMeshProUGUI textAltitude;
    public TextMeshProUGUI textSpeed;
    public TextMeshProUGUI textAccGeocoord;
    public TextMeshProUGUI textAccSpeed;
    public TextMeshProUGUI textTimeGap;
    public TextMeshProUGUI textPosGap;

    public Button buttonStartLog;
    public TextMeshProUGUI textButton;

    public Transform transformLogs;
    protected List<CellGeoData> mCells;

    protected bool mIsBeginLogging = false;
    protected int mSequence = 0;

    protected bool bIsDrawPreLog = false;
    protected int mDrawPreLogIndex;


    private void Start()
    {
        mCells = new List<CellGeoData>();

        buttonStartLog.onClick.AddListener(onClickedButtonLogging);
        EnableLogging(mIsBeginLogging);

        gpsLogger = new GPSLogger(gpsModule);
        gpsLogger.ACTION_LOG_GEO_DATA += this.onActionGPSData;
        
        gpsModule.ACTION_GPS_STATUS += this.onActionGPSStatus;
        gpsModule.StartGPS();
    }



    protected void onClickedButtonLogging ()
    {
        mIsBeginLogging = !mIsBeginLogging;
        EnableLogging(mIsBeginLogging);
    }



    protected void EnableLogging ( bool _logging )
    {
        if ( _logging == false )
        {
            textButton.text = "Start logging";
            imageContents.color = new Color(0.4f, 0.4f, 0.4f);
            DestroyLogData();
        }
        else
        {
            textButton.text = "Stop logging";
            imageContents.color = new Color(0.2f, 0.2f, 0.2f);
            mSequence = 0;
        }
    }





    protected void onActionGPSStatus ( GPSModule.eGPSStatus _status )
    {
        Debug.Log("#onActionGPSStatus: " + _status.ToString());
        textStatus.text = _status.ToString();
    }


    protected void onClickPreLog ( CellGeoData _cell )
    {
        if ( _cell.mSequence == mDrawPreLogIndex )
        {
            mDrawPreLogIndex = 0;
            bIsDrawPreLog = false;
            return;
        }

        mDrawPreLogIndex = _cell.mSequence;
        bIsDrawPreLog = true;

        textSeq.text = mDrawPreLogIndex.ToString();
        drawGPSData(_cell.mData);
    }


    protected void drawGPSData ( STGeoLogData _data )
    {
        STGeoData data = _data.geoData;

        textTimestamp.text = string.Format("{0}", data.timeStamp);
        textGeocoord.text = string.Format("{0}, {1}", data.lat, data.lng);
        textAltitude.text = string.Format("{0}", data.alt);
        textSpeed.text = string.Format("{0}", data.speed);
        textAccGeocoord.text = string.Format("{0}, {1}", data.acc, data.accV);
        textAccSpeed.text = string.Format("{0}", data.accS);
        textTimeGap.text = string.Format("{0}", _data.timeGap);
        textPosGap.text = string.Format("{0}, {1}", _data.posGapH, _data.posGapV);
    }



    protected void onActionGPSData ( STGeoLogData _data )
    {
        if ( mIsBeginLogging == false )
        {
            return;
        }

        mSequence++;
        createLogData(_data);

        if ( bIsDrawPreLog )
        {
            return;
        }

        textSeq.text = mSequence.ToString();

        if ( bIsDrawPreLog == false )
        {
            drawGPSData(_data);
        }
    }


    protected void createLogData ( STGeoLogData _data )
    {
        if ( _data.Equals(default(STGeoLogData)) )
        {
            return;
        }

        GameObject go = Resources.Load("Prefabs/GeoDataLog") as GameObject;
        CellGeoData cell = Instantiate(go, transformLogs).GetComponent<CellGeoData>();

        cell.SetData(mSequence - 1, _data);
        cell.ACTION_ON_CLICK = onClickPreLog;

        mCells.Add(cell);
    }


    protected void DestroyLogData()
    {
        for ( int i = 0; i < mCells.Count; i++ )
        {
            Destroy(mCells[i].gameObject);
        }

        mCells.Clear();
    }
}
