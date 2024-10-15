using Assets.Scripts.Menus;
using UnityEngine;
using UnityEngine.UI;

public class RemovableTimerSelector : RemovableBase
{
    //attached to each building as an invisible 2dtoolkit button

    [SerializeField] private Text TimeCounterLb, Price;
    [SerializeField] private Slider ProgressBar;

    public GameObject //own child obj
        GainCrystals,
        GainExperience;

    public bool
        inRemovalB;

    public int
        crystalAward,
        xpAward,
        remainingTime = 1;

    public string removableName;

    public float progTime = 0.57f, progCounter; //for progress timer, one minute

    private int
        hours,
        minutes,
        seconds, //for time remaining label
        finishPrice; //price displayed for "finish now" button. based on remaining time

    // Use this for initialization
    private void Start()
    {
        //tween = GetComponent<StructureTween> ();
        soundFX = GameObject.Find("SoundFX").GetComponent<SoundFX>();
        relay = GameObject.Find("Relay").GetComponent<Relay>();


        if (!battleMap)
        {
            removableCreator = GameObject.Find("RemovableCreator").GetComponent<RemovableCreator>();
            stats = GameObject.Find("Stats").GetComponent<Stats>();
        }

        //init price so user can't click fast on price 0
        remainingTime = removeTime - 1;
        UpdatePrice(remainingTime);
    }

    private void FixedUpdate()
    {
        if (inRemovalB) ProgressBarUpdate();
    }

    private void ProgressBarUpdate()
    {
        progCounter += Time.deltaTime * 0.5f;
        if (progCounter > progTime)
        {
            progCounter = 0;

            ProgressBar.value += Time.deltaTime / removeTime; //update progress bars values

            ProgressBar.value = Mathf.Clamp(ProgressBar.value, 0, 1);

            remainingTime = (int)(removeTime * (1 - ProgressBar.value));

            UpdatePrice(remainingTime);
            UpdateTimeCounter(remainingTime);

            if (ProgressBar.value == 1) //building finished - the progress bar has reached 1												
            {
                ((SoundFX)soundFX).BuildingFinished();

                if (!battleMap) //if this building is not finished on a battle map
                {
                    ((Stats)stats).occupiedBuilders--; //the builder previously assigned becomes available

                    if (crystalAward > 0)
                    {
                        ((Stats)stats).AddResources(0, 0, crystalAward, 0, 0);
                        MessageController.Instance.DisplayMessage("You found " + crystalAward + " crystals in " +
                                                                  removableName);
                        var gainCrystalIco = Instantiate(GainCrystals);
                        gainCrystalIco.transform.SetParent(GameObject.Find("GroupEffects").transform);
                        gainCrystalIco.transform.position = transform.position + new Vector3(0, 75, 0);


                        gainCrystalIco.GetComponent<FadeOutGain>().SetValue(crystalAward); //pass the value to the label
                    }

                    ((Stats)stats).experience += xpAward;

                    var gainExperienceIco = Instantiate(GainExperience);
                    gainExperienceIco.transform.SetParent(GameObject.Find("GroupEffects").transform);
                    gainExperienceIco.transform.position = transform.position;

                    gainExperienceIco.GetComponent<FadeOutGain>().SetValue(xpAward); //pass the value to 
                    ((Stats)stats).UpdateUI();
                }

                inRemovalB = false;
                Destroy(transform.parent.gameObject);
            }
        }
    }

    private void UpdateTimeCounter(int remainingTime) //calculate remaining time
    {
        hours = remainingTime / 60;
        minutes = remainingTime % 60;
        seconds = (int)(60 - ProgressBar.value * removeTime * 60 % 60);

        if (minutes == 60) minutes = 0;
        if (seconds == 60) seconds = 0;

        UpdateTimeLabel();
    }

    private void UpdateTimeLabel() //update the time labels on top
    {
        if (hours > 0 && minutes > 0 && seconds >= 0)
            TimeCounterLb.text =
                hours + " h " +
                minutes + " m " +
                seconds + " s ";
        else if (minutes > 0 && seconds >= 0)
            TimeCounterLb.text =
                minutes + " m " +
                seconds + " s ";
        else if (seconds > 0)
            TimeCounterLb.text =
                seconds + " s ";
    }

    private void UpdatePrice(int remainingTime) //update the price label on the button, based on remaining time		
    {
        /*
        0		30		1
        30		60		3
        60		180		7
        180		600		15
        600		1440	30
        1440	2880	45
        2880	4320	70
        4320			150
         */

        if (remainingTime >= 4320)
            finishPrice = 150;
        else if (remainingTime >= 2880)
            finishPrice = 70;
        else if (remainingTime >= 1440)
            finishPrice = 45;
        else if (remainingTime >= 600)
            finishPrice = 30;
        else if (remainingTime >= 180)
            finishPrice = 15;
        else if (remainingTime >= 60)
            finishPrice = 7;
        else if (remainingTime >= 30)
            finishPrice = 3;
        else if (remainingTime >= 0) finishPrice = 1;

        Price.text = finishPrice.ToString();
    }

    public void Finish()
    {
        if (!((Relay)relay).pauseInput && !((Relay)relay).delay) //panels are open / buttons were just pressed 
        {
            ((SoundFX)soundFX).Click();
            if (((Stats)stats).crystals >= finishPrice)
            {
                ((Stats)stats).crystals -= finishPrice;
                ((Stats)stats).UpdateUI();
                ProgressBar.value = 1;
            }
            else
            {
                MessageController.Instance.DisplayMessage("Insufficient crystals");
            }
        }
    }
}