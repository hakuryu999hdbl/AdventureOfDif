using OctoberStudio.Currency;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OctoberStudio
{
    using Upgrades;
    using Vibration;
    using Save;
    using OctoberStudio.Audio;
    using OctoberStudio.Easing;
    using OctoberStudio.Input;

    public class GameController : MonoBehaviour
    {
        private static GameController instance;

        [SerializeField] CurrenciesManager currenciesManager;
        public static CurrenciesManager CurrenciesManager => instance.currenciesManager;

        [SerializeField] UpgradesManager upgradesManager;
        public static UpgradesManager UpgradesManager => instance.upgradesManager;

        public static ISaveManager SaveManager { get; private set; }
        public static IAudioManager AudioManager { get; private set; }
        public static IVibrationManager VibrationManager { get; private set; }
        public static IInputManager InputManager { get; private set; }

        public static CurrencySave Gold { get; private set; }
        public static CurrencySave TempGold { get; private set; }

        public static AudioSource Music { get; private set; }

        private static StageSave stageSave;

        // Indicates that the main menu is just loaded, and not exited from the game scene
        public static bool FirstTimeLoaded { get; private set; }

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(this);

                FirstTimeLoaded = false;

                return;
            }

            instance = this;

            FirstTimeLoaded = true;

            currenciesManager.Init();

            DontDestroyOnLoad(gameObject);

            Application.targetFrameRate = 120;
        }

        private void Start()
        {
            Gold = SaveManager.GetSave<CurrencySave>("gold");
            TempGold = SaveManager.GetSave<CurrencySave>("temp_gold");

            stageSave = SaveManager.GetSave<StageSave>("Stage");

            if (!stageSave.loadedBefore)
            {
                stageSave.loadedBefore = true;

                //Gold.Deposit(1000);
            }

            EasingManager.DoAfter(0.1f, () => Music = AudioManager.AudioDatabase.Music.Play(true));
        }

        public static void RegisterInputManager(IInputManager inputManager)
        {
            InputManager = inputManager;
        }

        public static void RegisterSaveManager(ISaveManager saveManager)
        {
            SaveManager = saveManager;
        }

        public static void RegisterVibrationManager(IVibrationManager vibrationManager)
        {
            VibrationManager = vibrationManager;
        }

        public static void RegisterAudioManager(IAudioManager audioManager)
        {
            AudioManager = audioManager;
        }

        public static void LoadStage()
        {
            if(stageSave.ResetStageData) TempGold.Withdraw(TempGold.Amount);

            instance.StartCoroutine(StageLoadingCoroutine());

            SaveManager.Save(false);
        }

        public static void LoadMainMenu()
        {
            Gold.Deposit(TempGold.Amount);
            TempGold.Withdraw(TempGold.Amount);

            if (instance != null) instance.StartCoroutine(MainMenuLoadingCoroutine());

            SaveManager.Save(false);
        }

        private static IEnumerator StageLoadingCoroutine()
        {
            yield return LoadAsyncScene("Loading Screen", LoadSceneMode.Additive);
            yield return UnloadAsyncScene("Main Menu");
            yield return LoadAsyncScene("Game", LoadSceneMode.Single);
        }

        private static IEnumerator MainMenuLoadingCoroutine()
        {
            yield return LoadAsyncScene("Loading Screen", LoadSceneMode.Additive);
            yield return UnloadAsyncScene("Game");
            yield return LoadAsyncScene("Main Menu", LoadSceneMode.Single);
        }

        private static IEnumerator UnloadAsyncScene(string sceneName)
        {
            var asyncLoad = SceneManager.UnloadSceneAsync(sceneName);
            asyncLoad.allowSceneActivation = false;
            //wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }

        private static IEnumerator LoadAsyncScene(string sceneName, LoadSceneMode loadSceneMode)
        {
            var asyncLoad = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
            asyncLoad.allowSceneActivation = false;
            //wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone)
            {
                //scene has loaded as much as possible,
                // the last 10% can't be multi-threaded
                if (asyncLoad.progress >= 0.9f)
                {
                    asyncLoad.allowSceneActivation = true;
                }
                yield return null;
            }
        }

        private void OnApplicationFocus(bool focus)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            if (focus) { 
                EasingManager.DoAfter(0.1f, () => { 
                    if (!Music.isPlaying)
                    {
                        Music = AudioManager.AudioDatabase.Music.Play(true);
                    }
                });
            } 
#endif
        }
    }
}