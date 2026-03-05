using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject creditPanel;

    public AudioSource bgmSource;
    public AudioSource sfxSource;

    public AudioClip buttonSound;

    void Start()
    {
        bgmSource.Play(); // เล่น BGM ตอนเข้าเมนู
    }

    public void StartGame()
    {
        sfxSource.PlayOneShot(buttonSound);
        SceneManager.LoadScene("GameScene");
    }

    public void OpenCredit()
    {
        sfxSource.PlayOneShot(buttonSound);
        creditPanel.SetActive(true);
    }

    public void CloseCredit()
    {
        sfxSource.PlayOneShot(buttonSound);
        creditPanel.SetActive(false);
    }

    public void QuitGame()
    {
        sfxSource.PlayOneShot(buttonSound);
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}