using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;


public class SoundManager : SingletonMonobehaviour<SoundManager>
{
	public enum MusicPlayingType { None = 0, SourceA = 1, SourceB = 2, AtoB = 3, BtoA = 4 }
	[HideInInspector]
	public AudioMixer mixer = null; //오디오 믹서.
	[HideInInspector]
	public GameObject fadeA_GameObject = null; // 첫번째 배경음 게임오브젝트.
	[HideInInspector]
	public GameObject fadeB_GameObject = null; // 두번째 배경음 게임오브젝트.
	[HideInInspector]
	public AudioSource fadeA_audioSource = null; //첫번째 배경음 오디오소스.
	[HideInInspector]
	public AudioSource fadeB_audioSource = null; //두번째 배경음 오디오소스.

	public GameObject[] effect_GameObject = null; //이펙트 게임오브젝트.
	[HideInInspector]
	public AudioSource[] effect_audioSource = null; //이펙트 오디오소스.
	[HideInInspector]
	public float[] Effect_PlayStartTime = null; // 이펙트 오디오 시작타임.
	[HideInInspector]
	public GameObject UI_GameObject = null; // UI 게임오브젝트.
	[HideInInspector]
	public AudioSource UI_AudioSource = null; // UI 오디오소스.

	MusicPlayingType currentPlayingType = MusicPlayingType.None; //현재 재생 타입.
	bool isTicking = false;

	SoundClip currentSound = null; //현재 재생중인 사운드.
	SoundClip lastSound = null; //마지막으로 재생한 사운드.
	SoundClip storedSound = null; //임시 저장된 사운드.
	float storedTime = 0.0f; //임시 저장됐을시의 플레이 타임.

	private BGMSoundPlayer currentSceneSoundPlayer = null; // 사운드 플레이어.



	int effectAudioChannelCount = 5; // 이펙트 오디오 채널 카운트.
	Transform subRoot = null;
	float minVolume = -80.0f;
	float maxVolume = 0.0f;
	bool nowMute = false;
	float lastEffectVolume = 0.0f;
	float lastUIVolume = 0.0f;
	float lastBGMVolume = 0.0f;
	AudioListener listener = null;

	public AudioMixerGroup[] AllMixerGroups //오디오 믹서 그룹.
	{
		get
		{
			if (this.mixer != null)
			{
				return this.mixer.FindMatchingGroups(string.Empty);
			}
			else
			{
				return null;
			}
		}
	}

