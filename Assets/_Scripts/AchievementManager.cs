using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IObserverPattern;
using UnityEngine.UI;

enum ACHIEVEMENTS
{
    VOID_EXPLORER = 0,
    BUILDER,
    ACHIEVEMENT_UNLOCKER,
    ACHIEVEMENT_UNLOCKER_UNLOCKER,
    ACHIEVEMENT_UNLOCKER_UNLOCKER_UNLOCKER,
    ACHIEVEMENT_UNLOCKER_UNLOCKER_UNLOCKER_UNLOCKER,
    NUM_ACHIEVEMENTS
}

class Achievement
{
    public Achievement(string a_name = "wew lad", string a_description = "idk")
    {
        name = a_name;
        description = a_description;
        unlocked = false;
    }

    public void AttemptUnlock()
    {
        if (!unlocked)
        {
            unlocked = true;
            Debug.Log("Achievement Unlocked: -=<" + name + ">=- \n" + description);
            // something else
        }
    }

    public string name { get; private set; }
    public string description { get; private set; }
    public bool unlocked { get; private set; }
}

public class AchievementManager : IObserver
{
    [Header("Observees")]
    public PlayerAchievementChecker player;
    public BlockEditingSuite blockEditor;
    // Achievement Manager also observes itself

    [Header("Achievement popup box")]
    public GameObject textBox;
    public Text titleField;
    public Text descField;


    float popupTime = 0.5f;
    float holdTime = 5.0f;
    float exitTime = 0.5f;

    // achievements list
    static Achievement[] allAchievements;
    List<Achievement> unlockedAchievements;

    //
    enum AchievementPopupState
    {
        NONE = 0,
        POPUP,
        HOLD,
        EXIT,
        NUM_POPUP_STATES
    }

    private float timer;
    private int lastUnlockedAchievementIndex = 0;
    AchievementPopupState state = AchievementPopupState.NONE;



    // achievement-tracking stats
    private uint numBlocksPlaced = 0;

    private void Start()
    {
        // set up observee subscriptions
        SubscribeTo(player);
        SubscribeTo(blockEditor);
        SubscribeTo(this);
        //

        textBox.SetActive(false);

        // container to hold achievements that are already unlocked
        unlockedAchievements = new List<Achievement>();

        // create achievements
        allAchievements = new Achievement[(int)ACHIEVEMENTS.NUM_ACHIEVEMENTS];

        allAchievements[(int)ACHIEVEMENTS.VOID_EXPLORER] = new Achievement(
            "Void Explorer",
            "You fell into the infinite nothing. R.I.P."
            );

        allAchievements[(int)ACHIEVEMENTS.BUILDER] = new Achievement(
            "Builder",
            "Place 10 blocks"
            );

        allAchievements[(int)ACHIEVEMENTS.ACHIEVEMENT_UNLOCKER] = new Achievement(
           "Achievement Hunter",
           "Unlock 2 Achievements"
           );

        allAchievements[(int)ACHIEVEMENTS.ACHIEVEMENT_UNLOCKER_UNLOCKER] = new Achievement(
            "\"Achievement Hunter\" Achievement Hunter",
            "Unlock the \"Achievement Hunter\" Achievement (wew)"
            );

        allAchievements[(int)ACHIEVEMENTS.ACHIEVEMENT_UNLOCKER_UNLOCKER_UNLOCKER] = new Achievement(
            "\"\"Achievement Hunter\" Achievement Hunter\" Achievement Hunter",
            "Wow, congratulations on your achievement! You're such a winner!"
            );

        allAchievements[(int)ACHIEVEMENTS.ACHIEVEMENT_UNLOCKER_UNLOCKER_UNLOCKER_UNLOCKER] = new Achievement(
            "C-C-C-COMBO BREAKER",
            "old memes die hard"
            );
    }

    private void UnlockAchievement(ACHIEVEMENTS ach)
    {
        Achievement achObject = allAchievements[(int)ach];

        // if not already unlocked or queued to unlock
        if (!unlockedAchievements.Contains(achObject))
        {
            achObject.AttemptUnlock();
            unlockedAchievements.Add(achObject);
            NotifyAll(gameObject, OBSERVER_EVENT.UNLOCKED_ACHIEVEMENT);
        }
    }

