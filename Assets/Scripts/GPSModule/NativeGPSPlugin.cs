using System.Runtime.InteropServices;
using UnityEngine;
using System;

#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif


public enum eHeadingAccuracy
{
    DISAVAILABLE,       //유효하지 않음
    UNRELIABLE,         //신뢰할 수 없음
    LOW,                //낮음
    MIDDLE,             //중간
    HIGH,               //높음
}

public class NativeGPSPlugin : MonoBehaviour
{
    static NativeGPSPlugin instance = null;
    static GameObject go;

#if UNITY_ANDROID
    static AndroidJavaClass obj;

#endif

    #region Dll imports for iOS
#if UNITY_IOS
    [DllImport("__Internal")] private static extern void initialize();
    [DllImport("__Internal")] private static extern void startLocation();
    [DllImport("__Internal")] private static extern void stopLocation();
    [DllImport("__Internal")] private static extern void destroy();
    [DllImport("__Internal")] private static extern bool hasUserAuthorize();
    [DllImport("__Internal")] private static extern bool isEnableGps();
    [DllImport("__Internal")] private static extern double getTimestamp();
    [DllImport("__Internal")] private static extern double getLongitude();
    [DllImport("__Internal")] private static extern double getLatitude();
    [DllImport("__Internal")] private static extern double getAltitude();
    [DllImport("__Internal")] private static extern float getAccuracy();
    [DllImport("__Internal")] private static extern float getVerticalAccuracyMeters();
    [DllImport("__Internal")] private static extern float getSpeed();
    [DllImport("__Internal")] private static extern float getSpeedAccuracy();
    [DllImport("__Internal")] private static extern float getHeading();
    [DllImport("__Internal")] private static extern float getHeadingAccuracy();

#endif
    #endregion

    #region Init Singleton
    public static NativeGPSPlugin Instance
    {
        get
        {
            if (instance == null)
            {
                go = new GameObject();
                go.name = "NativeGPSPlugin";
                instance = go.AddComponent<NativeGPSPlugin>();

#if UNITY_ANDROID

                if (Application.platform == RuntimePlatform.Android)
                    obj = new AndroidJavaClass("com.natris.locationservice.Main");

#endif
            }

            return instance;
        }
    }
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
    }
    #endregion


    public static void Initialize(Action _callback)
    {
        Instance.Awake();

        Debug.Log("NativeGps : Initialize");

#if UNITY_IOS

            initialize();

            if ( _callback != null )
            {
                _callback();
            }
            

#elif UNITY_ANDROID

        if (HasUserAuthorize() == false)
        {
            PermissionCallbacks callback = new PermissionCallbacks();
            callback.PermissionGranted += (st) =>
            {
                obj.CallStatic("initialize");

                if (_callback != null)
                {
                    _callback();
                }
            };

            Permission.RequestUserPermission(Permission.FineLocation, callback);
        }
        else
        {
            obj.CallStatic("initialize");
            if (_callback != null)
            {
                _callback();
            }
        }

#endif
    }


    public static void Destroy()
    {
#if UNITY_IOS

        destroy();

#elif UNITY_ANDROID

        obj.CallStatic("destroy");

#endif
        instance = null;
    }


    public static bool StartLocation()
    {

#if UNITY_IOS

        startLocation();

        return true;

#elif UNITY_ANDROID

        if (HasUserAuthorize() == false)
        {
            return false;
        }

        obj.CallStatic("startLocation");
#endif

        return true;

    }


    public static void StopLocation()
    {

#if UNITY_IOS

        if( Application.platform == RuntimePlatform.IPhonePlayer )
        {
            stopLocation();
        }

#elif UNITY_ANDROID

        obj.CallStatic("stopLocation");

#endif
    }


    public static bool HasUserAuthorize()
    {

#if UNITY_IOS

        return hasUserAuthorize();

#elif UNITY_ANDROID

        bool authorize = Permission.HasUserAuthorizedPermission(Permission.FineLocation);

        return authorize;

#endif

        return false;
    }


    public static bool IsEnableGPS()
    {

#if UNITY_IOS

        bool res = isEnableGps();
        return res;

#elif UNITY_ANDROID

        int res = (int)Get(NativeAndroidFunction.GET_ENABLE_GPS);
        return res == 1;
#endif

        return false;
    }


    #region Getting GPS properties

    public static long GetTimestamp()
    {
#if UNITY_IOS

        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            return (long)getTimestamp();
        }

#elif UNITY_ANDROID

        return (long)Get(NativeAndroidFunction.GET_TIMESTAMP);