	private void Start()
	{
		if (this.mixer == null)
		{
			this.mixer = Resources.Load("AudioMixer") as AudioMixer;
		}

		if (this.subRoot == null)
		{
			this.subRoot = new GameObject("SoundContainer").transform;
			this.subRoot.SetParent(this.transform);
		}

		if (this.fadeA_GameObject == null)
		{
			this.fadeA_GameObject = new GameObject("fadeA");
			this.fadeA_GameObject.transform.SetParent(this.subRoot);
			this.fadeA_audioSource = this.fadeA_GameObject.AddComponent<AudioSource>();
			this.fadeA_audioSource.playOnAwake = false;
		}
		if (this.fadeB_GameObject == null)
		{
			this.fadeB_GameObject = new GameObject("fadeB");
			this.fadeB_GameObject.transform.SetParent(this.subRoot);
			this.fadeB_audioSource = this.fadeB_GameObject.AddComponent<AudioSource>();
			this.fadeB_audioSource.playOnAwake = false;
		}

		if (this.UI_GameObject == null)
		{
			this.UI_GameObject = new GameObject("UI");
			this.UI_GameObject.transform.SetParent(this.subRoot);
			this.UI_AudioSource = this.UI_GameObject.AddComponent<AudioSource>();
			this.UI_AudioSource.playOnAwake = false;
		}
		if (this.effect_GameObject == null || this.effect_GameObject.Length == 0)
		{
			this.Effect_PlayStartTime = new float[effectAudioChannelCount];
			this.effect_GameObject = new GameObject[effectAudioChannelCount];
			this.effect_audioSource = new AudioSource[effectAudioChannelCount];
			for (int i = 0; i < effectAudioChannelCount; i++)
			{
				this.Effect_PlayStartTime[i] = 0.0f;
				this.effect_GameObject[i] = new GameObject("Effect_" + i.ToString());
				this.effect_GameObject[i].transform.SetParent(this.subRoot);
				this.effect_audioSource[i] = this.effect_GameObject[i].AddComponent<AudioSource>();
				this.effect_audioSource[i].playOnAwake = false;
			}
		}
		if (this.mixer != null)
		{
			this.fadeA_audioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("BGM")[0];
			this.fadeB_audioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("BGM")[0];

			this.UI_AudioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("UI")[0];
			for (int i = 0; i < effectAudioChannelCount; i++)
			{
				this.effect_audioSource[i].outputAudioMixerGroup = mixer.FindMatchingGroups("Effect")[0];
			}


		}
		if (listener == null)
		{
			listener = gameObject.AddComponent<AudioListener>();
		}
		///
		this.VolumeInit();
	}
	/// <summary>
	/// 볼륨 초기화. 디바이스에 저장된 마지막 저장값으로 지정해 놓는다.
	/// </summary>
	public void VolumeInit()
	{
		if (PlayerPrefs.HasKey("BGM_Volume"))
		{
			float bgmVol = PlayerPrefs.GetFloat("BGM_Volume");
			this.SetBGMVolume(bgmVol, 1.0f);
		}
		else
		{
			this.SetBGMVolume(0.5f, 1.0f);
		}
		if (PlayerPrefs.HasKey("UI_Volume"))
		{
			float uiVol = PlayerPrefs.GetFloat("UI_Volume");
			this.SetUIVolume(uiVol, 1.0f);
		}
		else
		{
			this.SetUIVolume(1.0f, 1.0f);
		}
		if (PlayerPrefs.HasKey("Effect_Volume"))
		{
			float effectVol = PlayerPrefs.GetFloat("Effect_Volume");
			this.SetEffectVolume(effectVol, 1.0f);
		}
		else
		{
			this.SetEffectVolume(1.0f, 1.0f);
		}
	}
	/// <summary>
	/// 현재 볼륨을 가져온다.
	/// </summary>
	public float GetCurrentBGMVolume()
	{
		return PlayerPrefs.GetFloat("BGM_Volume");
	}
	/// <summary>
	/// 현재 UI 볼륨을 가져온다.
	/// </summary>
	public float GetCurrentUIVolume()
	{
		return PlayerPrefs.GetFloat("UI_Volume");
	}
	/// <summary>
	/// 현재 effect 볼륨을 가져온다.
	/// </summary>
	public float GetCurrentEffectVolume()
	{
		return PlayerPrefs.GetFloat("Effect_Volume");
	}
	/// <summary>
	/// 
	/// </summary>
	private void Update()
	{
		if (this.currentSound == null)
		{
			return;
		}
		if (this.currentPlayingType == MusicPlayingType.SourceA)
		{
			this.currentSound.DoFade(Time.deltaTime, this.fadeA_audioSource);
		}
		else if (this.currentPlayingType == MusicPlayingType.SourceB)
		{
			this.currentSound.DoFade(Time.deltaTime, this.fadeB_audioSource);
		}
		else if (this.currentPlayingType == MusicPlayingType.AtoB)
		{
			this.lastSound.DoFade(Time.deltaTime, this.fadeA_audioSource);
			this.currentSound.DoFade(Time.deltaTime, this.fadeB_audioSource);
		}
		else if (this.currentPlayingType == MusicPlayingType.BtoA)
		{
			this.lastSound.DoFade(Time.deltaTime, this.fadeB_audioSource);
			this.currentSound.DoFade(Time.deltaTime, this.fadeA_audioSource);
		}

		if (this.fadeA_audioSource.isPlaying && !this.fadeB_audioSource.isPlaying) this.currentPlayingType = MusicPlayingType.SourceA;
		else if (this.fadeB_audioSource.isPlaying && !this.fadeA_audioSource.isPlaying) this.currentPlayingType = MusicPlayingType.SourceB;
		else if (!this.fadeA_audioSource.isPlaying && !this.fadeB_audioSource.isPlaying) this.currentPlayingType = MusicPlayingType.None;
	}

