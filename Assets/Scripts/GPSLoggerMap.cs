using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OSMClient;

public class GPSLoggerMap : MonoBehaviour
{
    public OSMController map;
    public LineRenderer lineRenderer;

    protected List<STGeoData> datas;

    protected void Awake()
    {
        datas = new List<STGeoData>();
        map.MapChanged += OnActionChangeMap;
    }


    public void OnActionGPSData ( STGeoData _data )
    {
        Vector2d gps = new Vector2d(_data.lng, _data.lat);
        SetViewDirection(gps);

        map.Location = gps;
    }


    public void Create ( List<STGeoData> _data )
    {
        datas.Clear();
        datas.AddRange(_data);

        lineRenderer.positionCount = _data.Count;

        for ( int i = 0; i < _data.Count; i++ )
        {
            STGeoData data = _data[i];

            Vector2d gps = calculateGpsPosition(data);
            Vector3 position = map.LocationToPosition(gps.y, gps.x);

            lineRenderer.SetPosition(i, position);
        }
    }


    public STGeoData ViewGPSPositionData ( int _index )
    {
        STGeoData data = datas[_index];

        Vector2d gps = calculateGpsPosition(data);
        SetViewDirection(gps);

        map.Location = gps;

        return data;
    }


    public void Clear()
    {
        datas.Clear();
        lineRenderer.positionCount = 0;
    }


    protected void OnActionChangeMap ( OSMController _map )
    {
        if ( datas.Count < 1 )
        {
            return;
        }

        for ( int i = 0; i < datas.Count; i++ )
        {
            STGeoData data = datas[i];

            Vector2d gps = calculateGpsPosition(data);
            Vector3 position = _map.LocationToPosition(gps.y, gps.x);

            lineRenderer.SetPosition(i, position);
        }        
    }


    protected Vector2d calculateGpsPosition (STGeoData _data )
    {
        return new Vector2d(_data.lng, _data.lat);
    }


    private void SetViewDirection ( Vector2d _gps )
    {
        Vector2d o = map.Location;
        map.SetViewDirection(o, _gps);
    }
}
