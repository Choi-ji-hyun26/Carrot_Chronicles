using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingManager : MonoBehaviour
{
    public GameObject SettingMenu;
    public void ViewSettingMenu()
    {
        SettingMenu.SetActive(true);
    }
    public void Resume()
    {
        SettingMenu.SetActive(false);
    }

    public void Exit()
    {
        GameManager.Instance.Exit();
    }
}