	/// <summary>
	/// 재생중인가.
	/// </summary>
	public bool IsPlaying()
	{
		return (int)this.currentPlayingType > 0;
	}
	/// <summary>
	/// 현재 재생중인 사운드 아이디.
	/// </summary>
	public int GetCurrentID()
	{
		int retID = -1;
		if (this.currentSound != null) retID = this.currentSound.realID;
		return retID;
	}
	/// <summary>
	/// 현재 재생 타임.
	/// </summary>
	public float GetCurrentTime()
	{
		float retTime = -1.0f;
		if (this.fadeA_audioSource && (this.currentPlayingType == MusicPlayingType.SourceA || this.currentPlayingType == MusicPlayingType.BtoA))
		{
			retTime = this.fadeA_audioSource.time;
		}
		else if (this.fadeB_audioSource && (this.currentPlayingType == MusicPlayingType.SourceB || this.currentPlayingType == MusicPlayingType.AtoB))
		{
			retTime = this.fadeB_audioSource.time;
		}
		return retTime;
	}
	/// <summary>
	/// 현재 재생중인 사운드를 임시저장.
	/// </summary>
	public void StoreCurrent()
	{
		this.storedSound = this.currentSound;
		if (this.currentPlayingType == MusicPlayingType.SourceA || this.currentPlayingType == MusicPlayingType.BtoA) this.storedTime = this.fadeA_audioSource.time;
		else if (this.currentPlayingType == MusicPlayingType.SourceB || this.currentPlayingType == MusicPlayingType.AtoB) this.storedTime = this.fadeB_audioSource.time;

	}
	/// <summary>
	/// 임시 저장된 사운드 재생.
	/// </summary>
	public void PlayStored()
	{
		this.Play(this.storedSound);
		this.SetTime(this.storedTime);
	}
	/// <summary>
	/// 저장된 사운드 페이드 인.
	/// </summary>
	public void FadeInStroed(float time, Interpolate.EaseType type)
	{
		this.FadeIn(this.storedSound, time, type);
		this.SetTime(this.storedTime);
	}
	public void FadeToStored(float time, Interpolate.EaseType type)
	{
		this.FadeTo(this.storedSound, time, type);
		this.SetTime(this.storedTime);
	}
	/// <summary>
	/// 현재 재생중인 사운드 인지 확인.
	/// </summary>
	public bool CheckPlay(SoundClip m)
	{
		if (m == null)
		{
			return false;
		}

		bool retPlay = true;
		if (this.currentSound != null && m.realID == this.currentSound.realID && IsPlaying() && !this.currentSound.isFadeOut)
		{
			retPlay = false;
		}
		return retPlay;
	}
	/// <summary>
	/// 재생 플레이 타임 지정.
	/// </summary>
	public void SetTime(float time)
	{
		if (this.currentPlayingType == MusicPlayingType.SourceA || this.currentPlayingType == MusicPlayingType.BtoA) this.fadeA_audioSource.time = time;
		else if (this.currentPlayingType == MusicPlayingType.SourceB || this.currentPlayingType == MusicPlayingType.AtoB) this.fadeB_audioSource.time = time;
	}

	/// <summary>
	/// 사운드 재생.
	/// </summary>
	public void Play(int index)
	{
		SoundClip tempAudio = DataManager.SoundData().GetCopy(index);
		if (tempAudio != null)
		{
			this.Play(tempAudio);
		}
		else
		{
			Debug.LogWarning("Can not find SoundClip at " + index.ToString());
		}
	}

	/// <summary>
	/// 특정 타임부터 재생.
	/// </summary>
	public void PlayFromTime(int index, float t)
	{
		this.Play(DataManager.SoundData().GetCopy(index));
		this.SetTime(t);
	}

