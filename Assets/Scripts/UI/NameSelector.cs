using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NameSelector : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private Button connectionBtn;
    [SerializeField] private int minNameLength = 1;
    [SerializeField] private int maxNameLength = 12;
    
    public const string PlayerNameKey = "PlayerName";
    private void Start()
    {
        if(SystemInfo.graphicsDeviceType ==UnityEngine.Rendering.GraphicsDeviceType.Null){
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
            return;
        }

        nameInput.text=PlayerPrefs.GetString(PlayerNameKey,string.Empty);//get data from playerPrefs
        HandleNameChange();
    }
    public void HandleNameChange()
    {
        connectionBtn.interactable = nameInput.text.Length >= minNameLength && nameInput.text.Length <= maxNameLength;
    }

    public void Connect(){
        PlayerPrefs.SetString(PlayerNameKey,nameInput.text);//save data to PlayerPrefs
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }
}
