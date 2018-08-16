using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : SingletonMonobehaviour<TimeManager>
{

    public List<Timer> timerList = new List<Timer>();

    public float accumTime = 0.0f;

    public int accumFrame = 0;

    public void AddTimer(Timer newTimer)
    {
        if (timerList.Contains(newTimer) == false)
        {
            timerList.Add(newTimer);
        }
        else
        {
            Debug.LogWarning("AddTimer Timer Already Contains");
        }
    }

    public void RemoveTimer(string id)
    {
        for (int i = 0; i < timerList.Count; i++)
        {
            if (timerList[i].ID.Equals(id) == true)
            {
                timerList[i].Stop();
                timerList.RemoveAt(i);
                break;
            }
        }
    }

    public void RemoveTimer(Timer timer)
    {
        if (timerList.Contains(timer) == true)
        {
            timer.Stop();
            timerList.Remove(timer);
        }
    }

    public void UpdateTime()
    {
        accumTime += Time.deltaTime;
        accumFrame++;

        if(timerList.Count > 0)
        {
            for (int i = 0; i < timerList.Count; i++)
            {
                timerList[i].OnUpdate();
            }
        }
    }

}