	/// <summary>
	/// 배경음 사운드 재생.
	/// </summary>
	public void Play(SoundClip m)
	{
		if (this.CheckPlay(m))
		{
			this.fadeB_audioSource.Stop();
			this.lastSound = this.currentSound;
			this.currentSound = m;

			PlayAudioSource(this.fadeA_audioSource, m);

			this.currentPlayingType = MusicPlayingType.SourceA;
			if (this.currentSound.HasLoop())
			{
				this.isTicking = true;
				DoTick();
			}
		}
	}

	/// <summary>
	/// 이펙트,UI 같은 일회성 사운드 재생.
	/// </summary>
	public void PlayOneShot(SoundClip m)
	{
		if (m == null)
		{
			Debug.LogWarning("[Failed] param is null!");
		}
		m.PreLoad();

		switch (m.playType)
		{
			case SoundPlayType.EFFECT:
				Play_EffectSound(m);
				break;

			case SoundPlayType.BGM:
				Play(m);
				break;
			case SoundPlayType.None:
				Debug.LogWarning("SoundType이 None 으로 잘못 설정 되어 있음.:" + m.realID.ToString());
				break;
			case SoundPlayType.UI:
				Play_UISound(m);
				break;
		}
	}

	/// <summary>
	/// 사운드 재생.
	/// </summary>
	void PlayAudioSource(AudioSource source, SoundClip m)
	{
		if (source == null)
		{
			return;
		}
		source.Stop();
		source.clip = m.GetClip();
		source.volume = m.maxVolume;
		source.loop = m.isLoop;
		source.pitch = m.pitch;
		source.dopplerLevel = m.dopplerLevel;
		source.rolloffMode = m.rollOffMode;
		source.minDistance = m.minDistance;
		source.maxDistance = m.maxDistance;
		source.spatialBlend = m.spatialBlend;
		source.Play();
	}

	/// <summary>
	/// UI 사운드 재생.
	/// </summary>
	void Play_UISound(SoundClip m)
	{
		PlayAudioSource(this.UI_AudioSource, m);
	}

	/// <summary>
	/// 이펙트 사운드 재생.
	/// </summary>
	void Play_EffectSound(SoundClip m)
	{
		bool isPlaySuccess = false;
		//가지고 있는 오디오 채널중에 플레이중이 아닌 채널을 찾아보거나.
		for (int idx = 0; idx < this.effectAudioChannelCount; idx++)
		{
			if (this.effect_audioSource[idx].isPlaying == false)
			{
				PlayAudioSource(this.effect_audioSource[idx], m);
				this.Effect_PlayStartTime[idx] = Time.realtimeSinceStartup;
				isPlaySuccess = true;
				break;
			}
			else if (this.effect_audioSource[idx].clip == m.GetClip())
			{
				//같은 사운드가 거의 동시에 재생하여야 하는 경우 특정 폰의 경우 소리가 클리핑이나 피크 현상이 발생.
				//기존 사운드를 멈췄다가 다시 재생하는게 덜 어색할지도.
				this.effect_audioSource[idx].Stop();
				PlayAudioSource(this.effect_audioSource[idx], m);
				this.Effect_PlayStartTime[idx] = Time.realtimeSinceStartup;

				isPlaySuccess = true;
				break;
			}
		}

		if (isPlaySuccess == false)
		{
			float standTime = 0.0f;
			int selectIndex = 0;
			for (int idx = 0; idx < this.effectAudioChannelCount; idx++)
			{
				if (this.Effect_PlayStartTime[idx] > standTime)
				{
					standTime = this.Effect_PlayStartTime[idx];
					selectIndex = idx;
				}
			}
			PlayAudioSource(this.effect_audioSource[selectIndex], m);
		}
	}

	/// <summary>
	/// 일회성 사운드 재생.
	/// </summary>
	public void PlayOneShot(string clipName)
	{
		try
		{
			SoundList soundList = (SoundList)Enum.Parse(typeof(SoundList), clipName);
			this.PlayOneShot((int)soundList);
		}
		catch (System.Exception ex)
		{
			Debug.LogError("Can not find Correct SoundClip , Please Check Sound Name At Editor :[" + clipName + "] / " + ex.Message.ToString());
		}
	}

