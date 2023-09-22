using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;


public class GPSLogger
{
    public Action<STGeoData> ACTION_LOG_GEO_DATA;

    protected GPSModule mGPSModule;
    protected List<STGeoData> mLogs;
    protected int mSequence;

    public GPSLogger ( GPSModule _module )
    {
        mSequence = 0;
        mLogs = new List<STGeoData>();

        mGPSModule = _module;
        mGPSModule.ACTION_GPS_GEODATA += onActionGpsData;
    }


    /// <summary>
    /// 로그 데이터 추가
    /// </summary>
    /// <param name="_data"></param>
    protected void onActionGpsData ( STGeoData _data )
    {
        if ( IsAvailable(_data) == false )
        {
            return;
        }

        //중복 데이터 방지
        if ( mLogs.Count > 0 )
        {
            STGeoData preData = mLogs[mLogs.Count - 1];

            if ( preData == _data )
            {
                return;
            }
        }

        mSequence++;
        _data.sequence = mSequence;

        mLogs.Add(_data);

        if ( ACTION_LOG_GEO_DATA != null && ACTION_LOG_GEO_DATA.GetInvocationList().Length > 0 )
        {
            ACTION_LOG_GEO_DATA(_data);
        }        
    }


    /// <summary>
    /// 데이터 유효 검증
    /// </summary>
    /// <param name="_data"></param>
    /// <returns></returns>
    protected virtual bool IsAvailable ( STGeoData _data )
    {
        return true;
    }

    /// <summary>
    /// 로그 리스트 반환
    /// </summary>
    /// <returns></returns>
    public List<STGeoData> Get()
    {
        return mLogs;
    }


    /// <summary>
    /// 로그 리스트 초기화
    /// </summary>
    public void Clear()
    {
        mSequence = 0;
        mLogs.Clear();
    }


    public void Release()
    {
        mLogs.Clear();
        mLogs = null;

        mGPSModule.ACTION_GPS_GEODATA -= onActionGpsData;
    }


    //protected void Test()
    //{
    //    double lat = 37.52642;
    //    double lng = 126.92681;

    //    for ( int i = 0; i < 1000; i++ )
    //    {
    //        int ranLat = UnityEngine.Random.Range(-20, 20);
    //        int ranLng = UnityEngine.Random.Range(-20, 20);

    //        lat += 0.00001f * ranLat;
    //        lng += 0.00001f * ranLng;

    //        STGeoData data = new STGeoData(0, i, lat, lng, 0, 0, 0, 0, 0);
    //        mLogs.Add(data);
    //    }
    //}
}
