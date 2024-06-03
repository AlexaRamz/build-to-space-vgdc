using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    public List<Sprite> hairList;
    public List<Sprite> hatList;
    public int currentHairIndex = 0;
    int currentHatIndex = 0;
    public Character plrChar;
    public GameObject plrModel;
    [SerializeField] Slider hueSlider;
    [SerializeField] SelectionButton[] colorButtons;

    int selectedColor = 0;

    private void Awake()
    {
        plrChar.ClearSave();
    }
    public void Save()
    {
        plrChar.SaveChar(hairList[currentHairIndex], hatList[currentHatIndex], colorButtons[1].image.color, colorButtons[0].image.color);
    }
    public void SaveChar()
    {
        Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void UpdateChar()
    {
        Save();
        plrChar.LoadChar(plrModel);
    }
    public void SwitchHairLeft()
    {
        if (currentHairIndex == 0)
        {
            currentHairIndex = hairList.Count - 1;
        }
        else
        {
            currentHairIndex -= 1;
        }
        UpdateChar();
    }
    public void SwitchHairRight()
    {
        if (currentHairIndex + 1 < hairList.Count)
        {
            currentHairIndex++;
        }
        else
        {
            currentHairIndex = 0;
        }
        UpdateChar();
    }
    public void SwitchHatLeft()
    {
        if (currentHatIndex == 0)
        {
            currentHatIndex = hatList.Count - 1;
        }
        else
        {
            currentHatIndex -= 1;
        }
        UpdateChar();
    }
    public void SwitchHatRight()
    {
        if (currentHatIndex + 1 < hatList.Count)
        {
            currentHatIndex++;
        }
        else
        {
            currentHatIndex = 0;
        }
        UpdateChar();
    }
    public void OnHueChanged()
    {
        colorButtons[selectedColor].image.color = Color.HSVToRGB(hueSlider.value, 1f, 1f);
        UpdateChar();
    }
    public void SelectColor(int btnNo)
    {
        colorButtons[selectedColor].SetSelection(false);
        selectedColor = btnNo;
        colorButtons[selectedColor].SetSelection(true);
    }
}
