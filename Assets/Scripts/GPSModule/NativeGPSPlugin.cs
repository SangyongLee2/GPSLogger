using System.Runtime.InteropServices;
using UnityEngine;

#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

public class NativeGPSPlugin : MonoBehaviour
{
    static NativeGPSPlugin instance = null;
    static GameObject go;

#if UNITY_ANDROID
	static AndroidJavaClass obj;

#endif

    #region Dll imports for iOS
#if UNITY_IOS
    [DllImport("__Internal")] private static extern void startLocation();
    [DllImport("__Internal")] private static extern void stopLocation();
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
			if(instance == null)
			{
				go = new GameObject();
				go.name = "NativeGPSPlugin";
				instance = go.AddComponent<NativeGPSPlugin>();

                #if UNITY_ANDROID

				if(Application.platform == RuntimePlatform.Android)
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

    public static bool StartLocation()
	{
        Instance.Awake();

#if UNITY_IOS

        if(Application.platform == RuntimePlatform.IPhonePlayer)
        {
            startLocation();
        }

#elif UNITY_ANDROID


        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            PermissionCallbacks callback = new PermissionCallbacks();
            callback.PermissionGranted += (st) => { obj.CallStatic("startLocation"); };

            Permission.RequestUserPermission(Permission.FineLocation, callback);
        }
        else
        {
            obj.CallStatic("startLocation");
        }

        #endif

        return true;
	}


    public static void StopLocation()
    {
#if UNITY_IOS

        if(Application.platform == RuntimePlatform.IPhonePlayer)
        {
            stopLocation();
        }

#elif UNITY_ANDROID

        obj.CallStatic("stopLocation");

#endif
    }


    #region Getting GPS properties

    public static double GetTimestamp()
    {
#if UNITY_IOS

        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            return getTimestamp();
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
        
            return (double) Get(NativeAndroidFunction.GET_LONGITUDE);

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
        
            return (double) Get(NativeAndroidFunction.GET_LATITUDE);
        
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
        
            return (float) Get(NativeAndroidFunction.GET_ACCURACY);
        
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
        
            return (double) Get(NativeAndroidFunction.GET_ALTITUDE);
        
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
        
            return (float) Get(NativeAndroidFunction.GET_SPEED);
        
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
        
            return (float) Get(NativeAndroidFunction.GET_SPEED_ACCURACY_METERS_PER_SECOND);
        
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
        
            return (float) Get(NativeAndroidFunction.GET_VERTICAL_ACCURACY_METERS);
        
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
        if ( v < 0 )
        {
            v += 360;
        }

        return v;

#endif

        return 0;
    }

    public static float GetHeadingAccuracy()
    {
#if UNITY_IOS

        if(Application.platform == RuntimePlatform.IPhonePlayer)
        {
            return getHeadingAccuracy();
        }

#elif UNITY_ANDROID

        return (int)Get(NativeAndroidFunction.GET_HEADING_ACCURACY);
#endif

        return 0;
    }


    #endregion

    #region Android functions
#if UNITY_ANDROID

    private static object Get(NativeAndroidFunction functionName)
    {
        Instance.Awake();

        if(!Input.location.isEnabledByUser)
        {
            return 0;
        }
        
        if(Application.platform == RuntimePlatform.Android)
        {
            switch(functionName)
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
    }
#endif
    #endregion
}
