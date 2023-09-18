using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;




public struct STGeoData
{
    public long timeStamp;
    public double lat;
    public double lng;
    public double alt;
    public float speed;
    public float acc;
    public float accV;
    public float accS;

    public STGeoData ( long _timeStamp, double _lat, double _lng, double _alt, float _speed, float _acc, float _accV, float _accS )
    {
        timeStamp = _timeStamp;
        lat = _lat;
        lng = _lng;
        alt = _alt;
        speed = _speed;
        acc = _acc;
        accV = _accV;
        accS = _accS;
    }
    
}




public class GPSModule : MonoBehaviour
{
    public enum eGPSStatus
    {
        WAIT = 0,
        INITIALIZING,
        TIME_OUT,
        RUNNING,
        STOP,
        FAILED,
        COUNT
    }


    public Action<eGPSStatus> ACTION_GPS_STATUS;
    public Action<STGeoData> ACTION_GPS_GEODATA;

    protected eGPSStatus mStatus;
    protected bool mLocationServiceIsReady = false;

    protected STGeoData mGeoDataPrev;
    protected STGeoData mGeoDataCurr;
    


    public eGPSStatus status { get { return mStatus; } }
    public STGeoData geoDataCurrent { get { return mGeoDataCurr; } }
    public STGeoData geoDataPrev {  get { return mGeoDataPrev; } }


    private void Awake()
    {
        mStatus = eGPSStatus.WAIT;
    }



    public void StartGPS ()
    {
        StartCoroutine("IE_GPSModule");
    }




    protected IEnumerator IE_GPSModule ()
    {

        WaitForSeconds ws = new WaitForSeconds(1.0f);

        SetGPSStatus(eGPSStatus.INITIALIZING);

        mLocationServiceIsReady = NativeGPSPlugin.StartLocation();

        if ( mLocationServiceIsReady == false )
        {
            SetGPSStatus(eGPSStatus.FAILED);
            yield break;
        }

        yield return new WaitForSeconds(0.1f);
       
        SetGPSStatus(eGPSStatus.RUNNING);

        while (mLocationServiceIsReady)
        {
            updateGPS();
            yield return ws;
        }

    }


    protected void updateGPS ()
    {
        if ( mLocationServiceIsReady == true )
        {

            double timeStamp = NativeGPSPlugin.GetTimestamp();
            double lat = NativeGPSPlugin.GetLatitude();
            double lng = NativeGPSPlugin.GetLongitude();
            double alt = NativeGPSPlugin.GetAltitude();
            float acc = NativeGPSPlugin.GetAccuracy();
            float accV = NativeGPSPlugin.GetVerticalAccuracyMeters();
            float speed = NativeGPSPlugin.GetSpeed();
            float accS = NativeGPSPlugin.GetSpeedAccuracyMetersPerSecond();


#if UNITY_IOS

            long timePrev = (long)mGeoDataPrev.timeStamp;
            long timeCurr = (long)timeStamp;

            if ( timePrev == timeCurr )
            {
                return;
            }


#elif UNITY_ANDROID
        
            //return 0;
#endif

            STGeoData data = new STGeoData((long)timeStamp, lat, lng, alt, acc, accV, speed, accS);

            if ( ACTION_GPS_GEODATA != null && ACTION_GPS_GEODATA.GetInvocationList().Length > 0 )
            {
                ACTION_GPS_GEODATA(data);
            }

            mGeoDataPrev = data;

        }
    }




    protected void SetGPSStatus ( eGPSStatus _status )
    {
        if ( mStatus == _status )
        {
            return;
        }

        mStatus = _status;

        //textStatus.text = _status.ToString();

        if (ACTION_GPS_STATUS != null && ACTION_GPS_STATUS.GetInvocationList().Length > 0)
        {
            ACTION_GPS_STATUS(mStatus);
        }

    }

}
