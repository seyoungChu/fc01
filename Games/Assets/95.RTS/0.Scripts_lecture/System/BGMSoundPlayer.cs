using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMSoundPlayer : BaseInteraction
{
	/// <summary>사운드 플레이 타입 </summary>
    public MusicPlayType playType = MusicPlayType.PLAY;
    /// <summary> 플레이어에 필요한 인덱스들 </summary>
    public int musicClip = 0;
    public int musicClip_Second = 0;
    /// <summary> 페이드 타임 </summary>
    public float fadeTime = 1;
    /// <summary> 보간 방정식 </summary>
    public Interpolate.EaseType interpolate = Interpolate.EaseType.Linear;

    void Awake()
    {
        //musicplayer = collider layer set
        gameObject.layer = LayerMask.NameToLayer("Default");
    }
    void Start()
    {
        //! AUTOSTART라면 바로 플레이 해준다.
        if (EventStartType.AUTOSTART.Equals(this.startType)/* && this.CheckVariables()*/)
        {
            this.PlayMusic();
        }
    }
    void Update()
    {
        if (this.KeyPress()) this.PlayMusic();
        if (this.CheckVariables())
        {
            this.PlayMusic();
            //반복 이벤트가 아닌경우 한번만 재생하기 위해 조건을 삭제.
            if (!this.repeatExecution)
            {
                RemoveVariableCondition(0);
            }
        }
    }
    /// <summary>
    /// OnTriggerEnter
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        if (this.CheckTriggerEnter(other))
        {
            this.PlayMusic();
        }
    }
    /// <summary>
    /// OnTriggerStay
    /// </summary>
    void OnTriggerStay(Collider other)
    {
        if (this.CheckTriggerEnter(other))
        {
            this.PlayMusic();
        }
    }
    /// <summary>
    /// OnTriggerExit
    /// </summary>
    void OnTriggerExit(Collider other)
    {
        if (this.CheckTriggerExit(other))
        {
            this.PlayMusic();
        }
    }
    /// <summary>
    /// TouchInteract
    /// </summary>
    public override void TouchInteract()
    {
        this.OnMouseUp();
    }
    /// <summary>
    /// OnMouseUp
    /// </summary>
    void OnMouseUp()
    {
        if (EventStartType.INTERACT.Equals(this.startType) && this.CheckVariables() &&
                this.gameObject.activeInHierarchy)
        {
            this.PlayMusic();
        }
    }
    /// <summary>
    /// Interact
    /// </summary>
    public override bool Interact()
    {
        bool val = false;
        // start event on interaction here
        if (EventStartType.INTERACT.Equals(this.startType) && this.CheckVariables() && this.gameObject.activeInHierarchy)
        {
            this.PlayMusic();
            val = true;
        }
        return val;
    }
    /// <summary>
    /// PlayMusic
    /// </summary>
    public void PlayMusic()
    {

        if (MusicPlayType.PLAY.Equals(this.playType))  // 하나의 musicclip을 플레이함..
        {
            SoundManager.Instance.Play(this.musicClip);
        }
        else if (MusicPlayType.STOP.Equals(this.playType))
        {
            SoundManager.Instance.Stop();
        }
        else if (MusicPlayType.FADE_IN.Equals(this.playType))
        {
            SoundManager.Instance.FadeIn(this.musicClip, this.fadeTime, this.interpolate);
        }
        else if (MusicPlayType.FADE_OUT.Equals(this.playType))
        {
            SoundManager.Instance.FadeOut(this.fadeTime, this.interpolate);
        }
        else if (MusicPlayType.FADE_TO.Equals(this.playType))
        {
            SoundManager.Instance.FadeTo(this.musicClip, this.fadeTime, this.interpolate);
        }
    }
    /// <summary>
    /// OnDrawGizmos
    /// </summary>
    //void OnDrawGizmos()
    //{
    //    Gizmos.DrawIcon(transform.position, "musicplayer.png");
    //}

}
