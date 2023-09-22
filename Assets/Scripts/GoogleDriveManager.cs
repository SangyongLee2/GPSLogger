//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityGoogleDrive.Data;
//using UnityGoogleDrive;

//public class GoogleDriveManager
//{
//    public static GoogleDriveManager Instance
//    {
//        get
//        {
//            if ( instance == null )
//            {
//                instance = new GoogleDriveManager();
//                instance.initialize();
//            }

//            return instance;
//        }
//    }

//    protected static GoogleDriveManager instance;


//    protected void initialize()
//    {
//        GoogleDriveFiles.List().Send().OnDone += OnActionSendDone;
//    }


//    private void OnActionSendDone ( FileList obj )
//    {
//        Debug.Log("###Google Drive Send Done");    
//    }


//    protected void release()
//    {

//    }


//    public void Upload()
//    {
//        File file = new UnityGoogleDrive.Data.File
//        {
//            Name = "Test",
//            Content = null
//        };

//        GoogleDriveFiles.Create(file).Send();
//    }


//    public void Download()
//    {

//    }
//}
