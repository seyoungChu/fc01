using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BGMSoundPlayer))]
public class BGMSoundPlayerInspector : BaseInspector
{
    public override void OnInspectorGUI()
	{
		BGMSoundPlayer player = (BGMSoundPlayer)target;
        GUILayout.Label("Music setup", EditorStyles.boldLabel);
        this.EventStartSettings(player);   //CompSoundPlayer 가 들어가있는 object
        EditorGUILayout.Separator();

        //+재생 타입 설정.
        player.playType = (MusicPlayType)EditorGUILayout.EnumPopup("Play type", player.playType);
        if (!player.playType.Equals(MusicPlayType.FADE_OUT) && !player.playType.Equals(MusicPlayType.STOP))
        {  //+ 재생할 클립 선택.
            player.musicClip = EditorGUILayout.Popup("Music clip", player.musicClip, DataManager.SoundData().GetNameList(true));
        }
        
        if (!player.playType.Equals(MusicPlayType.PLAY) && !player.playType.Equals(MusicPlayType.STOP))
        {   //+ 페이드 타임과 페이드 보간공식을 선택.
            player.fadeTime = EditorGUILayout.FloatField("Fade time (s)", player.fadeTime);
            player.interpolate = (Interpolate.EaseType)EditorGUILayout.EnumPopup("Interpolation", player.interpolate);
        }

        EditorGUILayout.Separator();
        this.VariableSettings(player);
        EditorGUILayout.Separator();
        //+ 바뀐게 있으면 적용.
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
	}

}