	/// <summary>
	/// PlayOneShot((int)SoundList.S_EFX_BOMB1)
	/// </summary>
	/// <param name="index"></param>
	public void PlayOneShot(int index)
	{
		if (index == (int)SoundList.None)
		{
			return;
		}
		if (index < DataManager.SoundData().soundClips.Length)
		{
			this.PlayOneShot(DataManager.SoundData().GetCopy(index));
		}
	}
	/// <summary>
	/// 중지.
	/// </summary>
	public void Stop(bool allStop = false)
	{
		if (allStop == true)
		{
			this.fadeA_audioSource.Stop();
			this.fadeB_audioSource.Stop();
		}

		this.FadeOut(0.5f, Interpolate.EaseType.Linear);
		this.currentPlayingType = MusicPlayingType.None;
		this.isTicking = false;
	}
	/// <summary>
	/// 사운드 페이드인
	/// </summary>
	public void FadeIn(int index, float time, Interpolate.EaseType ease)
	{
		this.FadeIn(DataManager.SoundData().GetCopy(index), time, ease);
	}

	public void FadeIn(SoundClip m, float time, Interpolate.EaseType ease)
	{
		if (this.CheckPlay(m))
		{
			this.fadeA_audioSource.Stop();
			this.fadeB_audioSource.Stop();
			this.lastSound = this.currentSound;
			this.currentSound = m;
			if (this.currentSound != null) this.currentSound.PreLoad();
			this.fadeA_audioSource.clip = this.currentSound.GetClip();
			this.fadeA_audioSource.volume = 0;
			this.fadeA_audioSource.loop = this.currentSound.isLoop;
			this.fadeA_audioSource.pitch = this.currentSound.pitch;
			this.fadeA_audioSource.dopplerLevel = this.currentSound.dopplerLevel;
			this.fadeA_audioSource.rolloffMode = this.currentSound.rollOffMode;
			this.fadeA_audioSource.minDistance = this.currentSound.minDistance;
			this.fadeA_audioSource.maxDistance = this.currentSound.maxDistance;
			this.fadeA_audioSource.spatialBlend = this.currentSound.spatialBlend;

			this.currentSound.FadeIn(time, ease);
			this.fadeA_audioSource.Play();
			this.currentPlayingType = MusicPlayingType.SourceA;
			if (this.currentSound.HasLoop())
			{
				this.isTicking = true;
				DoTick();
			}
		}
	}
	/// <summary>
	/// 페이드 아웃.
	/// </summary>
	public void FadeOut(float time, Interpolate.EaseType ease)
	{
		if (this.currentSound != null)
		{
			this.currentSound.FadeOut(time, ease);
		}
	}
	/// <summary>
	/// 페이드 사운드.
	/// </summary>
	public void FadeTo(string soundName, float time, Interpolate.EaseType ease)
	{
		try
		{
			SoundList clip = (SoundList)Enum.Parse(typeof(SoundList), soundName);
			this.FadeTo(DataManager.SoundData().GetCopy((int)clip), time, ease);
		}
		catch (System.Exception ex)
		{
			Debug.LogError("Can not find Correct SoundClip, Please Check Sound Name At Editor:[" + soundName + "] / " + ex.Message.ToString());
		}
	}

	public void FadeTo(int index, float time, Interpolate.EaseType ease)
	{
		this.FadeTo(DataManager.SoundData().GetCopy(index), time, ease);
	}

