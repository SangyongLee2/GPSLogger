using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Text;
using UnityEngine.Networking;

public enum eAPI
{
    API_LIST,
    API_SAVE,
}


public enum eAPIType
{
    GET,
    POST
}


public struct STPacketRequestSave
{
    public string deviceId;
    public List<STGeoData> gpsList;


    public STPacketRequestSave ( string _deviceId, List<STGeoData> _gpsList )
    {
        deviceId = _deviceId;
        gpsList = _gpsList;
    }
}

public struct STPacketRequestList
{
    public string deviceId;


    public STPacketRequestList( string _deviceId )
    {
        deviceId = _deviceId;
    }
}


public struct STPacketResponseList
{
    public string code;
    public List<STGpsSetEntity> list;
}


public struct STGpsSetEntity
{
    public string deviceId;
    public string dataId;
    public int sequenceCount;
    public long createAt;
}


public struct STAPIInfo
{
    public string address;
    public eAPIType apiType;

    public STAPIInfo ( string _address, eAPIType _apiType )
    {
        address = _address;
        apiType = _apiType;
    }
}


public delegate void CallbackResponse(object _data);
public delegate void NetworkError(string _error);


public class GPSLoggerNetwork : MonoBehaviour
{
    public NetworkError ON_NETWORK_ERROR;

    //protected const string CONST_BASE_URI = "http://192.168.0.170:{0}{1}";
    //protected const int CONST_PORT = 8080;
    protected const string CONST_BASE_URI = "https://ilcggps.idess.kr:{0}{1}";
    protected const int CONST_PORT = 443;

    protected Dictionary<eAPI, STAPIInfo> mAPIUri;


    protected void Awake()
    {
        initialize();
    }


    protected void initialize()
    {
        mAPIUri = new Dictionary<eAPI, STAPIInfo>();

        mAPIUri.Add(eAPI.API_LIST, new STAPIInfo("/api/gps/dataset/list", eAPIType.POST));
        mAPIUri.Add(eAPI.API_SAVE, new STAPIInfo("/api/gps/save", eAPIType.POST));
    }


    public void RequestSave ( List<STGeoData> _gpsList, CallbackResponse _callback )
    {
        string deviceId = SystemInfo.deviceUniqueIdentifier;

        STPacketRequestSave packet = new STPacketRequestSave(deviceId, _gpsList);
        SendRequest(eAPI.API_SAVE, packet, _callback);
    }


    public void RequestGetList ( CallbackResponse _callback )
    {
        string deviceId = SystemInfo.deviceUniqueIdentifier;

        STPacketRequestList packet = new STPacketRequestList(deviceId);
        SendRequest(eAPI.API_LIST, packet, _callback);
    }





    protected void SendRequest ( eAPI _api, object _body = null, CallbackResponse _callback = null )
    {
        STAPIInfo apiInfo = mAPIUri[_api];

        string apiUri = apiInfo.address;
        string address = string.Format(CONST_BASE_URI, CONST_PORT, apiUri);

        Debug.Log("###API È£Ãâ : " + address);

        Uri uri = new Uri(address);

        switch (apiInfo.apiType)
        {
            case eAPIType.GET:
                {
                    StartCoroutine(Get(uri, _callback));
                    break;
                }
            case eAPIType.POST:
                {
                    string data = JsonConvert.SerializeObject(_body);
                    StartCoroutine(Post(uri, data, _callback));
                    break;
                }
                
        }
    }



    protected IEnumerator Post ( Uri _uri, string _data, CallbackResponse _callback )
    {
        UnityWebRequest req = UnityWebRequest.Post(_uri, _data);
        req.SetRequestHeader("Content-Type", "application/json");

        byte[] data = Encoding.UTF8.GetBytes(_data);
        req.uploadHandler = new UploadHandlerRaw(data);

        yield return req.SendWebRequest();

        if ( req.result == UnityWebRequest.Result.Success )
        {
            Debug.Log(req.downloadHandler.text);

            if ( _callback != null )
            {
                _callback(req.downloadHandler.text);
            }
        }
        else
        {
            Debug.Log(req.error);

            if ( ON_NETWORK_ERROR != null && ON_NETWORK_ERROR.GetInvocationList().Length > 0 )
            {
                ON_NETWORK_ERROR(req.error);
            }            
        }
    }


    protected IEnumerator Get ( Uri _uri, CallbackResponse _callback )
    {
        UnityWebRequest req = UnityWebRequest.Get(_uri);
        yield return req.SendWebRequest();

        if ( req.result == UnityWebRequest.Result.Success )
        {
            Debug.Log(req.downloadHandler.data);
            Debug.Log(req.downloadHandler.text);

            if ( _callback != null )
            {
                _callback(req.downloadHandler.text);
            }
        }
        else
        {
            Debug.Log(req.error); 

            if ( ON_NETWORK_ERROR != null && ON_NETWORK_ERROR.GetInvocationList().Length > 0 )
            {
                ON_NETWORK_ERROR(req.error);
            }
        }
    }
}