#endif

        return 0;

    }

    public static double GetLongitude()
    {
#if UNITY_IOS

        if(Application.platform == RuntimePlatform.IPhonePlayer)
        {
            return getLongitude();
        }

#elif UNITY_ANDROID

        return (double)Get(NativeAndroidFunction.GET_LONGITUDE);

#endif

        return 0;
    }

    public static double GetLatitude()
    {
#if UNITY_IOS

        if(Application.platform == RuntimePlatform.IPhonePlayer)
        {
            return getLatitude();
        }

#elif UNITY_ANDROID

        return (double)Get(NativeAndroidFunction.GET_LATITUDE);

#endif

        return 0;
    }

    public static float GetAccuracy()
    {
#if UNITY_IOS

        if(Application.platform == RuntimePlatform.IPhonePlayer)
        {
            return getAccuracy();
        }

#elif UNITY_ANDROID

        return (float)Get(NativeAndroidFunction.GET_ACCURACY);

#endif

        return 0;
    }

    public static double GetAltitude()
    {
#if UNITY_IOS

        if(Application.platform == RuntimePlatform.IPhonePlayer)
        {
            return getAltitude();
        }

#elif UNITY_ANDROID

        return (double)Get(NativeAndroidFunction.GET_ALTITUDE);

#endif

        return 0;
    }

    public static float GetSpeed()
    {
#if UNITY_IOS

        if(Application.platform == RuntimePlatform.IPhonePlayer)
        {
            return getSpeed();
        }

#elif UNITY_ANDROID

        return (float)Get(NativeAndroidFunction.GET_SPEED);

#endif

        return 0;
    }

    public static float GetSpeedAccuracyMetersPerSecond()
    {
#if UNITY_IOS

        if(Application.platform == RuntimePlatform.IPhonePlayer)
        {
            return getSpeedAccuracy();
        }

#elif UNITY_ANDROID

        return (float)Get(NativeAndroidFunction.GET_SPEED_ACCURACY_METERS_PER_SECOND);

#endif

        return 0;
    }

    public static float GetVerticalAccuracyMeters()
    {
#if UNITY_IOS

        if(Application.platform == RuntimePlatform.IPhonePlayer)
        {
            return getVerticalAccuracyMeters();
        }

#elif UNITY_ANDROID

        return (float)Get(NativeAndroidFunction.GET_VERTICAL_ACCURACY_METERS);

#endif

        return 0;
    }


    public static float GetHeading()
    {
#if UNITY_IOS

        if(Application.platform == RuntimePlatform.IPhonePlayer)
        {
            return getHeading();
        }

#elif UNITY_ANDROID

        float v = (float)Get(NativeAndroidFunction.GET_HEADING) * Mathf.Rad2Deg;

        //Convert 0 to 360
        if (v < 0)
        {
            v += 360;
        }

        return v;

#endif

        return 0;
    }

    public static eHeadingAccuracy GetHeadingAccuracy()
    {
#if UNITY_IOS

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                float v = getHeadingAccuracy();
                eHeadingAccuracy accuracy = default;

                if (v < 0)
                {
                    accuracy = eHeadingAccuracy.DISAVAILABLE;
                }
                else if (v == 0)
                {
                    accuracy = eHeadingAccuracy.UNRELIABLE;
                }
                else if ( v < 30 )
                {
                    accuracy = eHeadingAccuracy.HIGH;
                }
                else if ( v < 70 )
                {
                    accuracy = eHeadingAccuracy.MIDDLE;
                }
                else
                {
                    accuracy = eHeadingAccuracy.LOW;
                }

                return accuracy;
            }

#elif UNITY_ANDROID

        return (eHeadingAccuracy)((int)Get(NativeAndroidFunction.GET_HEADING_ACCURACY));
#endif

        return 0;
    }

    #endregion

    #region Android functions
#if UNITY_ANDROID

    private static object Get(NativeAndroidFunction functionName)
    {
        Instance.Awake();

        if (!Input.location.isEnabledByUser)
        {
            return 0;
        }

        if (Application.platform == RuntimePlatform.Android)
        {
            switch (functionName)
            {
                case NativeAndroidFunction.GET_LONGITUDE:
                    return obj.CallStatic<double>("getLongitude");
                case NativeAndroidFunction.GET_LATITUDE:
                    return obj.CallStatic<double>("getLatitude");
                case NativeAndroidFunction.GET_ACCURACY:
                    return obj.CallStatic<float>("getAccuracy");
                case NativeAndroidFunction.GET_ALTITUDE:
                    return obj.CallStatic<double>("getAltitude");
                case NativeAndroidFunction.GET_SPEED:
                    return obj.CallStatic<float>("getSpeed");
                case NativeAndroidFunction.GET_SPEED_ACCURACY_METERS_PER_SECOND:
                    return obj.CallStatic<float>("getSpeedAccuracyMetersPerSecond");
                case NativeAndroidFunction.GET_VERTICAL_ACCURACY_METERS:
                    return obj.CallStatic<float>("getVerticalAccuracyMeters");
                case NativeAndroidFunction.GET_TIMESTAMP:
                    return obj.CallStatic<long>("getTimestamp");
                case NativeAndroidFunction.GET_HEADING:
                    return obj.CallStatic<float>("getHeading");
                case NativeAndroidFunction.GET_HEADING_ACCURACY:
                    return obj.CallStatic<int>("getHeadingAccuracy");
                case NativeAndroidFunction.GET_ENABLE_GPS:
                    return obj.CallStatic<int>("getEnableGps");
                case NativeAndroidFunction.GET_ENABLE_GPS_GPS:
                    return obj.CallStatic<int>("getEnableGpsGps");
                case NativeAndroidFunction.GET_ENABLE_GPS_NETWORK:
                    return obj.CallStatic<int>("getEnableGpsNetwork");
                case NativeAndroidFunction.GET_ENABLE_GPS_PASSIVE:
                    return obj.CallStatic<int>("getEnableGpsPassive");
            }
        }

        return null;
    }

    private enum NativeAndroidFunction
    {
        GET_LONGITUDE,
        GET_LATITUDE,
        GET_ACCURACY,
        GET_ALTITUDE,
        GET_SPEED,
        GET_SPEED_ACCURACY_METERS_PER_SECOND,
        GET_VERTICAL_ACCURACY_METERS,
        GET_TIMESTAMP,
        GET_HEADING,
        GET_HEADING_ACCURACY,
        GET_ENABLE_GPS,
        GET_ENABLE_GPS_GPS,
        GET_ENABLE_GPS_NETWORK,
        GET_ENABLE_GPS_PASSIVE,
    }
#endif
    #endregion
}