	public void FadeTo(SoundClip m, float time, Interpolate.EaseType ease)
	{
		if (this.currentPlayingType == MusicPlayingType.None)
		{
			this.FadeIn(m, time, ease);
		}
		else if (this.CheckPlay(m))
		{
			if (this.currentPlayingType == MusicPlayingType.AtoB)
			{
				this.fadeA_audioSource.Stop();
				this.currentPlayingType = MusicPlayingType.SourceB;
			}
			else if (this.currentPlayingType == MusicPlayingType.BtoA)
			{
				this.fadeB_audioSource.Stop();
				this.currentPlayingType = MusicPlayingType.SourceA;
			}

			this.lastSound = this.currentSound;
			this.currentSound = m;
			if (this.currentSound != null) this.currentSound.PreLoad();
			this.lastSound.FadeOut(time, ease);
			this.currentSound.FadeIn(time, ease);
			if (this.currentPlayingType == MusicPlayingType.SourceA)
			{
				this.fadeB_audioSource.clip = this.currentSound.GetClip();
				this.fadeB_audioSource.volume = 0.0f;
				this.fadeB_audioSource.loop = this.currentSound.isLoop;
				this.fadeB_audioSource.pitch = this.currentSound.pitch;
				this.fadeB_audioSource.dopplerLevel = this.currentSound.dopplerLevel;
				this.fadeB_audioSource.rolloffMode = this.currentSound.rollOffMode;
				this.fadeB_audioSource.minDistance = this.currentSound.minDistance;
				this.fadeB_audioSource.maxDistance = this.currentSound.maxDistance;
				this.fadeB_audioSource.spatialBlend = this.currentSound.spatialBlend;
				this.fadeB_audioSource.Play();
				this.currentPlayingType = MusicPlayingType.AtoB;

			}
			else if (this.currentPlayingType == MusicPlayingType.SourceB)
			{
				this.fadeA_audioSource.clip = this.currentSound.GetClip();
				this.fadeA_audioSource.volume = 0.0f;
				this.fadeA_audioSource.loop = this.currentSound.isLoop;
				this.fadeA_audioSource.pitch = this.currentSound.pitch;
				this.fadeA_audioSource.dopplerLevel = this.currentSound.dopplerLevel;
				this.fadeA_audioSource.rolloffMode = this.currentSound.rollOffMode;
				this.fadeA_audioSource.minDistance = this.currentSound.minDistance;
				this.fadeA_audioSource.maxDistance = this.currentSound.maxDistance;
				this.fadeA_audioSource.spatialBlend = this.currentSound.spatialBlend;
				this.fadeA_audioSource.Play();
				this.currentPlayingType = MusicPlayingType.BtoA;
			}
			if (this.currentSound.HasLoop())
			{
				this.isTicking = true;
				DoTick();
			}
		}
	}

	/// <summary>
	/// 사운드매니저 메인 Update.
	/// </summary>
	public void DoTick()
	{
		StartCoroutine(DoTick2());
	}

