using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopupUploadResult : MonoBehaviour
{
    public Button buttonClose;
    public TextMeshProUGUI textResult;


    protected void Start()
    {
        buttonClose.onClick.AddListener(onClose);
    }


    public void SetReult ( string _res )
    {
        textResult.text = _res;
    }


    protected void onClose()
    {
        this.gameObject.SetActive(false);
    }
}
