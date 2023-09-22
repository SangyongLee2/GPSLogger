using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPSLoggerDataStorage
{
    protected List<List<STGeoData>> mLogs;
    

    public GPSLoggerDataStorage()
    {
        mLogs = new List<List<STGeoData>>();
    }


    public int Add ( List<STGeoData> _data )
    {
        List<STGeoData> list = new List<STGeoData>();
        list.AddRange(_data);

        mLogs.Add(list);

        return mLogs.Count - 1;
    }


    public List<STGeoData> Get ( int _index )
    {
        if ( mLogs.Count <= _index )
        {
            return null;
        }

        return mLogs[_index];
    }


    public List<STGeoData> GetHead()
    {
        if ( mLogs.Count <= 0 )
        {
            return null;
        }

        return mLogs[0];
    }


    public List<STGeoData> GetTail()
    {
        if ( mLogs.Count <= 0 )
        {
            return null;
        }

        return mLogs[mLogs.Count - 1];
    }


    public void Release()
    {
        mLogs.Clear();
        mLogs = null;
    }
}