	private IEnumerator DoTick2()
	{
		while (this.isTicking && (int)this.currentPlayingType > 0)
		{
			yield return new WaitForSeconds(0.05f);
			if (this.currentSound.HasLoop())
			{
				if (this.currentPlayingType == MusicPlayingType.SourceA)
				{
					this.currentSound.CheckLoop(this.fadeA_audioSource);
				}
				else if (this.currentPlayingType == MusicPlayingType.SourceB)
				{
					this.currentSound.CheckLoop(this.fadeB_audioSource);
				}
				else if (this.currentPlayingType == MusicPlayingType.AtoB)
				{
					this.lastSound.CheckLoop(this.fadeA_audioSource);
					this.currentSound.CheckLoop(this.fadeB_audioSource);
				}
				else if (this.currentPlayingType == MusicPlayingType.BtoA)
				{
					this.lastSound.CheckLoop(this.fadeB_audioSource);
					this.currentSound.CheckLoop(this.fadeA_audioSource);
				}
			}
		}
	}
	/// <summary>
	/// 배경음 출력.
	/// </summary>
	public void Play_BGM()
	{
		if (currentSceneSoundPlayer == null)
		{
			if (FindBasicPlayer() == false)
			{
				Debug.LogWarning("Can not find BGM Player~!! ");
			}
		}
		if (currentSceneSoundPlayer != null)
		{
			if (IsPlaying() == false)
			{
				currentSceneSoundPlayer.PlayMusic();
			}
		}
	}
	/// <summary>
	/// 배경 플레이어 검색,.
	/// </summary>
	public bool FindBasicPlayer()
	{
		bool retVal = false;
		BGMSoundPlayer[] players = FindObjectsOfType<BGMSoundPlayer>();
		foreach (BGMSoundPlayer player in players)
		{
			if (player.startType == EventStartType.AUTOSTART)
			{
				currentSceneSoundPlayer = player;
				retVal = true;
				break;
			}
		}

		return retVal;
	}
	/// <summary>
	/// SetBGMVolume
	/// </summary>
	public void SetBGMVolume(float current, float maxLimit)
	{
		if (maxLimit == 0)
		{
			Debug.LogWarning("최대값은 0 일수 없습니다.");
			return;
		}

		if (current > maxLimit)
		{
			Debug.LogWarning("설정값이 최대값을 넘을 수 없습니다.");
			return;
		}
		float setVol = 0.0f;
		if (current == 0.0f)
		{
			setVol = this.minVolume;
		}
		else if (current == maxLimit)
		{
			setVol = this.maxVolume;
		}
		else
		{
			setVol = Mathf.Lerp(-25.0f, 0.0f, (current / maxLimit));
		}
		this.mixer.SetFloat("BGM_Volume", setVol);
		PlayerPrefs.SetFloat("BGM_Volume", current);


	}
	/// <summary>
	/// SetEffectVolume
	/// </summary>
	public void SetEffectVolume(float current, float maxLimit)
	{
		if (maxLimit == 0)
		{
			Debug.LogWarning("최대값은 0 일수 없습니다.");
			return;
		}

		if (current > maxLimit)
		{
			Debug.LogWarning("설정값이 최대값을 넘을 수 없습니다.");
			return;
		}
		float setvol = 0.0f;
		if (current == 0.0f)
		{
			setvol = this.minVolume;
		}
		else if (current == maxLimit)
		{
			setvol = this.maxVolume;
		}
		else
		{
			setvol = Mathf.Lerp(-25.0f, 0.0f, (current / maxLimit));
		}
		this.mixer.SetFloat("Effect_Volume", setvol);
		PlayerPrefs.SetFloat("Effect_Volume", current);
	}
	/// <summary>
	/// SetUIVolume
	/// </summary>
	public void SetUIVolume(float current, float maxLimit)
	{
		if (maxLimit == 0)
		{
			Debug.LogWarning("최대값은 0 일수 없습니다.");
			return;
		}

		if (current > maxLimit)
		{
			Debug.LogWarning("설정값이 최대값을 넘을 수 없습니다.");
			return;
		}
		float setvol = 0.0f;
		if (current == 0.0f)
		{
			setvol = this.minVolume;
		}
		else if (current == maxLimit)
		{
			setvol = this.maxVolume;
		}
		else
		{
			setvol = Mathf.Lerp(-25.0f, 0.0f, (current / maxLimit));
		}
		this.mixer.SetFloat("UI_Volume", setvol);
		PlayerPrefs.SetFloat("UI_Volume", current);
	}
	/// <summary>
	/// SetOnOff
	/// </summary>
	public bool SetOnOff()
	{
		if (this.nowMute == false) //!소리 끄고.
		{
			this.mixer.GetFloat("BGM_Volume", out this.lastBGMVolume);
			this.mixer.GetFloat("UI_Volume", out this.lastUIVolume);
			this.mixer.GetFloat("Effect_Volume", out this.lastEffectVolume);
			this.mixer.SetFloat("BGM_Volume", this.minVolume);
			this.mixer.SetFloat("UI_Volume", this.minVolume);
			this.mixer.SetFloat("Effect_Volume", this.minVolume);
		}
		else //! 소리 킨다.
		{
			this.mixer.SetFloat("BGM_Volume", this.lastBGMVolume);
			this.mixer.SetFloat("UI_Volume", this.lastUIVolume);
			this.mixer.SetFloat("Effect_Volume", this.lastEffectVolume);
		}
		this.nowMute = !this.nowMute;
		return this.nowMute;
	}
	/// <summary>
	/// SetMute
	/// </summary>
	public bool SetMute(bool p_Mute)
	{
		return this.nowMute = p_Mute;
	}
	/// <summary>
	/// 모든 이펙트 사운드 종료.
	/// </summary>
	public void StopAllEffectSound()
	{
		if (effect_audioSource != null && effect_audioSource.Length > 0)
		{
			for (int i = 0; i < effect_audioSource.Length; i++)
			{
				effect_audioSource[i].Stop();
			}
		}
	}

}
