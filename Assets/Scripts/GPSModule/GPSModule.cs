using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;




public struct STGeoData
{
    public long timeStamp;
    public int sequence;
    public double lat;
    public double lng;
    public double alt;
    public float speed;
    public float acc;
    public float accV;
    public float accS;

    public STGeoData ( long _timeStamp, int _sequence, double _lat, double _lng, double _alt, float _speed, float _acc, float _accV, float _accS )
    {
        timeStamp = _timeStamp;
        sequence = _sequence;
        lat = _lat;
        lng = _lng;
        alt = _alt;
        speed = _speed;
        acc = _acc;
        accV = _accV;
        accS = _accS;
    }
    

    public static bool operator == ( STGeoData _v1, STGeoData _v2 )
    {
        return _v1.lat == _v2.lat && _v1.lng == _v2.lng;
    }

    public static bool operator != ( STGeoData _v1, STGeoData _v2 )
    {
        return _v1.lat != _v2.lat || _v1.lng != _v2.lng;
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

        NativeGPSPlugin.StartLocation();
        mLocationServiceIsReady = NativeGPSPlugin.HasUserAuthorize();

        yield return new WaitForSeconds(0.1f);


        if (mLocationServiceIsReady == false)
        {
            SetGPSStatus(eGPSStatus.FAILED);
            yield break;
        }
        else
        {
            SetGPSStatus(eGPSStatus.RUNNING);
        }        

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

            if ( NativeGPSPlugin.IsEnableGPS() == false )
            {
                return;
            }

            long timeStamp = NativeGPSPlugin.GetTimestamp();
            double lat = NativeGPSPlugin.GetLatitude();
            double lng = NativeGPSPlugin.GetLongitude();
            double alt = NativeGPSPlugin.GetAltitude();
            float acc = NativeGPSPlugin.GetAccuracy();
            float accV = NativeGPSPlugin.GetVerticalAccuracyMeters();
            float speed = NativeGPSPlugin.GetSpeed();
            float accS = NativeGPSPlugin.GetSpeedAccuracyMetersPerSecond();


            //소수점 5번째 자리수만 표시, 반올림
            lat = Math.Round(lat, 5);
            lng = Math.Round(lng, 5);

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

            STGeoData data = new STGeoData(timeStamp, 0, lat, lng, alt, acc, accV, speed, accS);

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
