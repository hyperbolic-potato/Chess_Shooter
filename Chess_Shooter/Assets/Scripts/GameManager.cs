using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;
public class GameManager : MonoBehaviour
{
    PlayerController player;

    GameObject weaponUI;
    GameObject pauseMenu;

    Image healthBar;
    TextMeshProUGUI ammoCounter;
    TextMeshProUGUI clip;
    TextMeshProUGUI fireMode;

    public bool isPaused = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex >= 1)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

            weaponUI = GameObject.FindGameObjectWithTag("weaponUI");
            pauseMenu = GameObject.FindGameObjectWithTag("ui_pause");

            pauseMenu.SetActive(false);

            healthBar = GameObject.FindGameObjectWithTag("ui_health").GetComponent<Image>();
            ammoCounter = GameObject.FindGameObjectWithTag("ui_ammo").GetComponent<TextMeshProUGUI>();
            clip = GameObject.FindGameObjectWithTag("ui_clip").GetComponent<TextMeshProUGUI>();
            fireMode = GameObject.FindGameObjectWithTag("ui_fireMode").GetComponent<TextMeshProUGUI>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex >= 1)
        {
            healthBar.fillAmount = (float)player.health / (float)player.maxHealth;

            if (player.currentWeapon != null)
            {
                weaponUI.SetActive(true);

                ammoCounter.text = "Ammo: " + player.currentWeapon.ammo;
                clip.text = "Clip: " + player.currentWeapon.clip + " / " + player.currentWeapon.clipSize;

                if (player.currentWeapon.fireModes >= 2)
                {
                    fireMode.gameObject.SetActive(true);

                    switch (player.currentWeapon.weaponID)
                    {
                        case 1:
                            {
                                if (player.currentWeapon.currentFireMode == 0)
                                    fireMode.text = "Fire Mode: Semi-Auto";

                                else if (player.currentWeapon.currentFireMode == 1)
                                    fireMode.text = "Fire Mode: Full Auto";

                                break;
                            }

                        default:
                            fireMode.gameObject.SetActive(false);
                            break;
                    }
                }

                else
                    fireMode.gameObject.SetActive(false);
            }

            else
                weaponUI.SetActive(false);
        }
    }

    public void Pause()
    {
        if (!isPaused)
        {
            isPaused = true;

            pauseMenu.SetActive(true);

            Time.timeScale = 0;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        else
            Resume();
    }
    public void Resume()
    {
        if (isPaused)
        {
            isPaused = false;

            pauseMenu.SetActive(false);

            Time.timeScale = 1;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    public void LoadLevel(int level)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(level);
    }
    public void MainMenu()
    {
        LoadLevel(0);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
