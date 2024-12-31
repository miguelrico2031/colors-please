
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {
        ServiceLocator.Get<IDayService>().ResetDays();
        ServiceLocator.Get<IDayService>().StartDay();
    }
}
