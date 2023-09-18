using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public struct STGeoLogData
{
    public STGeoData geoData;
    public long timeGap;
    public double posGapH;
    public double posGapV;

    public STGeoLogData ( STGeoData _data, long _timeGap, double _posGapH, double _posGapV )
    {
        geoData = _data;
        timeGap = _timeGap;
        posGapH = _posGapH;
        posGapV = _posGapV;
    }
}

public class GPSLogger
{
    public Action<STGeoLogData> ACTION_LOG_GEO_DATA;

    protected GPSModule mGpsModule;
    protected List<STGeoLogData> mLogs;


    public GPSLogger ( GPSModule _module )
    {
        mGpsModule = _module;
        mLogs = new List<STGeoLogData>();

        mGpsModule.ACTION_GPS_GEODATA += onActionGpsData;
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

        long timeGap = 0;
        double posGapH = 0;
        double posGapV = 0;

        if ( mLogs.Count > 0 )
        {
            STGeoLogData preLogData = mLogs[mLogs.Count - 1];
            STGeoData preData = preLogData.geoData;

            timeGap = _data.timeStamp - preData.timeStamp;
            posGapH = _data.lat - preData.lat;
            posGapV = _data.lng - preData.lng;
        }

        STGeoLogData logData = new STGeoLogData(_data, timeGap, posGapH, posGapV);
        mLogs.Add(logData);

        if ( ACTION_LOG_GEO_DATA != null && ACTION_LOG_GEO_DATA.GetInvocationList().Length > 0 )
        {
            ACTION_LOG_GEO_DATA(logData);
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
    public IEnumerator Get()
    {
        return mLogs.GetEnumerator();
    }


    /// <summary>
    /// 로그 리스트 초기화
    /// </summary>
    public void Clear()
    {
        mLogs.Clear();
    }
}
