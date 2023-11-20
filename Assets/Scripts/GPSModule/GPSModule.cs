using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;




public struct STGeoData
{
    public long timestamp;
    public int sequence;
    public double latitude;
    public double longitude;
    public double alt;
    public float speed;
    public float horizontalAccuracy;
    public float verticalAccuracy;
    public float accS;

    public STGeoData(long _timeStamp, int _sequence, double _lat, double _lng, double _alt, float _speed, float _acc, float _accV, float _accS)
    {
        timestamp = _timeStamp;
        sequence = _sequence;
        latitude = _lat;
        longitude = _lng;
        alt = _alt;
        speed = _speed;
        horizontalAccuracy = _acc;
        verticalAccuracy = _accV;
        accS = _accS;
    }


    public static bool operator ==(STGeoData _v1, STGeoData _v2)
    {
        return _v1.latitude == _v2.latitude && _v1.longitude == _v2.longitude;
    }

    public static bool operator !=(STGeoData _v1, STGeoData _v2)
    {
        return _v1.latitude != _v2.latitude || _v1.longitude != _v2.longitude;
    }


    public bool IsSameCoord(double _lat, double _lng)
    {
        return latitude == _lat && longitude == _lng;
    }


    public void Debug()
    {
        UnityEngine.Debug.Log("Lat = " + latitude + " / Lng = " + longitude + " / Timestamp = " + timestamp);
    }
}


public struct STHeadingData
{
    public eHeadingAccuracy accuracy;
    public float heading;


    public STHeadingData(eHeadingAccuracy _accuracy, float _heading)
    {
        accuracy = _accuracy;
        heading = _heading;
    }
}

public enum eGPSStatus
{
    WAIT = 0,
    INITIALIZING,
    TIME_OUT,
    RUNNING,
    STOP,
    DISABLE,
    FAILED,
    COUNT
}


public class GPSModule : MonoBehaviour
{
    protected bool bInitialized;

    public Action<eGPSStatus> ACTION_GPS_STATUS;
    public Action<STGeoData> ACTION_GPS_GEODATA;

    protected eGPSStatus mStatus;
    protected bool mLocationServiceIsReady = false;

    protected STGeoData mGeoDataPrev;
    protected STGeoData mGeoDataCurr;

    public eGPSStatus status { get { return mStatus; } }
    public STGeoData geoDataCurrent { get { return mGeoDataCurr; } }
    public STGeoData geoDataPrev { get { return mGeoDataPrev; } }

    protected STHeadingData mHeadingDataCurr;
    public STHeadingData headingDataCurrent { get { return mHeadingDataCurr; } }

    private void Awake()
    {
        mStatus = eGPSStatus.WAIT;
    }



    public void StartGPS()
    {
        if (bInitialized == false)
        {
            bInitialized = true;
            NativeGPSPlugin.Initialize(StartGPS);
        }
        else
        {
            StartCoroutine("IE_GPSModule");
        }
    }




    protected IEnumerator IE_GPSModule()
    {
        WaitForSeconds ws = new WaitForSeconds(0.1f);

        SetGPSStatus(eGPSStatus.INITIALIZING);

        NativeGPSPlugin.StartLocation();
        mLocationServiceIsReady = NativeGPSPlugin.HasUserAuthorize();

        yield return null;

        if (mLocationServiceIsReady == false)
        {
            SetGPSStatus(eGPSStatus.FAILED);
            yield break;
        }

        while (mLocationServiceIsReady)
        {
            updateGPS();
            yield return ws;
        }

    }


    protected void updateGPS()
    {
        if (mLocationServiceIsReady == true)
        {

            if (NativeGPSPlugin.IsEnableGPS() == false)
            {
                SetGPSStatus(eGPSStatus.DISABLE);

                //Debug.Log("GPS 비활성화");
                return;
            }


            long timeStamp = NativeGPSPlugin.GetTimestamp();

            long timePrev = (long)mGeoDataCurr.timestamp;
            long timeCurr = (long)timeStamp;

            if (timePrev == timeCurr)
            {
                return;
            }

            SetGPSStatus(eGPSStatus.RUNNING);

            double lat = NativeGPSPlugin.GetLatitude();
            double lng = NativeGPSPlugin.GetLongitude();

            //소수점 5번째 자리수만 표시, 반올림
            lat = Math.Round(lat, 5);
            lng = Math.Round(lng, 5);

            if (mGeoDataCurr.IsSameCoord(lat, lng))
            {
                //Debug.Log("GPS 같은 위치");
                return;
            }

            double alt = NativeGPSPlugin.GetAltitude();
            float acc = NativeGPSPlugin.GetAccuracy();
            float accV = NativeGPSPlugin.GetVerticalAccuracyMeters();
            float speed = NativeGPSPlugin.GetSpeed();
            float accS = NativeGPSPlugin.GetSpeedAccuracyMetersPerSecond();


            STGeoData data = new STGeoData(timeStamp, 0, lat, lng, alt, acc, accV, speed, accS);

            if (ACTION_GPS_GEODATA != null && ACTION_GPS_GEODATA.GetInvocationList().Length > 0)
            {
                ACTION_GPS_GEODATA(data);
            }

            mGeoDataPrev = mGeoDataCurr;
            mGeoDataCurr = data;

            mGeoDataCurr.Debug();
        }
    }



    protected void updateHeading()
    {
        if (mLocationServiceIsReady == true)
        {
            eHeadingAccuracy accuracy = NativeGPSPlugin.GetHeadingAccuracy();
            float heading = NativeGPSPlugin.GetHeading();

            mHeadingDataCurr = new STHeadingData(accuracy, heading);
        }
    }




    protected void SetGPSStatus(eGPSStatus _status)
    {
        if (mStatus == _status)
        {
            return;
        }

        mStatus = _status;

        if (ACTION_GPS_STATUS != null && ACTION_GPS_STATUS.GetInvocationList().Length > 0)
        {
            ACTION_GPS_STATUS(mStatus);
        }

    }


    public void StopGPS()
    {
        StopCoroutine("IE_GPSModule");

        mStatus = eGPSStatus.STOP;
        NativeGPSPlugin.StopLocation();
    }


    public void OnDestroy()
    {
        NativeGPSPlugin.Destroy();
    }

}