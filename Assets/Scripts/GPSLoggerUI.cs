using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GPSLoggerUI : MonoBehaviour
{
    public TextMeshProUGUI textStatus;
    public TextMeshProUGUI textSeq;
    public TextMeshProUGUI textDeviceID;

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

    public Button buttonUpload;
    public Button buttonPre;
    public Button buttonNext;

    public Transform transformLogs;
    public PopupUploadResult popupUploadResult;

    protected STGeoData mCurrentGeoData;

    private void Start()
    {
        buttonStartLog.onClick.AddListener(onClickedButtonLogging);
        buttonUpload.onClick.AddListener(onClickUpload);
        buttonPre.onClick.AddListener(onClickPre);
        buttonNext.onClick.AddListener(onClickNext);
        popupUploadResult.gameObject.SetActive(false);

        drawUI();
        DrawLogging();
    }


    protected void drawUI()
    {
        textDeviceID.text = SystemInfo.deviceUniqueIdentifier;
    }


    public void CreateLogData ( int _index )
    {
        GameObject go = Resources.Load("Prefabs/CellGeoData") as GameObject;
        CellGeoData cell = Instantiate(go, transformLogs).GetComponent<CellGeoData>();

        cell.SetData(_index);
        cell.ACTION_ON_CLICK = onActionClickCellGeoData;
    }


    public void OpenPopupUploadResult ( string _res )
    {
        popupUploadResult.gameObject.SetActive(true);
        popupUploadResult.SetReult(_res);
    }


    protected void onClickedButtonLogging ()
    {
        GPSLoggerManager.Instance.ChangeLoggingState();
        DrawLogging();
    }


    protected void onClickUpload()
    {
        GPSLoggerManager.Instance.Upload();
    }


    protected void onClickNext()
    {
        GPSLoggerManager.Instance.IncreaseDataTarget();
    }


    protected void onClickPre()
    {
        GPSLoggerManager.Instance.DecreaseDataTarget();
    }


    protected void DrawLogging()
    {
        bool logging = GPSLoggerManager.Instance.isLogging;

        if ( logging == false )
        {
            textButton.text = "Start logging";
        }
        else
        {

            textButton.text = "Stop logging";
        }

        buttonUpload.interactable = !logging;
        buttonPre.interactable = !logging;
        buttonNext.interactable = !logging;
    }


    public void OnActionGPSData( STGeoData _data )
    {
        textSeq.text = string.Format("{0}", _data.sequence);

        DrawGPSData(_data);
        mCurrentGeoData = _data;
    }


    public void OnActionGPSStatus ( GPSModule.eGPSStatus _status )
    {
        Debug.Log("#onActionGPSStatus: " + _status.ToString());
        textStatus.text = _status.ToString();
    }


    protected void onActionClickCellGeoData ( CellGeoData _cell )
    {
        GPSLoggerManager.Instance.SetLogData(_cell.index);
    }


    public void DrawGPSData ( STGeoData _data )
    {
        long timeGap = 0;
        double posGapH = 0;
        double posGapV = 0;

        if ( mCurrentGeoData.Equals(default(STGeoData)) == false )
        {
            if ( mCurrentGeoData == _data )
            {
                return;
            }

            timeGap = _data.timeStamp - mCurrentGeoData.timeStamp;
            posGapH = _data.lat - mCurrentGeoData.lat;
            posGapV = _data.lng - mCurrentGeoData.lng;
        }


        textTimestamp.text = string.Format("{0}", _data.timeStamp);
        textGeocoord.text = string.Format("{0}, {1}", _data.lat, _data.lng);
        textAltitude.text = string.Format("{0}", _data.alt);
        textSpeed.text = string.Format("{0}", _data.speed);
        textAccGeocoord.text = string.Format("{0}, {1}", _data.acc, _data.accV);
        textAccSpeed.text = string.Format("{0}", _data.accS);
        textTimeGap.text = string.Format("{0}", timeGap);
        textPosGap.text = string.Format("{0}, {1}", posGapH, posGapV);
    }
}