    public override void OnNotify(GameObject gameObject, OBSERVER_EVENT oEvent)
    {
        switch (oEvent)
        {
            case OBSERVER_EVENT.ENTERED_VOID:
                {
                    UnlockAchievement(ACHIEVEMENTS.VOID_EXPLORER);
                    break;
                }

            case OBSERVER_EVENT.PLACED_BLOCK:
                {
                    if (numBlocksPlaced < 10)
                    {
                        numBlocksPlaced++;

                        if (numBlocksPlaced >= 10)
                        {
                            UnlockAchievement(ACHIEVEMENTS.BUILDER);
                        }
                    }
                    break;
                }

            case OBSERVER_EVENT.UNLOCKED_ACHIEVEMENT:
                {
                    if (unlockedAchievements.Count >= 2)
                    {
                        {
                            UnlockAchievement(ACHIEVEMENTS.ACHIEVEMENT_UNLOCKER);
                        }

                        // if the last achievement unlocked was the achievement unlocker achievement
                        if (unlockedAchievements.Contains(allAchievements[(int)ACHIEVEMENTS.ACHIEVEMENT_UNLOCKER]))
                        {
                            UnlockAchievement(ACHIEVEMENTS.ACHIEVEMENT_UNLOCKER_UNLOCKER);
                        }

                        if (unlockedAchievements.Contains(allAchievements[(int)ACHIEVEMENTS.ACHIEVEMENT_UNLOCKER_UNLOCKER]))
                        {
                            UnlockAchievement(ACHIEVEMENTS.ACHIEVEMENT_UNLOCKER_UNLOCKER_UNLOCKER);
                        }

                        if (unlockedAchievements.Contains(allAchievements[(int)ACHIEVEMENTS.ACHIEVEMENT_UNLOCKER_UNLOCKER_UNLOCKER]))
                        {
                            UnlockAchievement(ACHIEVEMENTS.ACHIEVEMENT_UNLOCKER_UNLOCKER_UNLOCKER_UNLOCKER);
                        }
                    }

                    break;
                }
        }
    }

    private void Update()
    {
        // do the "Achievement Unlocked" animation

        timer -= Time.smoothDeltaTime;

        if (lastUnlockedAchievementIndex < (unlockedAchievements.Count) && state == AchievementPopupState.NONE)
        {
            textBox.SetActive(true);
            timer = popupTime;
            state = AchievementPopupState.POPUP;

            titleField.text = unlockedAchievements[lastUnlockedAchievementIndex].name;
            descField.text = unlockedAchievements[lastUnlockedAchievementIndex].description;
        }

        switch (state)
        {
            case AchievementPopupState.POPUP:
                {
                    float tVal = Mathf.InverseLerp(popupTime, 0.0f, timer);
                    float scale = 0.2f;//Mathf.SmoothStep(0.0f, 1.0f, tVal);

                    textBox.GetComponent<RectTransform>().localScale.Set(scale, scale, scale);
                    //textBox.transform.localScale.Set(scale, scale, scale);

                    //Debug.Log("hello?");
                    break;
                }
            case AchievementPopupState.HOLD:
                {
                    float scale = Mathf.Sin(Time.time);
                    textBox.GetComponent<RectTransform>().rotation.SetEulerAngles(0.0f, 0.0f, Mathf.Sin(Time.time) * 6.28f);
                    textBox.GetComponent<RectTransform>().localScale.Set(scale, scale, scale);
                    //Debug.Log("are you?");
                    break;
                }
            case AchievementPopupState.EXIT:
                {
                    float tVal = Mathf.InverseLerp(exitTime, 0.0f, timer);
                    float scale = Mathf.SmoothStep(1.0f, 0.0f, tVal);

                    textBox.transform.localScale.Set(scale, scale, scale);
                    //Debug.Log("there?");
                    break;
                }
        }

        // if state ends
        if (timer <= 0.0f)
        {
            switch (state)
            {
                case AchievementPopupState.POPUP:
                    {
                        timer = holdTime;
                        state = AchievementPopupState.HOLD;
                        break;
                    }
                case AchievementPopupState.HOLD:
                    {
                        timer = exitTime;
                        state = AchievementPopupState.EXIT;
                        break;
                    }
                case AchievementPopupState.EXIT:
                    {
                        lastUnlockedAchievementIndex++;
                        state = AchievementPopupState.NONE;
                        textBox.SetActive(false);
                        break;
                    }
            }
        }
    }
}