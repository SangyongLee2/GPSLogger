using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPSLoggerManager : MonoBehaviour
{
    public static GPSLoggerManager Instance
    {
        get
        {
            if ( instance == null )
            {
                return null;
            }

            return instance;
        }
    }

    protected static GPSLoggerManager instance;

    public GPSLoggerUI gpsLoggerUI;
    public GPSModule gpsModule;
    public GPSLoggerMap gpsLoggerMap;
    public GPSLogger gpsLogger;
    public GPSLoggerDataStorage gpsLoggerStorage;
    public GPSLoggerNetwork network;

    protected int mCurrentLogIndex;
    protected int mLogDataTargetIndex;
    protected int mLogDataTargetIndexMax;

    public bool isLogging = false;

    protected void Awake()
    {
        initialize();
    }


    protected void initialize()
    {
        mCurrentLogIndex = -1;
        instance = this;

        gpsLoggerStorage = new GPSLoggerDataStorage();
        gpsLogger = new GPSLogger(gpsModule);

        network.ON_NETWORK_ERROR += gpsLoggerUI.OpenPopupUploadResult;

        gpsLogger.ACTION_LOG_GEO_DATA += OnActionLogGPSData;
        gpsModule.ACTION_GPS_GEODATA += OnActionGPSData;
        gpsModule.ACTION_GPS_STATUS += OnActionGPSStatus;

        gpsModule.StartGPS();
    }


    protected void Start()
    {
        //Test
        //List<STGeoData> data = gpsLogger.Get();
        //createLogData(data);
    }


    public void ChangeLoggingState()
    {
        isLogging = !isLogging;

        if ( isLogging == false )
        {
            List<STGeoData> data = gpsLogger.Get();
            createLogData(data);
        }
        else
        {
            gpsLogger.Clear();
            gpsLoggerMap.Clear();
        }
    }


    protected void createLogData ( List<STGeoData> _data )
    {
        mCurrentLogIndex = -1;

        if (_data.Count > 0)
        {
            Debug.Log("Add Data");

            int index = gpsLoggerStorage.Add(_data);
            gpsLoggerUI.CreateLogData(index);

            SetLogData(index);
        }
    }


    public void SetLogData ( int _index )
    {
        if ( isLogging || mCurrentLogIndex == _index )
        {
            return;
        }

        Debug.Log("Set Log Data " + _index);

        List<STGeoData> storageData = gpsLoggerStorage.Get(_index);
        gpsLoggerMap.Create(storageData);

        mCurrentLogIndex = _index;
        mLogDataTargetIndex = 0;
        mLogDataTargetIndexMax = storageData.Count;
        drawDataTarget();
    }


    protected void OnDestroy()
    {
        gpsLogger.ACTION_LOG_GEO_DATA -= OnActionLogGPSData;
        gpsModule.ACTION_GPS_GEODATA -= OnActionGPSData;
        gpsModule.ACTION_GPS_STATUS -= OnActionGPSStatus;

        gpsLoggerStorage.Release();
        gpsLogger.Release();
    }


    public void Upload()
    {
        if ( isLogging || mCurrentLogIndex < 0 )
        {
            return;
        }

        Debug.Log("Upload data " + mCurrentLogIndex);

        List<STGeoData> data = gpsLoggerStorage.Get(mCurrentLogIndex);
        network.RequestSave(data, onResponseUpload);
    }


    protected void onResponseUpload ( object _data )
    {
        gpsLoggerUI.OpenPopupUploadResult("Upload Succeed");
    }


    protected void OnActionLogGPSData ( STGeoData _data )
    {
        if ( isLogging == false )
        {
            return;
        }

        gpsLoggerUI.OnActionGPSData(_data);
    }

     
    protected void OnActionGPSData ( STGeoData _data )
    {
        if ( isLogging == false )
        {
            return;
        }

        gpsLoggerMap.OnActionGPSData(_data);
    }


    protected void OnActionGPSStatus ( eGPSStatus _status )
    {
        gpsLoggerUI.OnActionGPSStatus(_status);
    }


    public void IncreaseDataTarget()
    {
        if ( mLogDataTargetIndex >= mLogDataTargetIndexMax )
        {
            return;
        }

        mLogDataTargetIndex++;
        drawDataTarget();
    }


    public void DecreaseDataTarget()
    {
        if ( mLogDataTargetIndex <= 0 )
        {
            return;
        }

        mLogDataTargetIndex--;
        drawDataTarget();
    }


    protected void drawDataTarget()
    {
        STGeoData data = gpsLoggerMap.ViewGPSPositionData(mLogDataTargetIndex);
        gpsLoggerUI.DrawGPSData(data);
    }
}
